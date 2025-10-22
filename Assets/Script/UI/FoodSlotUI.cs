using UnityEngine;

public class FoodSlotUI : ItemSlotUI
{
    //public override void Init(ItemSlot slot)
    //{
    //    base.Init(slot);
    //    slot.OnCondition += Slot_OnCondition;
    //}

    //public override void Deinit()
    //{
    //    if (Slot != null)
    //        Slot.OnCondition -= Slot_OnCondition;
    //    base.Deinit();
    //}

    //private bool Slot_OnCondition(Item arg)
    //{
    //    if (arg == null)
    //        return true;

    //    if (arg.TryGetAttribute<F>(out var attr))
    //    {
    //        if (attr.Part == part)
    //            return true;
    //    }
    //    return false;
    //}
}
