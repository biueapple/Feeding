using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum ItemRarity
{
    COMMON,
    UNCOMMON,
    RARE,
    EPIC,
    LEGENDARY
}

public enum ItemCategory
{
    None,
    Grain,
    Vegetable,
    Fruit,
    Meat,
    Herb,
    CraftedFood,
    Tool,
    Armor,
    Weapon,
    Misc,
}

[CreateAssetMenu(menuName = "Item/Item")]
public class Item : ScriptableObject
{
    [SerializeField, HideInInspector]
    private string itemID;
    public string ItemID => itemID;

    [SerializeField]
    private string itemName;
    public string ItemName => itemName;

    [SerializeField]
    private ItemCategory category;
    public ItemCategory Category => category;

    [SerializeField]
    private ItemRarity rarity;
    public ItemRarity Rarity => rarity;

    [SerializeField]
    private int price;
    public int Price => price;

    [SerializeField]
    private Sprite icon;
    public Sprite Icon => icon;

    [SerializeReference]
    public List<ItemAttribute> attributes;

    public bool TryGetAttribute<T>(out T attr) where T : ItemAttribute
    {
        attr = attributes?.OfType<T>().FirstOrDefault();
        return attr != null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        var collector = AssetDatabase.LoadAssetAtPath<ItemCollector>("Assets/Resources/ItemCollector.asset");
        if (collector == null) return;
        collector.AddItem(this);
        EditorUtility.SetDirty(collector);

        if (string.IsNullOrEmpty(itemID))
        {
            itemID = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log($"itemData {name} 에 새로운 id {itemID}");
        }
    }
#endif
}
