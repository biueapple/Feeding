using System.Collections.Generic;
using UnityEngine;

public class TooltipService : MonoBehaviour
{
    public static TooltipService Instance { get; private set; }

    [SerializeField]
    private TooltipView view;

    [SerializeField]
    private ItemDHeader prefab_header;
    [SerializeField]
    private ItemDDescription prefab_desc;
    [SerializeField]
    private ItemDEquipment prefab_equip;

    private ItemDHeader header;
    private ItemDDescription desc;
    private ItemDEquipment equip;
    //private readonly Queue<ItemDEquipment> equip = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        header = Instantiate(prefab_header, transform);
        desc = Instantiate(prefab_desc, transform);
        equip = Instantiate(prefab_equip, transform);
    }

    public void TooltipOpen(ItemSlotUI slot)
    {
        if (slot.Slot.Item == null) return;

        header.Setting(slot.Slot.Item);
        header.gameObject.SetActive(true);
        view.Attaching(header.Rect);

        //if(slot.Slot.Item.TryGetAttribute<EquipmentAttribute>(out var result))
        //{
        //    equip.Setting(result);
        //    equip.gameObject.SetActive(true);
        //    view.Attaching(equip.Rect);
        //}

        desc.Setting(slot.Slot.Item);
        desc.gameObject.SetActive(true);
        view.Attaching(desc.Rect);

        view.transform.position = slot.transform.position;
    }

    public void TooltipClose()
    {
        header.gameObject.SetActive(false);
        equip.gameObject.SetActive(false);
        desc.gameObject.SetActive(false);
        view.Clear();
    }

    //private ItemDEquipment CreateEquip()
    //{
    //    if(!equip.TryDequeue(out var result))
    //    {
    //        result = Instantiate(prefab_equip, transform);
    //    }
    //    return result;
    //}
}
