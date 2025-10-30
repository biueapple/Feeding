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

    //�κ��丮���� ������ ��ŭ ��������
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

    //��� �������� ������ �ִ���
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
