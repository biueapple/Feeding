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
    //����
    public void StartEncounter(Visitor visitor, IReadOnlyList<Item> allItmes)
    {
        currentVisitor = visitor;
        currentRequest = new ItemTradeRequest(TradeType.Sell, visitor, allItmes);

        Debug.Log("��밡 ���ϴ� ���� " + currentRequest.Summary);
        Emit(DialogueEvent.Arrive, TradeResult.None, 0);
        Emit(DialogueEvent.BrowseHint, TradeResult.None, 0);
    }

    //1 �ŷ�����
    public void StartTrade(Item selected, int offer)
    {
        if (currentVisitor == null || currentRequest == null) 
        { Debug.Log("������ ���µ� ��� �ŷ��� ��"); return; }

        if (selected == null)
        { Debug.Log("�ƹ� �����۵� �������� ����"); return; }

        //ù �ŷ���� ��
        if(current == null)
        {
            StartNewLine(selected);

            Emit(DialogueEvent.OfferAsked, TradeResult.None, offer);
        }
        //ù �ŷ��� �ƴѵ� ���� �������� �ƴ϶� ���� ������ ���� ������
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
    public Test test;
    public void TryCommit(int acceptedPrice)
    {
        var r = current.Request;
        var item = current.Item;

        //���� ���
        if(r.TradeType == TradeType.Sell)
        {
            if(test.item != null && test.pay < acceptedPrice)
            {
                Debug.Log("�κ�â�̳� ���� ���ڸ�");
                Emit(DialogueEvent.DealFail, TradeResult.Failed, acceptedPrice);
                return;
            }
            tradeService.CommitPurchase(test, item, acceptedPrice);
        }
        //���� �ȱ�
        else
        {
            if(test.item == null)
            {
                Emit(DialogueEvent.DealFail, TradeResult.Failed, acceptedPrice);
                Debug.Log("�������� ����");
                return;
            }
            tradeService.CommitSale(test, item, acceptedPrice);
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