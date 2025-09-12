using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Buff/StatRise")]
public class StatRise : Buff
{
    [SerializeField]
    private DerivationKind type;

    [SerializeField]
    private float value;

    public override void Apply(BuffAdministrator administrator)
    {
        if (administrator.Owner == null)
            return;
        administrator.Owner.AddStatModifier(new StatModifier(type, value, BuffName), BuffID);
    }

    public override void Remove(BuffAdministrator administrator)
    {
        if (administrator.Owner == null) return;
        administrator.Owner.RemoveStatModifier(BuffID);
    }

    public override BuffInstance CreateInstance(BuffAdministrator administrator)
    {
        return new BuffInstance(this, administrator);
    }
}
