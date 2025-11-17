using System.Collections.Generic;
using UnityEngine;

//kind 가 value만큼 증가하는 효과
[CreateAssetMenu(menuName = "Item/EquipmentEffect/DerivationEffect")]
public class AddDerivationEffect : EquipmentEffect
{
    [SerializeField]
    private DerivationKind kind;
    [SerializeField]
    private float value;

    public override void CollectTokens(Dictionary<string, string> tokens)
    {
        tokens.Add("kind", kind.ToString());
        tokens.Add("value", value.ToString());
    }

    public override void Apply(Unit target)
    {
        if (target == null) return;
        target.AddStatModifier(new StatModifier(kind, value, "AddDerivationEffect"), EffectID);
    }

    public override void Remove(Unit target)
    {
        if (target == null) return;
        target.RemoveStatModifier(EffectID);
    }

}
