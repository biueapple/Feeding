using System.Linq;
using UnityEngine;

public enum TradeType
{
    Buy,
    Sell,

}

public enum TradeResult
{
    None,
    Success,
    Failed,
    Maintenance,

}

//거래는 아이템이 결정되는 거래와 카테고리를 주고 아이템을 자유로이 선택하는 거래로 나눠진다.
public abstract class TradeRequest
{
    public readonly TradeType TradeType;
    public readonly VisitorSO Visitor;
    protected TradeRequest(TradeType tradeType, VisitorSO visitor) { TradeType = tradeType; Visitor = visitor; }
    public abstract string Summary { get; }

    public abstract int Margin(Item item);

    public virtual CategoryMatch ComputeMatch(Item item)
    {
        if (item == null) return CategoryMatch.Neutral;
        if (Visitor.Preferred.Contains(item.Category)) return CategoryMatch.Preferred;
        else if (Visitor.Disliked.Contains(item.Category)) return CategoryMatch.Disliked;
        return CategoryMatch.Neutral;
    }
}
