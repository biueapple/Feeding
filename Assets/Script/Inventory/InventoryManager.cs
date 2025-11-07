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

    //용사가 장착하는 장비들이 있는 옷장?
    [SerializeField]
    private Closet closet;
    public InventoryInterface HeroCloseInterface { get => closet.InventoryInterface; }
    
    //용사가 모험을 떠나기 전 안의 아이템을 챙기는 상자이자 모험을 마치고 돌아와서 넣는 아이템 상자
    //안의 음식이나 소모품은 모험중에 사용할지 생각해야 할듯
    [SerializeField]
    private Chest heroChest;
    public InventoryInterface HeroChest { get => heroChest.InventoryInterface; private set { } }
    //플레이어가 사용하는 상자
    [SerializeField]
    private Chest playerChest;
    public InventoryInterface PlayerChest { get => playerChest.InventoryInterface; private set { } }
    [SerializeField]
    private Hero hero;

    public int Gold { get; private set; }
    public event Action<int> OnAfterGold;

    //여기서 애니메이션이나 그런거 컨트롤 해야할듯
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
        Debug.Log("옷입기 완료");
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
        EarnGold(inventory.Gold);
        inventory.Gold = 0;

        yield return new WaitForSeconds(1);
        Debug.Log("상자로 아이템 옮기기 완료");
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