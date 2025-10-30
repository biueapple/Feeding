using System;
using UnityEngine;

//ó���� �������� ������ ���� �ʰ� �� ���Ŀ��� ���� �� ����
public class BuyTrade : TradeService
{
    private CategoryTradeRequest request;
    private Func<Item> OnGetItem;

    public BuyTrade(PriceModifierHub priceModifierHub, Func<Item> get) : base(priceModifierHub)
    {
        OnGetItem = get;
    }

    public override TradeType TradeType => TradeType.Buy;

    public override void Encounter(VisitorSO visitor)
    {
        Visitor = visitor;
        request = new CategoryTradeRequest(visitor);
        Request = request;
        Haggle = new HaggleSession();
    }

    public override HaggleResult Trade(int offer)
    {
        Item item = OnGetItem?.Invoke();
        if (item == null) { Debug.Log("�������� �ȵ���"); return HaggleResult.Reject; }

        //ó�� �ŷ�
        if(Haggle.Attempt == 0)
        {
            Pricing(item);
        }
        //�������� �޶���
        else if(item != Item)
        {
            var quote = Pricing(item);
            Debug.Log($"������ ����-> {item}");
            Debug.Log($"�� ������ {quote.FinalPrice}, �� �������� {Haggle.Spread}");
        }

        Item = item;
        return Haggle.EvaluateOffer(TradeType, offer);
    }

    public override bool Commit(int offer)
    {
        Debug.Log("�Ǹ� ����");
        InventoryManager.Instance.TryEarnGold(offer);
        return true;
    }

    private PriceQuote Pricing(Item item)
    {
        var quote = pricingService.GetQuote(item, Visitor, request, TradeType);
        Haggle.Start(quote.FinalPrice, CalcSpread(quote.BasePrice, Visitor), Visitor.MaxRounds, Visitor.ConcedePerRound);

        Debug.Log("���� " + Item);
        foreach (var m in quote.Steps)
        {
            Debug.Log("����� ��� " + m.Name);
            Debug.Log("����ġ " + m.Value);
            Debug.Log("���� �� " + m.SubtotalAfter);
        }
        Debug.Log("������ �⺻ ���� " + quote.BasePrice);
        Debug.Log("������ ���� ���� " + quote.FinalPrice);
        Debug.Log("�������� " + Haggle.Spread);

        return quote;
    }
}
