using System;
using UnityEngine;

public class InventoryInterface
{
    private readonly ItemSlot[] itemslots;
    public ItemSlot[] Itemslots => itemslots;

    public InventoryInterface(int length)
    {
        itemslots = new ItemSlot[length];
        for(int i = 0; i < length; i++)
        {
            itemslots[i] = new();
        }
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

    //인벤토리에서 가능한 만큼 가져오기
    public void InsertInventory(InventoryInterface inventory)
    {
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

    //몇개의 아이템을 가지고 있는지
    public int Count()
    {
        int count = 0;
        foreach(var slot in itemslots)
        {
            if (slot.Item != null) count++;
        }
        return count;
    }
}
