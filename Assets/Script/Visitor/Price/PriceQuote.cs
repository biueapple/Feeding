using System.Collections.Generic;
using UnityEngine;

public sealed class PriceQuote
{
    public int BasePrice { get; }
    public int FinalPrice { get; }
    public IReadOnlyList<PriceStep> Steps { get; }
    public PriceQuote(int basePrice, int finalPrice, List<PriceStep> steps)
    {
        BasePrice = basePrice;
        FinalPrice = finalPrice;
        Steps = steps;
    }
}


public sealed class PriceStep
{
    public string Name { get; }
    public PriceOP PriceOP { get; }
    public float Value { get; }
    public float SubtotalAfter { get; }
    public PriceStep(string name, PriceOP op, float value, float sub)
    {
        Name = name;
        PriceOP = op;
        Value = value;
        SubtotalAfter = sub;
    }
}