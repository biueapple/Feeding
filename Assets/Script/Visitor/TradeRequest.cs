using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public enum TradeType
{
    Buy,
    Sell,

}

public enum TradeResult
{
    Success,
    Failed,
    Maintenance,

}

//거래는 아이템이 결정되는 거래와 카테고리를 주고 아이템을 자유로이 선택하는 거래로 나눠진다.
public abstract class TradeRequest
{
    public readonly TradeType TradeType;
    public readonly Visitor Visitor;
    protected TradeRequest(TradeType tradeType, Visitor visitor) { TradeType = tradeType; Visitor = visitor; }
    public abstract string Summary { get; }

    //public TradeResult Accept(Haggle haggle)
    //{
    //    if (Attempt >= Visitor.MaxRounds)
    //    {
    //        Emit(DialogueEvent.MaxRoundsReached, haggle, TradeResult.Failed);
    //        return TradeResult.Failed;
    //    }

    //    basePrice = TradeManager.Instance.Modifiers(this, haggle.Item);
    //    spread = Mathf.Max(1, Margin(haggle.Item));
    //    spread += Mathf.RoundToInt(spread * Visitor.ConcedePerRound * Attempt);
    //    generosity = Mathf.RoundToInt(Mathf.Max(1, basePrice * Visitor.Generosity));


    //    TradeResult result;
    //    if (TradeType == TradeType.Buy)
    //    {
    //        //실패
    //        if(haggle.Pay > basePrice + spread + generosity)
    //        {
    //            result = TradeResult.Failed;
    //        }
    //        //유지
    //        else if(haggle.Pay > basePrice + spread)
    //        {
    //                result = TradeResult.Maintenance;
    //        }
    //        //성공
    //        else
    //        {
    //            result = TradeResult.Success;
    //        }
    //    }
    //    else
    //    {
    //        result = TradeResult.Success;
    //    }

    //    Emit(DialogueEvent.OfferEvaluated, haggle, result);

    //    Attempt++;

    //    if(result == TradeResult.Success)
    //    {
    //        Emit(DialogueEvent.DealSuccess, haggle, result);
    //    }
    //    else if(result == TradeResult.Maintenance)
    //    {
    //        Emit(DialogueEvent.DealMaintain, haggle, result);
    //    }
    //    else
    //    {
    //        if (Attempt >= Visitor.MaxRounds) Emit(DialogueEvent.MaxRoundsReached, haggle, result);
    //        else Emit(DialogueEvent.DealFail, haggle, result);
    //    }
    //    return result;
    //}

    public abstract int Margin(Item item);

    //protected virtual CategoryMatch ComputeMatch(Item item)
    //{
    //    if (Visitor.Preferred.Contains(item.Category)) return CategoryMatch.Preferred;
    //    else if (Visitor.Disliked.Contains(item.Category)) return CategoryMatch.Disliked;
    //    return CategoryMatch.Neutral;
    //}

    //private void Emit(DialogueEvent evt, Haggle haggle, TradeResult result)
    //{
    //    var dialogue = Visitor.DialoguePack;
    //    var ctx = new DialogueContext
    //    {
    //        visitor = Visitor,
    //        tradeType = TradeType,
    //        item = haggle.Item,
    //        offer = haggle.Pay,
    //        basePrice = basePrice,
    //        spread = spread,
    //        generosity = generosity,
    //        attempt = Attempt,
    //        maxRound = Visitor.MaxRounds,
    //        result = result,
    //        match = ComputeMatch(haggle.Item)
    //    };

    //    string line = DialogueService.Pick(dialogue, evt, ctx);
    //    if(!string.IsNullOrEmpty(line))
    //    {
    //        //ui에 띄우기
    //    }
    //}
}
