using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Debuff/Bleeding")]
public class Bleeding : Debuff
{
    public override string Description => "{name}\n�ൿ �� {value}�� {type}������� �޽��ϴ�.\n����Ƚ��{duration}";
    public override string BuildDescription(DebuffInstance inst)
    {
        string s = base.BuildDescription(inst);
        s = s.Replace("{type}", DamageType.Physical.ToString());
        s = s.Replace("{value}", inst.Stack.ToString());
        return s;
    }

    public override void Apply(BuffAdministrator administrator)
    {
        if (administrator.Owner == null)
            return;
        //administrator.Owner.AddStatModifier(new StatModifier(type, -value, DebuffName), DebuffID);
    }

    public override void Remove(BuffAdministrator administrator)
    {
        if (administrator.Owner == null) return;
        administrator.Owner.RemoveStatModifier(DebuffID);
    }

    public override void Tick(BuffAdministrator administrator)
    {

    }

    public override DebuffInstance CreateInstance(BuffAdministrator administrator)
    {
        return new DebuffInstance(this, administrator);
    }
}
