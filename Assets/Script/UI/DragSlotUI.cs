using UnityEngine;
using UnityEngine.UI;

public class DragSlotUI : MonoBehaviour
{
    private ItemSlotUI pre;
    [SerializeField]
    private Image icon;

    public void Begin(ItemSlotUI slot)
    {
        if (slot.Slot.Item == null)
            return;
        //���� �����ʹ� �̵����� ����
        gameObject.SetActive(true);
        pre = slot;
        SetSprite(slot.Slot.Icon);
    }

    public void Drop(ItemSlotUI slot)
    {
        if (pre == null) return;
        //������ �������� �̵�
        var (item1, item2) = (slot.Slot.Item, pre.Slot.Item);
        pre.Slot.Insert(item1);
        slot.Slot.Insert(item2);
    }

    public void End()
    {
        //�ٽ� �������ø� �ϰ� ���� ������ �̵��� ����
        gameObject.SetActive(false);
    }

    public void SetSprite(Sprite sprite)
    {
        icon.sprite = sprite;
    }
}
