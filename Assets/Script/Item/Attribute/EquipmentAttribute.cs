using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Attribute/EquipmentAttribute")]
public class EquipmentAttribute : ItemAttribute
{
    [SerializeField]
    private int level;                      //�����ϱ����� ��������
    public int Level => level;

    //���߿� StatMapping�� �ƴ϶� Foundation ������ �÷��ִ°ɷ� �����ؾ� �ϴµ� ������ �׳� �̰ɷ� ����
    [SerializeField]
    private List<StatMapping> stats;            //������ �ö󰡴� ����
    public IReadOnlyList<StatMapping> Stats => stats;

    [SerializeField]
    private EquipmentPart part;             //��� ������ �� �ִ���
    public EquipmentPart Part => part;

    [SerializeField]
    private EquipmentSetSO equipmentSet;                    //� �����۰� ��Ʈ���� (�ٸ� ��� �����ۿ� ���� equipmentSetSO�� �ִ����� �� �� ����)
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
