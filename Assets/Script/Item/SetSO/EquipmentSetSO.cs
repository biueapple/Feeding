using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Set/SetSO")]
public class EquipmentSetSO : ScriptableObject
{
    [SerializeField]
    private string setName;
    public string SetName => setName;

    [Header("2세트 효과"), SerializeField]
    private List<EquipmentEffect> twoSetEffect = new();
    public IReadOnlyList<EquipmentEffect> TwoSetEffect => twoSetEffect;
    public void ApplyTwoSet(Equipment equipment)
    {
        foreach (var effect in twoSetEffect)
            effect.Apply(equipment.Owner);
    }
    public void RemoveTwoSet(Equipment equipment)
    {
        foreach (var effect in twoSetEffect)
            effect.Remove(equipment.Owner);
    }

    [Header("4세트 효과"), SerializeField]
    private List<EquipmentEffect> fourSetEffect = new();
    public IReadOnlyList<EquipmentEffect> FourSetEffect => fourSetEffect;
    public void ApplyFourSet(Equipment equipment)
    {
        foreach (var effect in fourSetEffect)
            effect.Apply(equipment.Owner);
    }
    public void RemoveFourSet(Equipment equipment)
    {
        foreach (var effect in fourSetEffect)
            effect.Remove(equipment.Owner);
    }
}
