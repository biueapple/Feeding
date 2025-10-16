using System.Collections.Generic;
using UnityEngine;

//감전은 reapply하면 기존 대미지를 전부 한번에 받고 다시 걸리는걸로
[CreateAssetMenu(menuName = "RPG/Debuff/DotType_Electrocute")]
public class Electrocute : Dot
{
    public override string BuffID => "Electrocute";

    public override void Apply(Unit caster, BuffAdministrator target, BuffInstance inst)
    {
        if (target.Owner == null)
            return;

        void action()
        {
            float damage = inst.Stacks;
            Debug.Log($"Electrocute 인한 피해 {damage}");

            AttackEventArgs a = new(caster, target.Owner, false);
            a.Damages.Add(new DamagePacket(type, "Electrocute", damage));
            target.Owner.TakeDamage(a);

            if (inst.Tick(1))
            {
                Debug.Log("Electrocute 끝남");
                target.RemoveBuff(inst);
            }
        }

        target.SubscribeOnSecond(inst, action);
    }

    public override void Reapply(Unit caster, BuffAdministrator target, List<BuffInstance> list)
    {
        if (target == null || list == null) return;

        //사실 list에 여러개가 있을리가 없음
        foreach (var inst in list)
        {
            float damage = inst.Stacks * inst.Duration;
            AttackEventArgs a = new(caster, target.Owner, false);
            a.Damages.Add(new DamagePacket(type, "Electrocute", damage));
            target.Owner.TakeDamage(a);
            target.RemoveBuff(inst);
        }

        //Reapply를 호출할때는 CreateInstance 하지 않고 호출하기에 직접 만들어서 넣어주기
        BuffInstance instance = CreateInstance(target);
        target.AddInstance(caster, instance);
    }
}
