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

        HeroChest = new(10);
        PlayerChest = new(10);
    }

    //��簡 ������ ������ �� ���� �������� ì��� �������� ������ ��ġ�� ���ƿͼ� �ִ� ������ ����
    //���� �����̳� �Ҹ�ǰ�� �����߿� ������� �����ؾ� �ҵ�
    public InventoryInterface HeroChest { get; private set; }
    //�÷��̾ ����ϴ� ����
    public InventoryInterface PlayerChest { get; private set; }
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