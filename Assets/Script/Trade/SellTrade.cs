using UnityEngine;

//ó������ �������� ������ �ְ� �ٲ��� ����
public class SellTrade : TradeService
{
    private ItemTradeRequest request;
    public SellTrade(PriceModifierHub priceModifierHub) : base(priceModifierHub)
    {
    }

    public override TradeType TradeType => TradeType.Sell;

    public override void Encounter(VisitorSO visitor)
    {
        Visitor = visitor;
        request = new ItemTradeRequest(visitor);
        Item = request.TargetItem;
        Request = request;
        Haggle = new HaggleSession();
        var quote = pricingService.GetQuote(Item, visitor, request, TradeType);
        Haggle.Start(quote.FinalPrice, CalcSpread(quote.BasePrice, visitor), visitor.MaxRounds, visitor.ConcedePerRound);

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
    }

    public override HaggleResult Trade(int offer)
    {
        return Haggle.EvaluateOffer(TradeType, offer);
    }

    public override bool Commit(int offer)
    {
        //�� ���� Ȯ���ϸ� �Ǵ°���
        if (InventoryManager.Instance.Gold < offer)
        {
            Debug.Log("���� ���ڸ�");
            return false;
        }
        else if (!InventoryManager.Instance.PlayerChest.InsertItem(request.TargetItem))
        {
            Debug.Log("��������� ������");
            return false;
        }
        Debug.Log("���� ����");
        InventoryManager.Instance.TryEarnGold(-offer);
        return true;
    }
}
