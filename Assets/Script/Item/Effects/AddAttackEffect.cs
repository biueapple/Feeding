using System.Collections.Generic;
using UnityEngine;

//���ݽ� �߰� value type�������� ������ �ϴ� ���ȿ��
[CreateAssetMenu(menuName = "Item/EquipmentEffect/AttackEffect")]
public class AddAttackEffect : EquipmentEffect
{
    [SerializeField]
    private DamageType type;
    [SerializeField]
    private float value;

    //public override string Description => $"���ݸ��� �߰��� {value} {type} �������� �ݴϴ�.";

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
        target.OnAttackAfter += Target_OnAfterAttack;
    }

    public override void Remove(Unit target)
    {
        if (target == null) return;
        target.OnAttackAfter -= Target_OnAfterAttack;
    }

    private void Target_OnAfterAttack(AttackEventArgs args)
    {
        if(!args.IsExtraAttack && args.Attacker != null)
        {
            AttackEventArgs extra = new(args.Attacker, args.Defender, true);
            extra.Damages.Add(new(type, $"{name}", value));
            args.Attacker.PerformAttack(extra);
        }
    }
}
