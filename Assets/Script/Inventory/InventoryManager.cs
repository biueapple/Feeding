using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        for (int i = 0; i < inven.Length; i++)
        {
            inven[i] = new();
        }
    }

    private readonly ItemSlot[] inven = new ItemSlot[10];
    public ItemSlot[] Inven => inven;

    private int gold;
    public int Gold => gold;
    public event Action<int> OnAfterGold;


    public bool InsertItem(Item item, out Item ex)
    {
        ex = null;

        for (int i = 0; i < inven.Length; i++)
        {
            if (inven[i].Item == null)
            {
                ex = inven[i].Item;
                inven[i].Insert(item);
                return true;
            }
        }

        return false;
    }

    //마이너스도 가능하니까
    public void EarnGold(int value)
    {
        gold += value;
        OnAfterGold?.Invoke(gold);
    }
}


public class ItemSlot
{
    private Item item;
    public Item Item => item;
    public Sprite Icon => item == null ? null : item.Icon;
    public string ItemName => item == null ? "" : item.ItemName;

    public event Action<ItemSlot> OnBeforeChange;
    public event Action<ItemSlot> OnAfterChange;

    public void Insert(Item insert)
    {
        OnBeforeChange?.Invoke(this);
        (item, insert) = (insert, item);
        OnAfterChange?.Invoke(this);
    }
}