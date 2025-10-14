using UnityEngine;

//걸릴때마다 스택이 1씩 증가하는 형태
//du는 어쩌지 더 높은쪽으로
//치유감소가 있는걸로
[CreateAssetMenu(menuName = "RPG/Debuff/DotType_Poison")]
public class Poison : Dot
{
    public override string BuffID => "Poison";

    public override void Apply(BuffAdministrator administrator, BuffInstance inst)
    {
        if (administrator.Owner == null)
            return;

        void action()
        {
            Debug.Log($"poison으로 인한 피해 {inst.Stacks}");
            administrator.Owner.CurrentHP -= inst.Stacks;
            if (inst.Tick(1))
            {
                Debug.Log("poison 끝남");
                administrator.RemoveBuff(this);
            }
        }

        //치유감소
        administrator.Owner.OnRecoveryBefore += Owner_OnRecoveryBefore;
        administrator.SubscribeOnSecond(inst, action);
    }

    public override void Reapply(BuffAdministrator administrator, BuffInstance inst)
    {
        inst.AddStack();
        if (inst.Duration < Duration) inst.Duration = Duration;
    }

    public override void Remove(BuffAdministrator administrator, BuffInstance inst)
    {
        if (administrator == null) return;
        administrator.Owner.OnRecoveryBefore -= Owner_OnRecoveryBefore;
    }

    public override BuffInstance CreateInstance(BuffAdministrator administrator)
    {
        BuffInstance inst = new(this, administrator)
        {
            Duration = Duration
        };

        inst.AddStack(1);
        return inst;
    }

    private void Owner_OnRecoveryBefore(RecoveryEventArgs args)
    {
        foreach(var pack in args.Recovery)
        {
            pack.Value -= Mathf.Max(pack.Value * stack * 0.01f, 30);
        }
    }

}
