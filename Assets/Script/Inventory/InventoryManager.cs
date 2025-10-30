using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    //��簡 �����ϴ� ������ �ִ� ����?
    [SerializeField]
    private Closet closet;
    public InventoryInterface HeroCloseInterface { get => closet.InventoryInterface; }
    
    //��簡 ������ ������ �� ���� �������� ì��� �������� ������ ��ġ�� ���ƿͼ� �ִ� ������ ����
    //���� �����̳� �Ҹ�ǰ�� �����߿� ������� �����ؾ� �ҵ�
    [SerializeField]
    private Chest heroChest;
    public InventoryInterface HeroChest { get => heroChest.InventoryInterface; private set { } }
    //�÷��̾ ����ϴ� ����
    [SerializeField]
    private Chest playerChest;
    public InventoryInterface PlayerChest { get => playerChest.InventoryInterface; private set { } }
    [SerializeField]
    private Hero hero;

    public int Gold { get; private set; }
    public event Action<int> OnAfterGold;

    //���⼭ �ִϸ��̼��̳� �׷��� ��Ʈ�� �ؾ��ҵ�
    public IEnumerator RunEquipPhase()
    {
        Equipment equip =  hero.GetComponent<Equipment>();
        foreach(var slot in closet.InventoryInterface.Itemslots)
        {
            if (slot.Item == null) continue;
            equip.TryEquip(slot.Item, out _);
            slot.Insert(null);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1);
        Debug.Log("���Ա� �Ϸ�");
    }

    public IEnumerator RunUnequipPhase()
    {
        Equipment equip = hero.GetComponent<Equipment>();
        Inventory inventory = hero.GetComponent<Inventory>();

        foreach (EquipmentPart part in Enum.GetValues(typeof(EquipmentPart)))
        {
            Item item = equip.Unequip(part);
            if (item != null)
            {
                HeroCloseInterface.InsertItem(item);
                yield return new WaitForSeconds(0.1f);
            }
        }

        HeroChest.InsertInventory(inventory.InventoryInterface);

        yield return new WaitForSeconds(1);
        Debug.Log("���ڷ� ������ �ű�� �Ϸ�");
    }

    public void EarnGold(int amount)
    {
        Gold = Mathf.Max(0, Gold + amount);
        OnAfterGold?.Invoke(Gold);
    }

    public bool TryEarnGold(int amount)
    {
        if (!CanAfford(amount)) return false;
        EarnGold(amount);
        return true;
    }

    public bool CanAfford(int cost)
    {
        if (Gold >= cost) return true;
        return false;
    }
}


public class ItemSlot
{
    private Item item;
    public Item Item => item;
    public Sprite Icon => item == null ? null : item.Icon;
    public string ItemName => item == null ? "" : item.ItemName;

    public event Func<Item, bool> OnCondition;
    public event Action<ItemSlot> OnBeforeChange;
    public event Action<ItemSlot> OnAfterChange;

    public void Insert(Item insert)
    {
        if (!Condition(insert))
            return;
        OnBeforeChange?.Invoke(this);
        (item, insert) = (insert, item);
        OnAfterChange?.Invoke(this);
    }

    public bool Condition(Item item)
    {
        if (OnCondition != null)
            foreach (Func<Item, bool> handler in OnCondition.GetInvocationList().Cast<Func<Item, bool>>())
                if (!handler(item)) return false;
        return true;
    }
}