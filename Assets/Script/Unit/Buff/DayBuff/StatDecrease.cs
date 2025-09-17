using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Debuff/StatDecrease")]
public class StatDecrease : Debuff
{
    [SerializeField]
    private DerivationKind type;

    [SerializeField, Header("- ���� �Է�")]
    private float value;

    public override string Description => "{name}\n{type}�� {value} ��ŭ ����մϴ�.\n�����ð�{duration}";
    public override string BuildDescription(DebuffInstance inst)
    {
        string s = base.BuildDescription(inst);
        s = s.Replace("{type}", type.ToString());
        s = s.Replace("{value}", value.ToString());
        return s;
    }

    public override void Apply(BuffAdministrator administrator)
    {
        if (administrator.Owner == null)
            return;
        administrator.Owner.AddStatModifier(new StatModifier(type, -value, DebuffName), DebuffID);
    }

    public override void Remove(BuffAdministrator administrator)
    {
        if (administrator.Owner == null) return;
        administrator.Owner.RemoveStatModifier(DebuffID);
    }

    public override DebuffInstance CreateInstance(BuffAdministrator administrator)
    {
        return new DebuffInstance(this, administrator);
    }
}
