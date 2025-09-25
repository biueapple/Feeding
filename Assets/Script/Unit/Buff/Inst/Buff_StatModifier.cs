using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Buff/StatModifier")]
public class Buff_StatModifier : Buff
{
    [SerializeField]
    private DerivationKind type;

    [SerializeField]
    private float value;

    public override string Description => "{name}\n{type}이 {value} 만큼 상승합니다.\n남은시간 {duration}";
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
        
        administrator.Owner.AddStatModifier(new StatModifier(type, value, BuffName), BuffID);

        void action()
        {
            if (inst.Tick(1))
            {
                administrator.RemoveBuff(this);/* Remove(administrator, inst);*/
                DayCycleManager.Instance.OnNight -= action;
            }
        }

        DayCycleManager.Instance.OnNight += action;
    }

    public override void Reapply(BuffAdministrator administrator, BuffInstance inst)
    {
        if (administrator.Buffs.TryGetValue(BuffID, out var instance))
        {
            instance.Duration = Duration;
        }
    }

    public override void Remove(BuffAdministrator administrator, BuffInstance inst)
    {
        if (administrator.Owner == null) return;
        administrator.Owner.RemoveStatModifier(BuffID);
    }

    public override BuffInstance CreateInstance(BuffAdministrator administrator)
    {
        return new BuffInstance(this, administrator);
    }

    
}