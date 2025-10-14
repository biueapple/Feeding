using UnityEngine;

//공격에 추가 value 물리데미지가 생기는 장비효과 (치명타가 터짐)
[CreateAssetMenu(menuName = "Item/EquipmentEffect/DamageEffect")]
public class AddDamageEffect : EquipmentEffect
{
    [SerializeField]
    private DamageType type;
    [SerializeField]
    private float value;

    public override string Description => $"공격을 할때마다 추가로 {value}데미지의 {type} 공격을 합니다.";

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
