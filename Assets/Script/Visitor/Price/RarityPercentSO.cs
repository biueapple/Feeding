using UnityEngine;

[CreateAssetMenu(menuName = "Pricing/Event/RarityPercent")]
public class RarityPercentSO : PriceModifierSO
{
    public override (PriceOP op, float value) Evaluate(in PriceContext ctx)
    {
        float price = 0;
        switch(ctx.item.Rarity)
        {
            case ItemRarity.COMMON:
                price = 0;
                break;
            case ItemRarity.UNCOMMON:
                price = 0.1f;
                break;
            case ItemRarity.RARE:
                price = 0.2f;
                break;
            case ItemRarity.EPIC:
                price = 0.3f;
                break;
            case ItemRarity.LEGENDARY:
                price *= 0.4f;
                break;
        }
        return (PriceOP.PercentADD, price);
    }

    public override bool IsApplicable(in PriceContext ctx)
    {
        return ctx.item != null;
    }
}
