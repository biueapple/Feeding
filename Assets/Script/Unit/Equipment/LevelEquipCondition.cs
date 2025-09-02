using UnityEngine;

public class LevelEquipCondition : IEquipCondition
{
    private Unit unit;

    public LevelEquipCondition(Unit unit)
    {
        this.unit = unit;
    }

    public bool Condition(Item item)
    {
        if (unit == null)
            return false;

        if(item.TryGetAttribute(out EquipmentAttribute attr))
        {
            return unit.level >= attr.Level;
        }
        return true;
    }
}
