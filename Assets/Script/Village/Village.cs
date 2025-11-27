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

    [SerializeField]
    private string emergingMonsterKey = "Emerging Monster";
    [SerializeField]
    private string givenByMonstersKey = "Types Of Items Given By Monsters";
    [SerializeField]
    private string commonItemKey = "Common Item";
    [SerializeField]
    private string lackingItemKey = "Item That Is Lacking";


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
            LeftText = LocalizationManager.Instance.Get(so.VillageNameKey),
            LeftColor = Color.white
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Text,
            Text = LocalizationManager.Instance.Get(emergingMonsterKey)
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Sprite,
            sprites = so.Enemies.Select(x => x.Icon).ToList()
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Text,
            Text = LocalizationManager.Instance.Get(givenByMonstersKey)
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Sprite,
            sprites = GetItems().Select(x => x.Icon).ToList()
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Text,
            Text = LocalizationManager.Instance.Get(commonItemKey)
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Sprite,
            sprites = so.Export.Select(x => x.Icon).ToList()
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Text,
            Text = LocalizationManager.Instance.Get(lackingItemKey)
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Sprite,
            sprites = so.Import.Select(x => x.Icon).ToList()
        };
    }
}
