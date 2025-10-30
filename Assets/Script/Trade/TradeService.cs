using UnityEngine;

public abstract class TradeService 
{
    protected readonly PricingService pricingService;
    public VisitorSO Visitor { get; protected set; }
    public TradeRequest Request { get; protected set; }
    public HaggleSession Haggle { get; protected set; }
    public Item Item { get; protected set; }
    public abstract TradeType TradeType { get; }

    public TradeService(PriceModifierHub priceModifierHub)
    {
        pricingService = new(priceModifierHub);
    }

    public abstract void Encounter(VisitorSO visitor);
    public abstract HaggleResult Trade(int offer);
    public abstract bool Commit(int offer);

    protected int CalcSpread(int baseQueto, VisitorSO visitorSO)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseQueto * visitorSO.Generosity));
    }
}
