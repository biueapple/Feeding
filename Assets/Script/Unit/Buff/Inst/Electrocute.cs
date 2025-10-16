using System.Collections.Generic;
using UnityEngine;

//������ reapply�ϸ� ���� ������� ���� �ѹ��� �ް� �ٽ� �ɸ��°ɷ�
[CreateAssetMenu(menuName = "RPG/Debuff/DotType_Electrocute")]
public class Electrocute : Dot
{
    public override string BuffID => "Electrocute";

    public override void Apply(Unit caster, BuffAdministrator target, BuffInstance inst)
    {
        if (target.Owner == null)
            return;

        void action()
        {
            float damage = inst.Stacks;
            Debug.Log($"Electrocute ���� ���� {damage}");

            AttackEventArgs a = new(caster, target.Owner, false);
            a.Damages.Add(new DamagePacket(type, "Electrocute", damage));
            target.Owner.TakeDamage(a);

            if (inst.Tick(1))
            {
                Debug.Log("Electrocute ����");
                target.RemoveBuff(inst);
            }
        }

        target.SubscribeOnSecond(inst, action);
    }

    public override void Reapply(Unit caster, BuffAdministrator target, List<BuffInstance> list)
    {
        if (target == null || list == null) return;

        //��� list�� �������� �������� ����
        foreach (var inst in list)
        {
            float damage = inst.Stacks * inst.Duration;
            AttackEventArgs a = new(caster, target.Owner, false);
            a.Damages.Add(new DamagePacket(type, "Electrocute", damage));
            target.Owner.TakeDamage(a);
            target.RemoveBuff(inst);
        }

        //Reapply�� ȣ���Ҷ��� CreateInstance ���� �ʰ� ȣ���ϱ⿡ ���� ���� �־��ֱ�
        BuffInstance instance = CreateInstance(target);
        target.AddInstance(caster, instance);
    }
}
