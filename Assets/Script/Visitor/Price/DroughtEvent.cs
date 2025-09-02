using UnityEngine;


[CreateAssetMenu(menuName = "Pricing/Event/Drought")]
public class DroughtEvent : PriceModifierSO
{
    [SerializeField]
    private ItemCategory target = ItemCategory.Grain;
    [SerializeField]
    private float percentIncrease = 0.20f;


    public override (PriceOP op, float value) Evaluate(in PriceContext ctx)
    {
        return (PriceOP.PercentADD, percentIncrease);
    }

    public override bool IsApplicable(in PriceContext ctx)
    {
        return ctx.item.Category == target;
    }
}