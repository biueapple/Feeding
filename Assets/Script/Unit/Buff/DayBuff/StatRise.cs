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
        Debug.Log("치확 증가");
        administrator.Owner.AddStatModifier(new StatModifier(type, value, BuffName), BuffID);
    }

    public override void Remove(BuffAdministrator administrator)
    {
        if (administrator.Owner == null)
            return;
        if (!administrator.Owner.Derivaton.ContainsKey(type))
            return;
        administrator.Owner.RemoveStatModifier(BuffID);
    }

    public override BuffInstance CreateInstance(BuffAdministrator administrator)
    {
        return new BuffInstance(this, administrator);
    }
}
