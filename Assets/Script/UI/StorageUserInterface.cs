using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageUserInterface : MonoBehaviour
{
    private InventoryInterface inventoryInterface;
    [SerializeField]
    private ItemSlotUI prefab;

    private ItemSlotUI[] slots;

    //이미 생성된 슬롯
    private readonly Queue<ItemSlotUI> queue = new();

    private RectTransform rect;
    public RectTransform Rect { get { if (rect == null) rect = GetComponent<RectTransform>(); return rect; } }

    public event Action<InventoryInterface> OnClose;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Interlock(InventoryInterface inventoryInterface)
    {
        if(this.inventoryInterface != null && slots != null && slots.Length != 0)
            Close();

        this.inventoryInterface = inventoryInterface;
        slots = new ItemSlotUI[inventoryInterface.Itemslots.Length];
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i] = CreateSlotUI();         
        }
    }

    public void Open()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Init(inventoryInterface.Itemslots[i]);
        }
        gameObject.SetActive(true);
    }

    public void Close()
    {
        if (slots == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Deinit();
            DeleteSlotUI(slots[i]);
        }
        slots = null;
        gameObject.SetActive(false);
        OnClose?.Invoke(inventoryInterface);
    }

    private ItemSlotUI CreateSlotUI()
    {
        if(queue.TryDequeue(out ItemSlotUI result))
        {
            result.gameObject.SetActive(true);
            return result;
        }
        else
        {
            return Instantiate(prefab, transform);
        }
    }

    private void DeleteSlotUI(ItemSlotUI slotUI)
    {
        slotUI.gameObject.SetActive(false);
        queue.Enqueue(slotUI);
    }
}
