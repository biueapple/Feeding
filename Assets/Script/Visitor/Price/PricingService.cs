using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class PricingService
{
    private readonly PriceModifierHub hub;

    public PricingService(PriceModifierHub hub)
    {
        this.hub = hub;
    }

    public PriceQuote GetQuote(Item item, VisitorSO visitor, TradeRequest request, TradeType tradeType)
    {
        var ctx = new PriceContext(item, visitor, tradeType);
        float price = item.Price;
        int margin = request.Margin(item);
        var steps = new List<PriceStep>();

        //모디파이어 수집
        //우선순위 정렬, 중복제거 (우선순위 없음)
        var modifiers = hub.GetApplicable(ctx);
        var applied = new HashSet<string>();

        //적용규칙 perADD -> flatADD -> perMul
        float pendingPercentAdd = 0;
        float pendingFlatAdd = 0;

        void FlushBucket()
        {
            if (Math.Abs(pendingPercentAdd) > 0.0001f)
                price *= (1 + pendingPercentAdd);
            if (Math.Abs(pendingFlatAdd) > 0.0001f)
                price += pendingFlatAdd;

            pendingPercentAdd = 0;
            pendingFlatAdd = 0;
        }

        foreach(var m in modifiers)
        {
            if (!m.IsApplicable(ctx)) continue;
            if (!string.IsNullOrEmpty(m.StackingKey) && applied.Contains(m.StackingKey)) continue;

            var (op, val) = m.Evaluate(ctx);
            switch(op)
            {
                case PriceOP.PercentADD:
                    pendingPercentAdd += val;
                    steps.Add(new PriceStep(m.DisplayName, op, val, price * (1 + pendingPercentAdd)));
                    break;
                case PriceOP.FlatADD:
                    pendingFlatAdd += val;
                    steps.Add(new PriceStep(m.DisplayName, op, val, price * (1 + pendingPercentAdd) + pendingFlatAdd));
                    break;
                case PriceOP.PercentMul:
                    FlushBucket();
                    price *= val;
                    steps.Add(new PriceStep(m.DisplayName, op, val, price));
                    break;
            }
        }

        if (WorldContext.Instance.CurrentVillage.Import.Contains(item))
            price *= 1.5f;
        FlushBucket();

        //글로벌 규칙
        price = Mathf.Clamp(price, 1, 999_999);
        int final = Mathf.RoundToInt(price);
        steps.Add(new PriceStep("Rounding & Clamp", PriceOP.FlatADD, 0, final));
        //상대가 사러 왔을때만 마진을 추가
        if(tradeType == TradeType.Buy)
        {
            final += margin;
            steps.Add(new PriceStep("Margin", PriceOP.PercentADD, margin, final));
        }

        return new PriceQuote(item.Price, final, steps);
    }
}
