using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Unit
{
    //drop
    [SerializeField]
    private LootEntry lootEntry;
    public LootEntry LootEntry => lootEntry;
}


[System.Serializable]
public class LootEntry
{
    public List<LootEntryItem> entry;
    //아이템을 몇개 드랍할지와 그 확률
    public AnimationCurve rootCurve;
    //골드를 얼마나 드랍할지와 그 확률
    public AnimationCurve gold = AnimationCurve.Linear(0, 50, 1, 100);

    public void Loot(out List<Item> items, out int gold)
    {
        float root = Random.value;

        items = new();
        gold = (int)this.gold.Evaluate(root);

        int itemCount = (int)rootCurve.Evaluate(root);

        for(int i = 0; i < itemCount; i++)
        {
            float value = Random.Range(0, entry.Sum(x => x.probability));
            foreach(var item in entry)
            {
                if(item.probability >= value)
                {
                    items.Add(item.item);
                    break;
                }
                value -= item.probability;
            }
        }
    }
}

[System.Serializable]
public class LootEntryItem
{
    public Item item;
    public float probability;
}

//리스트중에 어떤 아이템이 어느 확률인지
//아이템은 중복되게 드랍하는지
//골드는 항상 드랍하는지