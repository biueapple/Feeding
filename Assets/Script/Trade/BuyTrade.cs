using System;
using UnityEngine;

//처음에 아이템이 정해져 있지 않고 그 이후에도 변할 수 있음
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
        Category = request.Category;
        Request = request;
        Haggle = new HaggleSession();
    }

    public override HaggleResult Trade(int offer)
    {
        Item item = OnGetItem?.Invoke();
        if (item == null) { Debug.Log("아이템이 안들어옴"); return HaggleResult.Reject; }

        //처음 거래
        if(Haggle.Attempt == 0)
        {
            Pricing(item);
        }
        //아이템이 달라짐
        else if(item != Item)
        {
            var quote = Pricing(item);
            Debug.Log($"아이템 변경-> {item}");
            Debug.Log($"새 견적가 {quote.FinalPrice}, 새 스프레드 {Haggle.Spread}");
        }

        Item = item;
        return Haggle.EvaluateOffer(TradeType, offer);
    }

    public override bool Commit(int offer)
    {
        Debug.Log("판매 성공");
        InventoryManager.Instance.TryEarnGold(offer);
        return true;
    }

    private PriceQuote Pricing(Item item)
    {
        var quote = pricingService.GetQuote(item, Visitor, request, TradeType);
        Haggle.Start(quote.FinalPrice, CalcSpread(quote.BasePrice, Visitor), Visitor.MaxRounds, Visitor.ConcedePerRound);

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

        return quote;
    }
}
