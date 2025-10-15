using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Buff/StatModifier")]
public class Buff_StatModifier : Buff
{
    [SerializeField]
    private DerivationKind type;

    [SerializeField]
    private float value;

    public override string BuildDescription(BuffInstance inst)
    {
        string s = base.BuildDescription(inst);
        s = s.Replace("{type}", type.ToString());
        s = s.Replace("{value}", value.ToString());
        return s;
    }

    public override void Apply(BuffAdministrator administrator, BuffInstance inst)
    {
        if (administrator.Owner == null)
            return;

        administrator.SubscribeOnNight(inst, action);
        
        administrator.Owner.AddStatModifier(new StatModifier(type, value, BuffName), BuffID);

        void action()
        {
            if (inst.Tick(1))
            {
                administrator.RemoveBuff(inst);
            }
        }
    }

    public override void Reapply(BuffAdministrator administrator, List<BuffInstance> list)
    {
        if (list == null || administrator == null) return;

        //사실 list에 여러개가 있을리가 없긴 함
        foreach(var inst in list)
            inst.Duration = Duration;
    }

    public override void Remove(BuffAdministrator administrator, BuffInstance inst)
    {
        if (administrator.Owner == null) return;
        administrator.Owner.RemoveStatModifier(BuffID);
    }

    public override BuffInstance CreateInstance(BuffAdministrator administrator)
    {
        return new (this, administrator);
    }
}