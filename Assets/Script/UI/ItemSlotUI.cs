using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, ITooltipHeaderProvider, ITooltipBottomProvider
{
    public ItemSlot Slot { get; private set; }

    public Transform Transform => transform;

    public Vector2 Offset => new (50, 100);

    [SerializeField]
    private Image back;
    [SerializeField]
    private Image icon;

    public void SetSprite(Sprite sprite)
    {
        icon.sprite = sprite;
    }

    public virtual void Init(ItemSlot slot)
    {
        if (slot == null) { Deinit(); return; }
        Slot = slot;
        slot.OnBeforeChange += OnBeforeChange;
        slot.OnAfterChange += OnAfterChange;
        OnAfterChange(slot);
    }

    public virtual void Deinit()
    {
        if (Slot != null)
        {
            Slot.OnBeforeChange -= OnBeforeChange;
            Slot.OnAfterChange -= OnAfterChange;
        }
        Slot = null;
    }

    private void OnBeforeChange(ItemSlot slot)
    {
        
    }
    private void OnAfterChange(ItemSlot slot)
    {
        icon.sprite = slot.Icon;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        UIManager.Instance.DragSlot.Begin(this);
        //UIManager.Instance.ItemDescription.Enable = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        UIManager.Instance.DragSlot.Rect.position = Mouse.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UIManager.Instance.DragSlot.End();
        //UIManager.Instance.ItemDescription.Enable = true;
    }

    //
    public void OnDrop(PointerEventData eventData)
    {
        UIManager.Instance.DragSlot.Drop(this);
    }

    public bool TooltipHeader(out string leftText, out Color leftColor, out string rightText, out Color rightColor)
    {
        leftText = string.Empty;
        leftColor = default;
        rightText = string.Empty;
        rightColor = default;
        if (Slot.Item == null) return false;

        rightText = Slot.Item.Rarity.ToString();
        rightColor = RarityManager.Instance.RarityColor[Slot.Item.Rarity];
        leftText = Slot.Item.ItemName;
        leftColor = RarityManager.Instance.RarityColor[Slot.Item.Rarity];
    
        return true;
    }

    public bool TooltipBottom(out string text, out Color color)
    {
        text = string.Empty;
        color = default;
        if (Slot.Item == null) return false;

        text = Slot.Item.Description;
        color = Color.black;
        return text != string.Empty;
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    //UIManager.Instance.ItemDescription.Show(ItemData);
    //    TooltipService.Instance.TooltipOpen(this);
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    //UIManager.Instance.ItemDescription.Close();
    //    TooltipService.Instance.TooltipClose();
    //}
}
