using UnityEngine;
using UnityEngine.UI;

public class DragSlotUI : MonoBehaviour
{
    private ItemSlotUI pre;
    [SerializeField]
    private Image icon;

    private RectTransform rect;
    public RectTransform Rect { get { if(rect == null) rect = GetComponent<RectTransform>(); return rect; } }

    public void Begin(ItemSlotUI slot)
    {
        if (slot.Slot.Item == null)
            return;
        //실제 데이터는 이동하지 않음
        gameObject.SetActive(true);
        pre = slot;
        SetSprite(slot.Slot.Item.Icon);
    }

    public void Drop(ItemSlotUI slot)
    {
        if (pre == null) return;
        //실제로 데이터의 이동
        var (item1, item2) = (slot.Slot.Item, pre.Slot.Item);
        if(pre.Slot.Condition(item1) && slot.Slot.Condition(item2))
        {
            pre.Slot.Insert(item1);
            slot.Slot.Insert(item2);
        }
        else
        {

        }
    }

    public void End()
    {
        //다시 리프레시만 하고 실제 데이터 이동은 없음
        gameObject.SetActive(false);
    }

    public void SetSprite(Sprite sprite)
    {
        icon.sprite = sprite;
    }
}
