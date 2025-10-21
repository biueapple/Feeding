using UnityEngine;

//kind 가 value만큼 증가하는 효과
[CreateAssetMenu(menuName = "Item/EquipmentEffect/DerivationEffect")]
public class AddDerivationEffect : EquipmentEffect
{
    [SerializeField]
    private DerivationKind kind;
    [SerializeField]
    private float value;

    //public override string Description => $"{kind} 가 {value}만큼 증가합니다.";

    public override string BuildDescription(EquipmentEffect effect)
    {
        string s = base.BuildDescription(effect);
        s = s.Replace("{kind}", kind.ToString());
        s = s.Replace("{value}", value.ToString());
        return s;
    }

    public override void Apply(Unit target)
    {
        if (target == null) return;
        target.AddStatModifier(new StatModifier(kind, value, EffectName), EffectID);
    }

    public override void Remove(Unit target)
    {
        if (target == null) return;
        target.RemoveStatModifier(EffectID);
    }
}
