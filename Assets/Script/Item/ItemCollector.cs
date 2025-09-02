using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemCollecotr")]
public class ItemCollector : ScriptableObject
{
    [SerializeField]
    private List<Item> items = new();
    public IReadOnlyList<Item> Items => items;
    public void AddItem(Item item) { if (items.Contains(item)) return; items.Add(item); }
}
