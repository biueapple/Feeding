using System;
using System.Collections.Generic;
using UnityEngine;

//걸때마다 du가 1씩 증가하도록
//한번에 여러번 걸어야 du가 길어저서 점점 중첩하기 쉽고 쌔지도록
//근데 공격마다 출혈의 du가 줄어드니 쉽지 않을듯
[CreateAssetMenu(menuName = "RPG/Debuff/DotType_Bleeding")]
public class Bleeding : Dot
{
    public override string BuffID => "Bleeding";
    public override float Duration => 1;    //출혈은 항상 지속시간 1

    public override void Apply(object caster, BuffAdministrator target, BuffInstance inst)
    {
        if (target.Owner == null)
            return;

        void action(AttackEventArgs args)
        {
            float damage = inst.Stacks;
            Debug.Log($"bleeding으로 인한 피해 {damage}");

            AttackEventArgs a = new(caster as Unit, target.Owner, false);
            a.Damages.Add(new DamagePacket(type, this, damage));
            target.Owner.TakeDamage(a);

            if (inst.Tick(1))
            {
                Debug.Log("출혈 끝남");
                target.RemoveBuff(inst);
            }
        }

        target.SubscribeOnAfterAttack(inst, action);
    }

    public override void Reapply(object caster, BuffAdministrator target, List<BuffInstance> list)
    {
        if (target == null || list == null) return;

        //사실 list에 여러개가 있을리가 없음
        foreach(var inst in list)
        {
            inst.Duration++;
            inst.AddStack(stack);
        }
    }
}
