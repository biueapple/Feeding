using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputManagerEntry;

//공격에 추가 value 물리데미지가 생기는 장비효과 (치명타가 터짐)
[CreateAssetMenu(menuName = "Item/EquipmentEffect/DamageEffect")]
public class AddDamageEffect : EquipmentEffect
{
    [SerializeField]
    private DamageType type;
    [SerializeField]
    private float value;

    public override void CollectTokens(Dictionary<string, string> tokens)
    {
        tokens.Add("type", type.ToString());
        tokens.Add("value", value.ToString());
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
        if (args.Attacker == null)
            args.Damages.Add(new DamagePacket(type, this, value));
        else
            args.Damages.Add(new DamagePacket(type, this, args.IsCritical ? value * (1 + args.Attacker.StatValue(DerivationKind.CD)) : value));
    }
}
