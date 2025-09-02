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

//장비 효과나 새트 효과가 중복되거나 해제해도 다른 장비로 인해 효과가 남아있는 경우를 구현해야함
public class Equipment : MonoBehaviour
{
    public Unit Owner { get; private set; }

    private readonly Dictionary<EquipmentPart, Item> equipment = new();         //장착하고 있는 장비
    //private readonly Dictionary<EquipmentEffect, int> activeEffects = new();    //적용중인 효과와 그 수
    private readonly Dictionary<EquipmentSetSO, (int count, bool two, bool four)> setCounter = new();        //장비의 세트 효과와 숫자

    //장비를 장착할 수 있는지 확인하는 이벤트    (기본적으로 레벨제한 스텟제한것들이 필요함)
    public readonly List<IEquipCondition> OnCondition = new();
    public void AddCondition(IEquipCondition func) => OnCondition.Add(func);
    public void RemoveCondition(IEquipCondition func) => OnCondition.Remove(func);

    //장비를 장착한 후 호출하는 이벤트
    public event Action<Item> OnEquipped;
    //장비를 해제한 후 호출하는 이벤트
    public event Action<Item> OnUnequipped;

    private void Awake()
    {
        Owner = GetComponent<Unit>();
    }

    //장비를 장착하도록 시도하고 replacedItem에는 기존의 아이템이 들어감
    public bool TryEquip(Item item, out Item replacedItem)
    {
        replacedItem = null;

        //장비 아이템이 아닌경우
        if (!item.TryGetAttribute(out EquipmentAttribute attr)) return false;

        //조건을 불만족하는 경우
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

    //장비를 해제하고 기존 아이템을 리턴해줌
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

    //모든 조건을 둘러보며 하나라도 false라면 false
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