using System.Collections.Generic;
using UnityEngine;

public class ClosetUserInterface : MonoBehaviour
{
    private InventoryInterface inventoryInterface;
    
    [SerializeField]
    private ItemSlotUI[] slots;

    public void Interlock(InventoryInterface inventoryInterface)
    {
        if (this.inventoryInterface != null && slots != null && slots.Length != 0)
            Close();

        this.inventoryInterface = inventoryInterface;
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
        }
        gameObject.SetActive(false);
    }
}
