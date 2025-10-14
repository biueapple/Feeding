using System;
using UnityEngine;

//걸때마다 du가 1씩 증가하도록
//한번에 여러번 걸어야 du가 길어저서 점점 중첩하기 쉽고 쌔지도록
//근데 공격마다 출혈의 du가 줄어드니 쉽지 않을듯
[CreateAssetMenu(menuName = "RPG/Debuff/DotType_Bleeding")]
public class Bleeding : Dot
{
    public override string BuffID => "Bleeding";
    public override float Duration => 1;    //출혈은 항상 지속시간 1

    public override void Apply(BuffAdministrator administrator, BuffInstance inst)
    {
        if (administrator.Owner == null)
            return;

        void action(AttackEventArgs args)
        {
            Debug.Log($"bleeding으로 인한 피해 {inst.Stacks}");
            administrator.Owner.CurrentHP -= inst.Stacks;
            if (inst.Tick(1))
            {
                Debug.Log("출혈 끝남");
                administrator.RemoveBuff(this);
            }
        }

        administrator.SubscribeOnAfterAttack(inst, action);
    }

    public override void Reapply(BuffAdministrator administrator, BuffInstance inst)
    {
        inst.Duration++;
        inst.AddStack(stack);
    }

    public override void Remove(BuffAdministrator administrator, BuffInstance inst)
    {
        if (administrator == null) return;

    }

    public override BuffInstance CreateInstance(BuffAdministrator administrator)
    {
        BuffInstance inst = new(this, administrator)
        {
            Duration = Duration
        };
        inst.AddStack(stack);
        return inst;
    }

    
}
