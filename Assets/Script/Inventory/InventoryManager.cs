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
    }

    //용사가 모험을 떠나기 전 안의 아이템을 챙기는 상자이자 모험을 마치고 돌아와서 넣는 아이템 상자
    //안의 음식이나 소모품은 모험중에 사용할지 생각해야 할듯
    [SerializeField]
    private Chest heroChest;
    public InventoryInterface HeroChest { get => heroChest.InventoryInterface; private set { } }
    //플레이어가 사용하는 상자
    [SerializeField]
    private Chest playerChest;
    public InventoryInterface PlayerChest { get => playerChest.InventoryInterface; private set { } }
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