using System.Collections.Generic;
using System.Diagnostics;
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
        if (slot.Item != null)
            icon.sprite = slot.Item.Icon;
        else
            icon.sprite = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SoundManager.Instance.Play(SoundType.UIItemPick);
        UIManager.Instance.DragSlot.Begin(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UIManager.Instance.DragSlot.Rect.position = Mouse.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UIManager.Instance.DragSlot.End();
    }

    //
    public void OnDrop(PointerEventData eventData)
    {
        SoundManager.Instance.Play(SoundType.UIItemPut);
        UIManager.Instance.DragSlot.Drop(this);
    }

    public IEnumerable<TooltipElementModel> GetTooltipElements()
    {
        if (Slot.Item == null) yield break;

        // Header
        var col = RarityManager.Instance.RarityColor[Slot.Item.Rarity];
        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Header,
            LeftText = LocalizationManager.Instance.Get(Slot.Item.ItemNameKey),
            LeftColor = col,
            RightText = LocalizationManager.Instance.Get( Slot.Item.Rarity.ToString()),
            RightColor = col
        };

        // 세트명 텍스트
        if (Slot.Item.TryGetAttribute<EquipmentAttribute>(out var eq))
        {
            yield return new TooltipElementModel { Type = TooltipElementType.Text, Text = $"<b>[{LocalizationManager.Instance.Get(eq.EquipmentSet.SetNameKey)}]</b>" };

            // 태그 리스트
            List<string> bullet = new();
            Equipment equipment = GameManager.Instance.Hero.GetComponent<Equipment>();
            int count = equipment.SetCounter.ContainsKey(eq.EquipmentSet) ? equipment.SetCounter[eq.EquipmentSet].count : 0;

            string set2 = LocalizationManager.Instance.Get("2SET");
            foreach (var e in eq.EquipmentSet.TwoSetEffect)
            {
                bullet.Add(set2 + " : " + BuildDescription(e) + $"  ({count})");
            }

            string set4 = LocalizationManager.Instance.Get("4SET");
            foreach (var e in eq.EquipmentSet.FourSetEffect)
            {
                bullet.Add(set4 + " : " + BuildDescription(e) + $"  ({count})");
            }

            yield return new TooltipElementModel { Type = TooltipElementType.BulletList, Items = bullet };

            // 스탯 Key:Value
            var pairs = new List<(string, string)>();
            foreach(var s in eq.Stats)
            {
                pairs.Add((s.Derivation.Kind.ToString(), s.Figure.ToString()));
            }
            if (pairs.Count > 0)
                yield return new TooltipElementModel { Type = TooltipElementType.KeyValueList, Pairs = pairs };
        }




        // Footer (설명)
        string desc = BuildDescription(Slot.Item);
        if (!string.IsNullOrEmpty(desc))
            yield return new TooltipElementModel { Type = TooltipElementType.Footer, Text = desc };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Sound,
            soundType = SoundType.UIItemUp
        };
    }


    public string BuildDescription(Item item)
    {
        string s = LocalizationManager.Instance.Get(item.DescriptionKey);
        string n = LocalizationManager.Instance.Get(item.ItemNameKey);
        string c = LocalizationManager.Instance.Get(item.Category.ToString());
        string r = LocalizationManager.Instance.Get(item.Rarity.ToString());

        s = s.Replace("{name}", n);
        s = s.Replace("{price}", item.Price.ToString());
        s = s.Replace("{category}", c);
        s = s.Replace("{rarity}", r);

        return s;
    }

    public string BuildDescription(EquipmentEffect effect)
    {
        string s = LocalizationManager.Instance.Get(effect.DescriptionKey);
        string n = LocalizationManager.Instance.Get(effect.EffectNameKey);
        Dictionary<string, string> tokens = new();
        effect.CollectTokens(tokens);

        s = s.Replace("{name}", n);

        foreach(var t in tokens)
        {
            s = s.Replace("{" + t.Key + "}", t.Value);
        }

        return s;
    }
}
