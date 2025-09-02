using UnityEngine;

public enum HaggleResult
{
    Accept,
    Counter,
    Reject,

}

public class HaggleSession
{
    public int BaseQuote { get; private set; }
    public int Spread { get; private set; }
    public int MaxRound { get; private set; }
    public float ConcedePerRound { get; private set; }
    public int Attempt { get; private set; }

    public void Start(int quotedPrice, int spread, int maxRound, float concedePerRound)
    {
        BaseQuote = quotedPrice;
        Spread = spread;
        MaxRound = maxRound;
        ConcedePerRound = concedePerRound;
        Attempt = 0;
    }

    public (HaggleResult result, int counterPrice) EvaluateOffer(TradeType type, int offer)
    {
        Attempt++;
        float concedeFactor = 1 - Mathf.Clamp01(ConcedePerRound * (Attempt - 1));
        int target = BaseQuote;

        int threshold = Mathf.RoundToInt(Spread * concedeFactor);
        
        if(type == TradeType.Sell)
        {
            if (offer >= target - threshold) return (HaggleResult.Accept, offer);
            return Attempt >= MaxRound ?
                (HaggleResult.Reject, 0) :
                (HaggleResult.Counter, Mathf.Max(offer, target - threshold));
        }
        else
        {
            if (offer <= target + threshold) return (HaggleResult.Accept, offer);
            return Attempt >= MaxRound ?
                (HaggleResult.Reject, 0) :
                (HaggleResult.Counter, Mathf.Min(offer, target + threshold));
        }
    }
}
