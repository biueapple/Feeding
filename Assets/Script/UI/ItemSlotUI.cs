using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, ITooltipProvider
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

    //public bool TooltipHeader(out string leftText, out Color leftColor, out string rightText, out Color rightColor)
    //{
    //    leftText = string.Empty;
    //    leftColor = default;
    //    rightText = string.Empty;
    //    rightColor = default;
    //    if (Slot.Item == null) return false;

    //    rightText = Slot.Item.Rarity.ToString();
    //    rightColor = RarityManager.Instance.RarityColor[Slot.Item.Rarity];
    //    leftText = Slot.Item.ItemName;
    //    leftColor = RarityManager.Instance.RarityColor[Slot.Item.Rarity];
    
    //    return true;
    //}

    //public bool TooltipKeyValue(out string key, out Color keyColor, out string value, out Color valueColor)
    //{
    //    key = string.Empty;
    //    keyColor = default;
    //    value = string.Empty;
    //    valueColor = default;
    //    if (Slot.Item == null) return false;

    //    //���������� �ƴϴ��� ��� ����
    //    if(Slot.Item.TryGetAttribute<EquipmentAttribute>(out var equipment))
    //    {
    //        key = equipment.Level.ToString();
    //        keyColor = Color.white;
    //        value = equipment.Part.ToString();
    //        valueColor = Color.white;
    //        return true;
    //    }

    //    return false;
    //}

    //public bool TooltipSection(out string str, out Color color)
    //{
    //    str = string.Empty;
    //    color = default;
    //    if (Slot.Item == null) return false;

    //    //�Ƹ� �������۸� ���������?
    //    if(Slot.Item.TryGetAttribute<EquipmentAttribute>(out var equipment))
    //    {
    //        str = equipment.EquipmentSet.SetName;
    //        color = Color.white;
    //        return true;
    //    }

    //    return false;
    //}

    //public bool TooltipBottom(out string text, out Color color)
    //{
    //    text = string.Empty;
    //    color = default;
    //    if (Slot.Item == null) return false;

    //    text = Slot.Item.Description;
    //    color = Color.black;
    //    return text != string.Empty;
    //}

    public IEnumerable<TooltipElementModel> GetTooltipElements()
    {
        if (Slot.Item == null) yield break;

        // Header
        var col = RarityManager.Instance.RarityColor[Slot.Item.Rarity];
        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Header,
            LeftText = Slot.Item.ItemName,
            LeftColor = col,
            RightText = Slot.Item.Rarity.ToString(),
            RightColor = col
        };

        // ��Ʈ�� �ؽ�Ʈ
        if (Slot.Item.TryGetAttribute<EquipmentAttribute>(out var eq))
        {
            yield return new TooltipElementModel { Type = TooltipElementType.Text, Text = $"<b>[{eq.EquipmentSet.SetName}]</b>" };

            // �±� ����Ʈ
            List<string> bullet = new();
            foreach(var e in eq.EquipmentSet.TwoSetEffect)
            {
                bullet.Add("2��Ʈ : " + e.BuildDescription(e));
            }
            
                yield return new TooltipElementModel { Type = TooltipElementType.BulletList, Items = bullet };

            // ���� Key:Value
            var pairs = new List<(string, string)>();
            foreach(var s in eq.Stats)
            {
                pairs.Add((s.Derivation.Kind.ToString(), s.Figure.ToString()));
            }
            if (pairs.Count > 0)
                yield return new TooltipElementModel { Type = TooltipElementType.KeyValueList, Pairs = pairs };
        }




        // Footer (����)
        if (!string.IsNullOrEmpty(Slot.Item.Description))
            yield return new TooltipElementModel { Type = TooltipElementType.Footer, Text = Slot.Item.Description };
    }
}
