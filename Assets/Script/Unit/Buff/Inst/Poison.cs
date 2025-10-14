using UnityEngine;

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
        administrator.SubscribeOnRecoveryBefore(inst, reduction);
        //지속시간 계산
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
    }

    public override BuffInstance CreateInstance(BuffAdministrator administrator)
    {
        BuffInstance inst = new(this, administrator)
        {
            Duration = Duration
        };

        inst.AddStack(Stack);
        return inst;
    }
}
