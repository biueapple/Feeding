using System;
using UnityEngine;

//�ɶ����� du�� 1�� �����ϵ���
//�ѹ��� ������ �ɾ�� du�� ������� ���� ��ø�ϱ� ���� ��������
//�ٵ� ���ݸ��� ������ du�� �پ��� ���� ������
[CreateAssetMenu(menuName = "RPG/Debuff/DotType_Bleeding")]
public class Bleeding : Dot
{
    public override string BuffID => "Bleeding";
    public override float Duration => 1;    //������ �׻� ���ӽð� 1

    public override void Apply(BuffAdministrator administrator, BuffInstance inst)
    {
        if (administrator.Owner == null)
            return;

        void action(AttackEventArgs args)
        {
            Debug.Log($"bleeding���� ���� ���� {inst.Stacks}");
            administrator.Owner.CurrentHP -= inst.Stacks;
            if (inst.Tick(1))
            {
                Debug.Log("���� ����");
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
