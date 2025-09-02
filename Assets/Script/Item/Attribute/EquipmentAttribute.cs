using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Attribute/EquipmentAttribute")]
public class EquipmentAttribute : ItemAttribute
{
    [SerializeField]
    private int level;                      //장착하기위한 레벨제한
    public int Level => level;

    //나중에 StatMapping이 아니라 Foundation 스탯을 올려주는걸로 수정해야 하는데 지금은 그냥 이걸로 하자
    [SerializeField]
    private List<StatMapping> stats;            //장착시 올라가는 스탯
    public IReadOnlyList<StatMapping> Stats => stats;

    [SerializeField]
    private EquipmentPart part;             //어디에 장착할 수 있는지
    public EquipmentPart Part => part;

    [SerializeField]
    private EquipmentSetSO equipmentSet;                    //어떤 아이템과 세트인지 (다른 장비 아이템에 같은 equipmentSetSO가 있는지로 알 수 있음)
    public EquipmentSetSO EquipmentSet => equipmentSet;

    [SerializeField]
    private List<EquipmentEffect> effects;
    public IReadOnlyList<EquipmentEffect> Effects => effects;

    public void Apply(Equipment equipment)
    {
        foreach (var effect in effects)
            effect.Apply(equipment.Owner);
    }

    public void Remove(Equipment equipment)
    {
        foreach (var effect in effects)
            effect.Remove(equipment.Owner);
    }
}
