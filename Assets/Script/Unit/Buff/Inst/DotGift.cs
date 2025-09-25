using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Debuff/Gift Dot")]
public class DotGift : Buff
{
    [SerializeField]
    private Dot dot;
    [SerializeField]
    private int value;

    public override string BuffID => dot.BuffID ?? BuffID;
    public override string Description => "{dot}을 {value} 만큼 부여합니다.";
    public override string BuildDescription(BuffInstance inst)
    {
        string s = base.BuildDescription(inst);
        s = s.Replace("{dot}", dot.BuffName);
        s = s.Replace("{value}", value.ToString());
        return s;
    }

    public override void Apply(BuffAdministrator administrator, BuffInstance inst)
    {
        if (administrator.Owner == null)
            return;
        dot.Apply(administrator, inst);
        inst.Duration += value;
    }

    public override void Reapply(BuffAdministrator administrator, BuffInstance inst)
    {
        inst.Duration += value;
    }

    public override void Remove(BuffAdministrator administrator, BuffInstance inst)
    {
        if (administrator.Owner == null) return;
        administrator.Owner.RemoveStatModifier(BuffID);
    }

    public override BuffInstance CreateInstance(BuffAdministrator administrator)
    {
        return dot.CreateInstance(administrator);
    }
}
