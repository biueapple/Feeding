using UnityEngine;

//�ɸ������� ������ 1�� �����ϴ� ����
//du�� ��¼�� �� ����������
//ġ�����Ұ� �ִ°ɷ�
[CreateAssetMenu(menuName = "RPG/Debuff/DotType_Poison")]
public class Poison : Dot
{
    public override string BuffID => "Poison";
    public override int Stack => 1;             //�׻� ������ 1

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
            Debug.Log($"poison���� ���� ���� {inst.Stacks}");
            administrator.Owner.CurrentHP -= inst.Stacks;
            if (inst.Tick(1))
            {
                Debug.Log("poison ����");
                administrator.RemoveBuff(this);
            }
        }

        void reduction(RecoveryEventArgs args)
        {
            foreach (var pack in args.Recovery)
            {
                //�ִ� ġ�� ��ġ�� 30��
                float stack = Mathf.Min(inst.Stacks, 30);
                pack.Value -= pack.Value * stack * 0.01f;
            }
        }

        //ġ������
        administrator.SubscribeOnRecoveryBefore(inst, reduction);
        //���ӽð� ���
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
