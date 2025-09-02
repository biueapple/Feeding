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

    public PriceQuote Quote(Item item, Visitor visitor, TradeType tradeType) => pricing.GetQuote(item, visitor, tradeType);

    //1 �ŷ�����
    public void StartTrade(TradeRequest req, Item selected, Visitor visitor, HaggleSession haggle)
    {
        var margin = req.Margin(selected);
        var quote = Quote(selected, visitor, req.TradeType);
        haggle.Start(quotedPrice: quote.FinalPrice, spread: CalcSpread(selected, visitor, margin), maxRound: visitor.MaxRounds, concedePerRound: visitor.ConcedePerRound);

        current = new (req, selected, visitor, quote, haggle);

        //ui
        Debug.Log("���� �� ���� " + current.Item);
        Debug.Log("��밡 ���ϴ� ���� " + current.Request.Summary);
        foreach (var m in quote.Steps)
        {
            Debug.Log("����� ��� " + m.Name);
            Debug.Log("����ġ " + m.Value);
            Debug.Log("���� �� " + m.SubtotalAfter);
        }
        Debug.Log("���� �� ������ �⺻ ���� " + quote.BasePrice);
        Debug.Log("���� �� ������ ���� ���� " + quote.FinalPrice);
    }

    //2 ���� �Է�
    public void OnPlyaerOffer(int offer)
    {
        if (current == null) return;

        var (result, counter) = current.Haggle.EvaluateOffer(current.Request.TradeType, offer);

        switch(result)
        {
            case HaggleResult.Accept:
                TryCommit(offer); 
                break;
            case HaggleResult.Counter:
                //ui
                Debug.Log("���� " + counter);
                break;
            case HaggleResult.Reject:
                EndSession(false, "��������");
                break;
        }
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
                return;
            }
            tradeService.CommitPurchase(test, item, acceptedPrice);
        }
        //���� �ȱ�
        else
        {
            if(test.item == null)
            {
                Debug.Log("�������� ����");
                return;
            }
            tradeService.CommitSale(test, item, acceptedPrice);
        }

        //ui
        EndSession(true);
    }

    private void EndSession(bool success, string reason = "")
    {
        //ui
        Debug.Log(success + " " + reason);
        current = null;
    }

    private int CalcSpread(Item item, Visitor visitor, int margin)
    {
        return Mathf.Max(1, Mathf.RoundToInt(item.Price + margin + visitor.Generosity));
    }
}


public sealed class TradeSession
{
    public readonly TradeRequest Request;
    public readonly Item Item;
    public readonly Visitor Visitor;
    public readonly PriceQuote PriceQuote;
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