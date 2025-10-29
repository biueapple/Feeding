using UnityEngine;

public enum PriceOP
{
    FlatADD,
    PercentADD,
    PercentMul,

}


public interface IPriceModifier
{
    string DisplayName { get; }
    string StackingKey { get; }
    bool IsApplicable(in PriceContext ctx);
    (PriceOP op, float value) Evaluate(in PriceContext ctx);
}

public class PriceContext
{
    public readonly Item item;
    public readonly VisitorSO visitor;
    //public readonly Shop
    public readonly TradeType type;

    public PriceContext(Item item, VisitorSO visitor, TradeType type)
    {
        this.item = item;
        this.visitor = visitor;
        this.type = type;
    }
}