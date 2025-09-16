using System.Collections.Generic;
using UnityEngine;

public class RarityManager : MonoBehaviour
{
    public static RarityManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Dictionary<ItemRarity, Color> RarityColor = new()
    {
        { ItemRarity.COMMON,    Color.gray },
        { ItemRarity.UNCOMMON,  Color.green },
        { ItemRarity.RARE,      Color.blue },
        { ItemRarity.EPIC,      new Color(0.6f,0,0.8f) }, //º¸¶ó»ö
        { ItemRarity.LEGENDARY, Color.yellow }
    };

    //public ItemRarity RollRarity()
    //{
    //    float r = Random.value;
    //    if (r < eco.legendary) return ItemRarity.LEGENDARY;
    //    else if (r < eco.epic) return ItemRarity.EPIC;
    //    else if (r < eco.rare) return ItemRarity.RARE;
    //    else if (r < eco.uncommon) return ItemRarity.UNCOMMON;
    //    else return ItemRarity.COMMON;
    //}
}
