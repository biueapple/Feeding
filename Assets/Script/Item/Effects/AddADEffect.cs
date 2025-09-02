using UnityEngine;

//ad �� �����ϴ� ���ȿ��
[CreateAssetMenu(menuName = "Item/EquipmentEffect/DerivationEffect")]
public class AddDerivationEffect : EquipmentEffect
{
    [SerializeField]
    private DerivationKind kind;
    [SerializeField]
    private float value;

    public override void Apply(Unit target)
    {
        if (target == null) return;
        if (target.Derivaton.ContainsKey(kind)) return;
        target.AddStatModifier(new StatModifier(kind, value, EffectName), EffectID);
    }

    public override void Remove(Unit target)
    {
        if (target == null) return;
        if (target.Derivaton.ContainsKey(kind)) return;
        target.RemoveStatModifier(EffectID);
    }
}
