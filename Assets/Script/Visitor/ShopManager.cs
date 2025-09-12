using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField]
    private PriceModifierHub hub;
    [SerializeField]
    private TradeService tradeService;

    private PricingService pricing;
    private TradeSession current;


    private void Awake()
    {
        pricing = new PricingService(hub);
        tradeService = new();
    }

    public PriceQuote Quote(Item item, Visitor visitor, TradeRequest tradeRequest) => pricing.GetQuote(item, visitor, tradeRequest);


    private Visitor currentVisitor;
    private TradeRequest currentRequest;
    //만남
    public void StartEncounter(Visitor visitor, IReadOnlyList<Item> allItmes)
    {
        currentVisitor = visitor;
        currentRequest = new ItemTradeRequest(TradeType.Sell, visitor, allItmes);

        Debug.Log("상대가 원하는 물건 " + currentRequest.Summary);
        Emit(DialogueEvent.Arrive, TradeResult.None, 0);
        Emit(DialogueEvent.BrowseHint, TradeResult.None, 0);
    }

    //1 거래시작
    public void StartTrade(Item selected, int offer)
    {
        if (currentVisitor == null || currentRequest == null) 
        { Debug.Log("만남이 없는데 어떻게 거래를 해"); return; }

        if (selected == null)
        { Debug.Log("아무 아이템도 제시하지 않음"); return; }

        //첫 거래라는 뜻
        if(current == null)
        {
            StartNewLine(selected);

            Emit(DialogueEvent.OfferAsked, TradeResult.None, offer);
        }
        //첫 거래는 아닌데 기존 아이템이 아니라 가격 견적을 새로 내야함
        else if(current.Item != selected)
        {
            RequoteAndRetarget(selected, false);

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
    public Test test;
    public void TryCommit(int acceptedPrice)
    {
        var r = current.Request;
        var item = current.Item;

        //내가 사기
        if(r.TradeType == TradeType.Sell)
        {
            if(test.item != null && test.pay < acceptedPrice)
            {
                Debug.Log("인벤창이나 돈이 모자름");
                Emit(DialogueEvent.DealFail, TradeResult.Failed, acceptedPrice);
                return;
            }
            tradeService.CommitPurchase(test, item, acceptedPrice);
        }
        //내가 팔기
        else
        {
            if(test.item == null)
            {
                Emit(DialogueEvent.DealFail, TradeResult.Failed, acceptedPrice);
                Debug.Log("아이템이 없음");
                return;
            }
            tradeService.CommitSale(test, item, acceptedPrice);
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
            Debug.Log(evt + line);
        }
    }

    private int CalcSpread(int baseQueto, Visitor visitor)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseQueto * visitor.Generosity));
    }
}


public sealed class TradeSession
{
    public TradeRequest Request { get; }
    public Item Item { get; set; }
    public Visitor Visitor { get; }
    public PriceQuote PriceQuote { get; set; }
    public HaggleSession Haggle { get; }
    public TradeSession(TradeRequest req, Item item, Visitor visitor, PriceQuote quote, HaggleSession haggle)
    {
        Request = req;
        Item = item;
        Visitor = visitor;
        PriceQuote = quote;
        Haggle = haggle;
    }
}