using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Debuff/DotType_Burn")]
public class Burn : Dot
{
    public override string BuffID => "Burn";

    public override void Apply(object caster, BuffAdministrator target, BuffInstance inst)
    {
        if (target == null || target.Owner == null || inst == null)
            return;

        void action(AttackEventArgs args)
        {
            Debug.Log($"Burn ���� ���� {inst.Stacks}"); 
            AttackEventArgs a = new(caster as Unit, target.Owner, false);
            a.Damages.Add(new DamagePacket(type, "Burn", inst.Stacks));
            target.Owner.TakeDamage(a);

            if (inst.Tick(1))
            {
                Debug.Log("Burn ����");
                target.RemoveBuff(inst);
            }
        }

        target.SubscribeOnHitAfter(inst, action);
    }
}
