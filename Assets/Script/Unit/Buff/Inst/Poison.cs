using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

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

    public override void Apply(object caster, BuffAdministrator target, BuffInstance inst)
    {
        if (target.Owner == null)
            return;

        void action()
        {
            float damage = inst.Stacks;
            Debug.Log($"poison���� ���� ���� {damage}");
            AttackEventArgs a = new(caster as Unit, target.Owner, false);
            a.Damages.Add(new DamagePacket(type, "Poison", damage));
            target.Owner.TakeDamage(a);

            if (inst.Tick(1))
            {
                Debug.Log("poison ����");
                target.RemoveBuff(inst);
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
        target.SubscribeOnRecoveryBefore(inst, reduction);
        //���ӽð� ���
        target.SubscribeOnSecond(inst, action);
    }

    public override void Reapply(object caster, BuffAdministrator target, List<BuffInstance> list)
    {
        if (target == null || list == null) return;

        //��� list�� �������� �������� ����
        foreach(var inst in list)
        {
            inst.AddStack();
            if (inst.Duration < Duration) inst.Duration = Duration;
        }
    }
}
