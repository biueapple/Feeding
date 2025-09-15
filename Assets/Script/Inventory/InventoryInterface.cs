using System;
using UnityEngine;

public class InventoryInterface
{
    private readonly ItemSlot[] itemslots;
    public ItemSlot[] Itemslots => itemslots;
    public int Gold { get; private set; }
    public event Action<int> OnAfterGold;

    public InventoryInterface(int length)
    {
        itemslots = new ItemSlot[length];
        for(int i = 0; i < length; i++)
        {
            itemslots[i] = new();
        }
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

    public bool InsertItem(Item item)
    {
        for (int i = 0; i < itemslots.Length; i++)
        {
            if (itemslots[i].Item == null)
            {
                itemslots[i].Insert(item);
                return true;
            }
        }

        return false;
    }

    //�κ��丮���� ������ ��ŭ ��������
    public void InsertInventory(InventoryInterface inventory)
    {
        EarnGold(inventory.Gold);
        inventory.EarnGold(-inventory.Gold);

        int index = 0;
        for(int i = 0; i < itemslots.Length; i++)
        {
            while (index < inventory.itemslots.Length && inventory.itemslots[index].Item == null)
                index++;
            if (index >= inventory.itemslots.Length) break;

            if (InsertItem(inventory.itemslots[index].Item))
            {
                inventory.itemslots[index].Insert(null);
                index++;
            }
            else
            {
                break;
            }
        }
    }

    //�׽�Ʈ ���� �޼ҵ�
    public void Print()
    {
        foreach (var i in itemslots)
        {
            if (i.Item != null)
                Debug.Log($"������ �̸� {i.Item.ItemName}");
        }
    }
}
