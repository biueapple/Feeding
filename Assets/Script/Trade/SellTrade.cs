using UnityEngine;

//처음부터 아이템이 정해져 있고 바뀌지 않음
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

        Debug.Log("물건 " + Item);
        foreach (var m in quote.Steps)
        {
            Debug.Log("적용된 모드 " + m.Name);
            Debug.Log("변동치 " + m.Value);
            Debug.Log("적용 후 " + m.SubtotalAfter);
        }
        Debug.Log("물건의 기본 가격 " + quote.BasePrice);
        Debug.Log("물건의 현재 가격 " + quote.FinalPrice);
        Debug.Log("스프레드 " + Haggle.Spread);
    }

    public override HaggleResult Trade(int offer)
    {
        return Haggle.EvaluateOffer(TradeType, offer);
    }

    public override bool Commit(int offer)
    {
        //내 돈만 확인하면 되는거임
        if (InventoryManager.Instance.Gold < offer)
        {
            Debug.Log("돈이 모자름");
            return false;
        }
        else if (!InventoryManager.Instance.PlayerChest.InsertItem(request.TargetItem))
        {
            Debug.Log("저장공간이 부족함");
            return false;
        }
        Debug.Log("구입 성공");
        InventoryManager.Instance.TryEarnGold(-offer);
        return true;
    }
}
