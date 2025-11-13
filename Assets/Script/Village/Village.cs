using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Village : MonoBehaviour, ITooltipProvider, IPointerClickHandler
{
    [SerializeField]
    private VillageSO so;

    //몬스터에게서 얻는 아이템 종류
    public List<Item> LootItem => GetItems();

    public Transform Transform => transform;

    public Vector2 Offset => new Vector2(50, 0);

    //현재 발동중인 이벤트나 발동할 수 있는 이벤트 리스트도 필요한가?
    private List<Item> GetItems()
    {
        HashSet<Item> items = new();
        foreach(var enemy in so.Enemies)
        {
            foreach(var e in enemy.LootEntry.entry)
                items.Add(e.item);
        }

        return items.ToList();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        WorldContext.Instance.SetVillage(so);
    }

    public IEnumerable<TooltipElementModel> GetTooltipElements()
    {
        // Header
        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Header,
            LeftText = so.VillageName,
            LeftColor = Color.white
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Text,
            Text = "출현하는 몬스터"
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Sprite,
            sprites = so.Enemies.Select(x => x.Icon).ToList()
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Text,
            Text = "몬스터가 주는 아이템 종류"
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Sprite,
            sprites = GetItems().Select(x => x.Icon).ToList()
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Text,
            Text = "흔한 아이템"
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Sprite,
            sprites = so.Export.Select(x => x.Icon).ToList()
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Text,
            Text = "부족한 아이템"
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Sprite,
            sprites = so.Import.Select(x => x.Icon).ToList()
        };
    }
}
