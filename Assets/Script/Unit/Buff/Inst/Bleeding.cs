using System;
using System.Collections.Generic;
using UnityEngine;

//�ɶ����� du�� 1�� �����ϵ���
//�ѹ��� ������ �ɾ�� du�� ������� ���� ��ø�ϱ� ���� ��������
//�ٵ� ���ݸ��� ������ du�� �پ��� ���� ������
[CreateAssetMenu(menuName = "RPG/Debuff/DotType_Bleeding")]
public class Bleeding : Dot
{
    public override string BuffID => "Bleeding";
    public override float Duration => 1;    //������ �׻� ���ӽð� 1

    public override void Apply(object caster, BuffAdministrator target, BuffInstance inst)
    {
        if (target.Owner == null)
            return;

        void action(AttackEventArgs args)
        {
            float damage = inst.Stacks;
            Debug.Log($"bleeding���� ���� ���� {damage}");

            AttackEventArgs a = new(caster as Unit, target.Owner, false);
            a.Damages.Add(new DamagePacket(type, this, damage));
            target.Owner.TakeDamage(a);

            if (inst.Tick(1))
            {
                Debug.Log("���� ����");
                target.RemoveBuff(inst);
            }
        }

        target.SubscribeOnAfterAttack(inst, action);
    }

    public override void Reapply(object caster, BuffAdministrator target, List<BuffInstance> list)
    {
        if (target == null || list == null) return;

        //��� list�� �������� �������� ����
        foreach(var inst in list)
        {
            inst.Duration++;
            inst.AddStack(stack);
        }
    }
}
