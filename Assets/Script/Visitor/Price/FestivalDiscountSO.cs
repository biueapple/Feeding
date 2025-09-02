using UnityEngine;

[CreateAssetMenu(menuName = "Pricing/Event/Fastival")]
public class FestivalDiscountSO : PriceModifierSO
{
    [SerializeField]
    private ItemCategory target = ItemCategory.Weapon;
    [SerializeField]
    private int flat = -50;

    public override (PriceOP op, float value) Evaluate(in PriceContext ctx)
    {
        return (PriceOP.FlatADD, flat);
    }

    public override bool IsApplicable(in PriceContext ctx)
    {
        return ctx.item.Category == target;
    }
}
