using UnityEngine;

//���ݿ� �߰� value ������������ ����� ���ȿ�� (ġ��Ÿ�� ����)
[CreateAssetMenu(menuName = "Item/EquipmentEffect/DamageEffect")]
public class AddDamageEffect : EquipmentEffect
{
    [SerializeField]
    private DamageType type;
    [SerializeField]
    private float value;

    //public override string Description => $"������ �Ҷ����� �߰��� {value}�������� {type} ������ �մϴ�.";

    public override string BuildDescription(EquipmentEffect effect)
    {
        string s = base.BuildDescription(effect);
        s = s.Replace("{type}", type.ToString());
        s = s.Replace("{value}", value.ToString());
        return s;
    }

    public override void Apply(Unit target)
    {
        if (target == null) return;
        target.OnAttackBefore += Target_OnBeforeAttack;
    }

    public override void Remove(Unit target)
    {
        if (target == null) return;
        target.OnAttackBefore -= Target_OnBeforeAttack;
    }

    private void Target_OnBeforeAttack(AttackEventArgs args)
    {
        args.Damages.Add(new DamagePacket(type, $"{name}", args.IsCritical ? value * (1 + args.Attacker.StatValue(DerivationKind.CD)) : value));
    }
}
