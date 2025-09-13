using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using NUnit.Framework;

public sealed class ItemTradeRequest : TradeRequest
{
    public readonly Item TargetItem;
    public override string Summary => $"{TradeType}: {TargetItem.ItemName}";

    public ItemTradeRequest(TradeType type, Visitor visitor, IReadOnlyList<Item> list) : base(type, visitor)
    {
        TargetItem = PickExactItemFor(visitor, list);
    }

    public override int Margin(Item item)
    {
        if (item == TargetItem)
        {
            return Mathf.RoundToInt(item.Price * (Visitor.BaseMargin + Visitor.PreferMarginBonus));
        }
        return Mathf.RoundToInt(item.Price * (Visitor.BaseMargin + Visitor.DislikeMarginPenalty));
    }

    //아이템을 랜덤으로 선정할 때 사용
    public const float PreferredMul = 1.7f;
    public const float DislikedMul = 0.7f;
    public const float MinWeight = 0.01f;

    public float GetWeight(Visitor v, Item item)
    {
        float w = 1;
        if (v.Preferred.Contains(item.Category)) w *= PreferredMul;
        if (v.Disliked.Contains(item.Category)) w *= DislikedMul;
        if (w < MinWeight) w = MinWeight;
        return w;
    }

    //catelog 중에서 하나 선택해주기
    public T Pick<T>(IReadOnlyList<T> catelog, Func<T, float> weight)
    {
        if (catelog == null || catelog.Count == 0) return default;

        float total = 0;
        for (int i = 0; i < catelog.Count; i++)
        {
            total += MathF.Max(0, weight(catelog[i]));
        }

        if (total <= 0f)
        {
            return catelog[UnityEngine.Random.Range(0, catelog.Count)];
        }

        float r = UnityEngine.Random.value * total;
        float acc = 0f;
        for (int i = 0; i < catelog.Count; i++)
        {
            acc += Mathf.Max(0f, weight(catelog[i]));
            if (r <= acc) return catelog[i];
        }

        return catelog[catelog.Count - 1];
    }

    public bool TryPick<T>(IReadOnlyList<T> catelog, Func<T, float> weight, out T result)
    {
        result = Pick(catelog, weight);
        return !EqualityComparer<T>.Default.Equals(result, default);
    }

    //전체 아이템중에 선택하기 (필터/제외 리스트는 옵션)
    public Item PickExactItemFor(Visitor v, IReadOnlyList<Item> allItmes, Predicate<Item> filter = null, HashSet<Item> exclude = null)
    {
        var pool = new List<Item>(allItmes.Count);
        for (int i = 0; i < allItmes.Count; i++)
        {
            var it = allItmes[i];
            if (exclude != null && exclude.Contains(it)) continue;
            if (filter != null && !filter(it)) continue;
            pool.Add(it);
        }
        if (pool.Count == 0)
            return null;

        return Pick(pool, it => GetWeight(v, it));
    }
}

public sealed class CategoryTradeRequest : TradeRequest
{
    public readonly ItemCategory Category;
    public override string Summary => $"{TradeType}: {Category}";

    public CategoryTradeRequest(TradeType type, Visitor visitor) : base(type, visitor)
    {
        Category = PickExactItemFor(visitor);
    }

    public override int Margin(Item item)
    {
        if (item.Category == Category)
        {
            return Mathf.RoundToInt(item.Price * (Visitor.BaseMargin + Visitor.PreferMarginBonus));
        }
        return Mathf.RoundToInt(item.Price * (Visitor.BaseMargin + Visitor.DislikeMarginPenalty));
    }

    //아이템을 랜덤으로 선정할 때 사용
    public const float PreferredMul = 1.7f;
    public const float DislikedMul = 0.7f;
    public const float MinWeight = 0.01f;

    public float GetWeight(Visitor v, ItemCategory category)
    {
        float w = 1;
        if (v.Preferred.Contains(category)) w *= PreferredMul;
        if (v.Disliked.Contains(category)) w *= DislikedMul;
        if (w < MinWeight) w = MinWeight;
        return w;
    }

    //catelog 중에서 하나 선택해주기
    public T Pick<T>(IReadOnlyList<T> catelog, Func<T, float> weight)
    {
        if (catelog == null || catelog.Count == 0) return default;

        float total = 0;
        for (int i = 0; i < catelog.Count; i++)
        {
            total += MathF.Max(0, weight(catelog[i]));
        }

        if (total <= 0f)
        {
            return catelog[UnityEngine.Random.Range(0, catelog.Count)];
        }

        float r = UnityEngine.Random.value * total;
        float acc = 0f;
        for (int i = 0; i < catelog.Count; i++)
        {
            acc += Mathf.Max(0f, weight(catelog[i]));
            if (r <= acc) return catelog[i];
        }

        return catelog[catelog.Count - 1];
    }

    public bool TryPick<T>(IReadOnlyList<T> catelog, Func<T, float> weight, out T result)
    {
        result = Pick(catelog, weight);
        return !EqualityComparer<T>.Default.Equals(result, default);
    }

    //전체 아이템중에 선택하기 (필터/제외 리스트는 옵션)
    public ItemCategory PickExactItemFor(Visitor v, Predicate<ItemCategory> filter = null, HashSet<ItemCategory> exclude = null)
    {
        List<ItemCategory> pool = new ();
        int length = Enum.GetValues(typeof(ItemCategory)).Length;
        for (int i = 0; i < length; i++)
        {
            var it = (ItemCategory)i;
            if (exclude != null && exclude.Contains(it)) continue;
            if (filter != null && !filter(it)) continue;
            pool.Add(it);
        }
        if (pool.Count == 0)
            return ItemCategory.None;

        return Pick(pool, it => GetWeight(v, it));
    }
}