using System.Collections.Generic;
using UnityEngine;

public class StorageUserInterface : MonoBehaviour
{
    private InventoryInterface inventoryInterface;
    [SerializeField]
    private ItemSlotUI prefab;
    [SerializeField]
    private Transform content;

    private ItemSlotUI[] slots;

    //이미 생성된 슬롯
    private readonly Queue<ItemSlotUI> queue = new();

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
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Deinit();
            DeleteSlotUI(slots[i]);
        }
        slots = null;
        gameObject.SetActive(false);
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
            return Instantiate(prefab, content != null ? content : transform);
        }
    }

    private void DeleteSlotUI(ItemSlotUI slotUI)
    {
        slotUI.gameObject.SetActive(false);
        queue.Enqueue(slotUI);
    }
}
