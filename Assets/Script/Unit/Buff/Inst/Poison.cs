using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

//걸릴때마다 스택이 1씩 증가하는 형태
//du는 어쩌지 더 높은쪽으로
//치유감소가 있는걸로
[CreateAssetMenu(menuName = "RPG/Debuff/DotType_Poison")]
public class Poison : Dot
{
    public override string BuffID => "Poison";
    public override int Stack => 1;             //항상 스택이 1

    public override string BuildDescription(BuffInstance inst)
    {
        string s = base.BuildDescription(inst);
        s = s.Replace("{reduction}", (inst.Stacks * 0.01f).ToString());
        return s;
    }

    public override void Apply(object caster, BuffAdministrator target, BuffInstance inst)
    {
        if (target.Owner == null)
            return;

        void action()
        {
            float damage = inst.Stacks;
            Debug.Log($"poison으로 인한 피해 {damage}");
            AttackEventArgs a = new(caster as Unit, target.Owner, false);
            a.Damages.Add(new DamagePacket(type, "Poison", damage));
            target.Owner.TakeDamage(a);

            if (inst.Tick(1))
            {
                Debug.Log("poison 끝남");
                target.RemoveBuff(inst);
            }
        }

        void reduction(RecoveryEventArgs args)
        {
            foreach (var pack in args.Recovery)
            {
                //최대 치감 수치는 30퍼
                float stack = Mathf.Min(inst.Stacks, 30);
                pack.Value -= pack.Value * stack * 0.01f;
            }
        }

        //치유감소
        target.SubscribeOnRecoveryBefore(inst, reduction);
        //지속시간 계산
        target.SubscribeOnSecond(inst, action);
    }

    public override void Reapply(object caster, BuffAdministrator target, List<BuffInstance> list)
    {
        if (target == null || list == null) return;

        //사실 list에 여러개가 있을리가 없음
        foreach(var inst in list)
        {
            inst.AddStack();
            if (inst.Duration < Duration) inst.Duration = Duration;
        }
    }
}
