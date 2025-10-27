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

    //���߿��� �ٸ������� �ű���� �� �� ����
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

    //����
    public void StartEncounter(Visitor visitor)
    {
        currentVisitor = visitor;

        if (InventoryManager.Instance.PlayerChest.Count() < 10)
            currentRequest = new ItemTradeRequest(TradeType.Sell, visitor, itemCollector.Items);
        else
            currentRequest = UnityEngine.Random.value > 0.5f ? new ItemTradeRequest(TradeType.Sell, visitor, itemCollector.Items) : new CategoryTradeRequest(TradeType.Buy, visitor);
        
        if (currentRequest.TradeType == TradeType.Buy)
            UIManager.Instance.TradeSlot.gameObject.SetActive(true);

        Debug.Log("��밡 ���ϴ� �� " + currentRequest.Summary);
        Emit(DialogueEvent.Arrive, TradeResult.None, 0);
        Emit(DialogueEvent.BrowseHint, TradeResult.None, 0);
    }

    //1 �ŷ�����
    public void StartTrade()
    {
        if (currentVisitor == null || currentRequest == null) 
        { Debug.Log("������ ���µ� ��� �ŷ��� ��"); return; }

        if (currentRequest.TradeType == TradeType.Buy && tradeSlot.Item == null)
        { Debug.Log("�ƹ� �����۵� �������� ����"); return; }

        int offer = numeric.GetNumericValue();

        textPlate.text = "";
        //ù �ŷ���� ��
        if(current == null)
        {
            StartNewLine(tradeSlot.Item);

            Emit(DialogueEvent.OfferAsked, TradeResult.None, offer);
        }
        //ù �ŷ��� �ƴѵ� ���� �������� �ƴ϶� ���� ������ ���� ������
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
                Debug.Log("������ ");
                break;
            case HaggleResult.Reject:
                if (current.Haggle.Attempt >= current.Haggle.MaxRound)
                    Emit(DialogueEvent.MaxRoundsReached, TradeResult.Failed, offer);
                else
                    Emit(DialogueEvent.DealFail, TradeResult.Failed, offer);
                EndSession(false, "��������");
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
        Debug.Log("���� �� ���� " + current.Item);
        foreach (var m in quote.Steps)
        {
            Debug.Log("����� ��� " + m.Name);
            Debug.Log("����ġ " + m.Value);
            Debug.Log("���� �� " + m.SubtotalAfter);
        }
        Debug.Log("���� �� ������ �⺻ ���� " + quote.BasePrice);
        Debug.Log("���� �� ������ ���� ���� " + quote.FinalPrice);
        Debug.Log("�������� " + current.Haggle.Spread);
    }

    private void RequoteAndRetarget(Item selected, bool resetConcession)
    {
        var quote = Quote(selected, currentVisitor, currentRequest);

        current.Item = selected;
        current.PriceQuote = quote;
        current.Haggle.Retarget(quote.FinalPrice, CalcSpread(quote.FinalPrice, currentVisitor), resetConcession);

        Debug.Log($"������ ����-> {selected}");
        Debug.Log($"�� ������ {quote.FinalPrice}, �� �������� {current.Haggle.Spread}");
    }

    //3 Ŀ��
    public void TryCommit(int acceptedPrice)
    {
        var r = current.Request;        //�ŷ�

        //��� ���⼭ nullüũ�� �ǹ̰� ���°� ��� ������ �׽�Ʈ �غ��� �����ؼ� ���� ���ε�
        //���� ���
        if (r.TradeType == TradeType.Sell && r is ItemTradeRequest itemTrade)
        {
            //�� ���� Ȯ���ϸ� �Ǵ°���
            if (InventoryManager.Instance.PlayerChest.Gold < acceptedPrice)
            {
                Debug.Log("���� ���ڸ�");
                Emit(DialogueEvent.DealFail, TradeResult.Failed, acceptedPrice);
                return;
            }
            else if (InventoryManager.Instance.PlayerChest.InsertItem(itemTrade.TargetItem))
            {
                Debug.Log("��������� ������");
                Emit(DialogueEvent.DealFail, TradeResult.Failed, acceptedPrice);
                return;
            }
            Debug.Log("���� ����");
            tradeService.CommitPurchase(acceptedPrice);
        }
        //���� �ȱ�
        else if (r.TradeType == TradeType.Buy && r is CategoryTradeRequest)
        {
            if (current.Item == null)
            {
                Emit(DialogueEvent.DealFail, TradeResult.Failed, acceptedPrice);
                Debug.Log("����� �������� ����");
                return;
            }
            Debug.Log("�Ǹ� ����");
            tradeService.CommitSale(current.Item, acceptedPrice);
            tradeSlot.Insert(null);
        }
        else
        {
            Debug.Log("ItemTrade�� Sell �� ��� Category�� Buy�� ��쿡�� �����ѵ� �׷��� �ʾұ⿡ ���");
            return;
        }

        //ui
        Emit(DialogueEvent.DealSuccess, TradeResult.Success, acceptedPrice);
        EndSession(true, "�ŷ� ����");
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
            //ui�� ����
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