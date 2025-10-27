using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        pricing = new PricingService(hub);
        tradeService = new();
        tradeSlot = new();
    }

    [SerializeField]
    private PriceModifierHub hub;
    [SerializeField]
    private ItemCollector itemCollector;
    [SerializeField]
    private List<Visitor> allVisitor;
    [SerializeField]
    private TextMeshProUGUI textPlate;
    [SerializeField]
    private NumericInputField numeric;

    private TradeService tradeService;


    private PricingService pricing;
    private TradeSession current;

    public event Action<TradeSession> OnCreateTradeSession;

    private void Start()
    {
        UIManager.Instance.TradeSlot.Init(tradeSlot);
    }

    public PriceQuote Quote(Item item, Visitor visitor, TradeRequest tradeRequest) => pricing.GetQuote(item, visitor, tradeRequest);


    private Visitor currentVisitor;
    private TradeRequest currentRequest;
    private ItemSlot tradeSlot;

    //나중에는 다른곳으로 옮기던가 할 수 있음
    public Visitor CreateVisitor()
    {
        return allVisitor[UnityEngine.Random.Range(0, allVisitor.Count)];
    }

    public void TerminationTrade()
    {
        current = null;
        currentRequest = null;
        currentVisitor = null;
        if(InventoryManager.Instance.PlayerChest.InsertItem(tradeSlot.Item))
        {
            Debug.Log("trade -> playerchest");
        }
        else
        {
            Debug.Log("trade -> null");
        }
        tradeSlot.Insert(null);
    }

    //만남
    public void StartEncounter(Visitor visitor)
    {
        currentVisitor = visitor;

        if (InventoryManager.Instance.PlayerChest.Count() < 10)
            currentRequest = new ItemTradeRequest(TradeType.Sell, visitor, itemCollector.Items);
        else
            currentRequest = UnityEngine.Random.value > 0.5f ? new ItemTradeRequest(TradeType.Sell, visitor, itemCollector.Items) : new CategoryTradeRequest(TradeType.Buy, visitor);
        
        if (currentRequest.TradeType == TradeType.Buy)
            UIManager.Instance.TradeSlot.gameObject.SetActive(true);

        Debug.Log("상대가 원하는 것 " + currentRequest.Summary);
        Emit(DialogueEvent.Arrive, TradeResult.None, 0);
        Emit(DialogueEvent.BrowseHint, TradeResult.None, 0);
    }

    //1 거래시작
    public void StartTrade()
    {
        if (currentVisitor == null || currentRequest == null) 
        { Debug.Log("만남이 없는데 어떻게 거래를 해"); return; }

        if (currentRequest.TradeType == TradeType.Buy && tradeSlot.Item == null)
        { Debug.Log("아무 아이템도 제시하지 않음"); return; }

        int offer = numeric.GetNumericValue();

        textPlate.text = "";
        //첫 거래라는 뜻
        if(current == null)
        {
            StartNewLine(tradeSlot.Item);

            Emit(DialogueEvent.OfferAsked, TradeResult.None, offer);
        }
        //첫 거래는 아닌데 기존 아이템이 아니라 가격 견적을 새로 내야함
        else if(current.Item != tradeSlot.Item)
        {
            RequoteAndRetarget(tradeSlot.Item, false);

            Emit(DialogueEvent.BrowseHint, TradeResult.None, offer);
        }

        Emit(DialogueEvent.OfferEvaluated, TradeResult.None, offer);

        var result = current.Haggle.EvaluateOffer(current.Request.TradeType, offer);

        switch (result)
        {
            case HaggleResult.Accept:
                TryCommit(offer);
                break;
            case HaggleResult.Counter:
                //ui
                Emit(DialogueEvent.DealMaintain, TradeResult.Maintenance, offer);
                Debug.Log("재흥정 ");
                break;
            case HaggleResult.Reject:
                if (current.Haggle.Attempt >= current.Haggle.MaxRound)
                    Emit(DialogueEvent.MaxRoundsReached, TradeResult.Failed, offer);
                else
                    Emit(DialogueEvent.DealFail, TradeResult.Failed, offer);
                EndSession(false, "흥정실패");
                break;
        }

    }

    private void StartNewLine(Item selected)
    {
        var quote = Quote(selected, currentVisitor, currentRequest);
        var haggle = new HaggleSession();
        haggle.Start(quotedPrice: quote.FinalPrice, spread: CalcSpread(quote.FinalPrice, currentVisitor), maxRound: currentVisitor.MaxRounds, concedePerRound: currentVisitor.ConcedePerRound);

        current = new(currentRequest, selected, currentVisitor, quote, haggle);
        OnCreateTradeSession?.Invoke(current);

        //ui
        Debug.Log("내가 준 물건 " + current.Item);
        foreach (var m in quote.Steps)
        {
            Debug.Log("적용된 모드 " + m.Name);
            Debug.Log("변동치 " + m.Value);
            Debug.Log("적용 후 " + m.SubtotalAfter);
        }
        Debug.Log("내가 준 물건의 기본 가격 " + quote.BasePrice);
        Debug.Log("내가 준 물건의 현재 가격 " + quote.FinalPrice);
        Debug.Log("스프레드 " + current.Haggle.Spread);
    }

    private void RequoteAndRetarget(Item selected, bool resetConcession)
    {
        var quote = Quote(selected, currentVisitor, currentRequest);

        current.Item = selected;
        current.PriceQuote = quote;
        current.Haggle.Retarget(quote.FinalPrice, CalcSpread(quote.FinalPrice, currentVisitor), resetConcession);

        Debug.Log($"아이템 변경-> {selected}");
        Debug.Log($"새 견적가 {quote.FinalPrice}, 새 스프레드 {current.Haggle.Spread}");
    }

    //3 커밋
    public void TryCommit(int acceptedPrice)
    {
        var r = current.Request;        //거래

        //사실 여기서 null체크는 의미가 없는게 모든 조건을 테스트 해보고 성공해서 오는 곳인데
        //내가 사기
        if (r.TradeType == TradeType.Sell && r is ItemTradeRequest itemTrade)
        {
            //내 돈만 확인하면 되는거임
            if (InventoryManager.Instance.PlayerChest.Gold < acceptedPrice)
            {
                Debug.Log("돈이 모자름");
                Emit(DialogueEvent.DealFail, TradeResult.Failed, acceptedPrice);
                return;
            }
            else if (InventoryManager.Instance.PlayerChest.InsertItem(itemTrade.TargetItem))
            {
                Debug.Log("저장공간이 부족함");
                Emit(DialogueEvent.DealFail, TradeResult.Failed, acceptedPrice);
                return;
            }
            Debug.Log("구입 성공");
            tradeService.CommitPurchase(acceptedPrice);
        }
        //내가 팔기
        else if (r.TradeType == TradeType.Buy && r is CategoryTradeRequest)
        {
            if (current.Item == null)
            {
                Emit(DialogueEvent.DealFail, TradeResult.Failed, acceptedPrice);
                Debug.Log("재시한 아이템이 없음");
                return;
            }
            Debug.Log("판매 성공");
            tradeService.CommitSale(current.Item, acceptedPrice);
            tradeSlot.Insert(null);
        }
        else
        {
            Debug.Log("ItemTrade는 Sell 인 경우 Category는 Buy인 경우에만 가능한데 그렇지 않았기에 취소");
            return;
        }

        //ui
        Emit(DialogueEvent.DealSuccess, TradeResult.Success, acceptedPrice);
        EndSession(true, "거래 성공");
    }

    private void EndSession(bool success, string reason = "")
    {
        //ui
        Emit(DialogueEvent.Goodbye, success ? TradeResult.Success : TradeResult.Failed, 0);
        Debug.Log(success + " " + reason);
        current = null;
        UIManager.Instance.TradeSlot.gameObject.SetActive(false);
    }

    private void Emit(DialogueEvent evt, TradeResult result, int offer)
    {
        if (currentRequest == null || currentVisitor == null) return;

        bool hasCurrent = current != null;

        var v = currentVisitor;
        var req = currentRequest;
        var it = hasCurrent ? current.Item : null;
        var dlg = v.DialoguePack;

        int generosity = hasCurrent ? Mathf.RoundToInt(v.Generosity * current.Haggle.BaseQuote) : 0;


        var ctx = new DialogueContext
        {
            visitor = v,
            tradeType = req.TradeType,
            item = it,
            offer = offer,
            basePrice = hasCurrent ? current.Haggle.BaseQuote : 0,
            spread = hasCurrent ? current.Haggle.Spread : 0,
            generosity = generosity,
            attempt = hasCurrent ? current.Haggle.Attempt : 0,
            maxRound = hasCurrent ? current.Haggle.MaxRound : 0,
            result = result,
            match = req.ComputeMatch(it)
        };

        string line = DialogueService.Pick(dlg, evt, ctx);
        if (!string.IsNullOrEmpty(line))
        {
            //ui에 띄우기
            textPlate.text += line + "\n";
            //Debug.Log(evt + line);
        }
    }

    private int CalcSpread(int baseQueto, Visitor visitor)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseQueto * visitor.Generosity));
    }
}


public sealed class TradeSession
{
    public readonly TradeRequest Request;
    public Item Item { get; set; }
    public readonly Visitor Visitor;
    public PriceQuote PriceQuote { get; set; }
    public readonly HaggleSession Haggle;
    public TradeSession(TradeRequest req, Item item, Visitor visitor, PriceQuote quote, HaggleSession haggle)
    {
        Request = req;
        Item = item;
        Visitor = visitor;
        PriceQuote = quote;
        Haggle = haggle;
    }
}