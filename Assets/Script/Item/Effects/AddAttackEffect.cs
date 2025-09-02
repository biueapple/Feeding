using System.Collections.Generic;
using UnityEngine;

//공격시 추가 10 물리데미지의 공격을 하는 장비효과
[CreateAssetMenu(menuName = "Item/EquipmentEffect/AttackEffect")]
public class AddAttackEffect : EquipmentEffect
{
    [SerializeField]
    private DamageType type;
    [SerializeField]
    private float value;

    public override void Apply(Unit target)
    {
        if (target == null) return;
        target.OnAfterAttack += Target_OnAfterAttack;
    }

    public override void Remove(Unit target)
    {
        if (target == null) return;
        target.OnAfterAttack -= Target_OnAfterAttack;
    }

    private void Target_OnAfterAttack(AttackEventArgs args)
    {
        if(!args.IsExtraAttack)
        {
            args.Attacker.Attack(args.Defender, new List<DamagePacket>() { new(type, $"{name}", value) }, true);
        }
    }
}
