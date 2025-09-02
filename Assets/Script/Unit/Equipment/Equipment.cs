using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EquipmentPart
{
    Helmet,
    Breastplate,
    Legguards,
    Boots,
    Accessories,

}

//��� ȿ���� ��Ʈ ȿ���� �ߺ��ǰų� �����ص� �ٸ� ���� ���� ȿ���� �����ִ� ��츦 �����ؾ���
public class Equipment : MonoBehaviour
{
    public Unit Owner { get; private set; }

    private readonly Dictionary<EquipmentPart, Item> equipment = new();         //�����ϰ� �ִ� ���
    //private readonly Dictionary<EquipmentEffect, int> activeEffects = new();    //�������� ȿ���� �� ��
    private readonly Dictionary<EquipmentSetSO, (int count, bool two, bool four)> setCounter = new();        //����� ��Ʈ ȿ���� ����

    //��� ������ �� �ִ��� Ȯ���ϴ� �̺�Ʈ    (�⺻������ �������� �������Ѱ͵��� �ʿ���)
    public readonly List<IEquipCondition> OnCondition = new();
    public void AddCondition(IEquipCondition func) => OnCondition.Add(func);
    public void RemoveCondition(IEquipCondition func) => OnCondition.Remove(func);

    //��� ������ �� ȣ���ϴ� �̺�Ʈ
    public event Action<Item> OnEquipped;
    //��� ������ �� ȣ���ϴ� �̺�Ʈ
    public event Action<Item> OnUnequipped;

    private void Awake()
    {
        Owner = GetComponent<Unit>();
    }

    //��� �����ϵ��� �õ��ϰ� replacedItem���� ������ �������� ��
    public bool TryEquip(Item item, out Item replacedItem)
    {
        replacedItem = null;

        //��� �������� �ƴѰ��
        if (!item.TryGetAttribute(out EquipmentAttribute attr)) return false;

        //������ �Ҹ����ϴ� ���
        if (!Condition(item)) return false;

        if(equipment.TryGetValue(attr.Part, out replacedItem))
        {
            Unequip(attr.Part);
        }

        equipment[attr.Part] = item;
        attr.Apply(this);

        if (attr.EquipmentSet != null)
        {
            if (!setCounter.ContainsKey(attr.EquipmentSet)) setCounter[attr.EquipmentSet] = (0, false, false);
            EquipSetCheck(setCounter, attr.EquipmentSet);
        }

        OnEquipped?.Invoke(item);
        return true;
    }    

    //��� �����ϰ� ���� �������� ��������
    public Item Unequip(EquipmentPart part)
    {
        if (!equipment.TryGetValue(part, out Item item) || item == null)
            return null;

        equipment[part] = null;
        if(item.TryGetAttribute(out EquipmentAttribute attr) && attr.EquipmentSet != null)
        {
            RemoveSetCheck(setCounter, attr.EquipmentSet);
        }
        OnUnequipped?.Invoke(item);
        return item;
    }

    //��� ������ �ѷ����� �ϳ��� false��� false
    public bool Condition(Item item)
    {
        return OnCondition.All(c => c.Condition(item));
    }

    public void EquipSetCheck(Dictionary<EquipmentSetSO, (int count, bool two, bool four)> keys, EquipmentSetSO set)
    {
        var (count, two, four) = keys[set];
        count++;
        if(count >= 2 && !two)
        {
            set.ApplyTwoSet(this);
            two = true;
        }
        if(count >= 4 && !four)
        {
            set.ApplyFourSet(this);
            four = true;
        }
        keys[set] = (count, two, four);
    }

    public void RemoveSetCheck(Dictionary<EquipmentSetSO, (int count, bool two, bool four)> keys, EquipmentSetSO set)
    {
        var (count, two, four) = keys[set];
        count--;
        if (count < 2 && two)
        {
            set.RemoveTwoSet(this);
            two = false;
        }
        if (count < 4 && four)
        {
            set.RemoveFourSet(this);
            four = false;
        }
        keys[set] = (count, two, four);
    }
}