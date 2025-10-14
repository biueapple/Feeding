using UnityEngine;

//�ɸ������� ������ 1�� �����ϴ� ����
//du�� ��¼�� �� ����������
//ġ�����Ұ� �ִ°ɷ�
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
            Debug.Log($"poison���� ���� ���� {inst.Stacks}");
            administrator.Owner.CurrentHP -= inst.Stacks;
            if (inst.Tick(1))
            {
                Debug.Log("poison ����");
                administrator.RemoveBuff(this);
            }
        }

        //ġ������
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
