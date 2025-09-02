using UnityEngine;

//공격에 추가 10 물리데미지가 생기는 장비효과
[CreateAssetMenu(menuName = "Item/EquipmentEffect/DamageEffect")]
public class AddDamageEffect : EquipmentEffect
{
    [SerializeField]
    private DamageType type;
    [SerializeField]
    private float value;

    public override void Apply(Unit target)
    {
        if (target == null) return;
        target.OnBeforeAttack += Target_OnBeforeAttack;
    }

    public override void Remove(Unit target)
    {
        if (target == null) return;
        target.OnBeforeAttack -= Target_OnBeforeAttack;
    }

    private void Target_OnBeforeAttack(AttackEventArgs args)
    {
        args.Damages.Add(new DamagePacket(type, $"{name}", value));
    }
}
