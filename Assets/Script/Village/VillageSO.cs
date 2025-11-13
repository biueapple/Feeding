using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Game/Village")]
public class VillageSO : ScriptableObject
{
    [SerializeField]
    private string villageName;
    public string VillageName => villageName;

    [Header("어떤 몬스터가 출현하는지"), SerializeField]
    private List<Enemy> enemies;
    public IReadOnlyList<Enemy> Enemies => enemies;

    [Header("아이템이 거래에 얼마나 등장하는지"), SerializeField]
    private List<Item> export;
    public IReadOnlyList<Item> Export => export;
    [SerializeField]
    private List<Item> import;
    public IReadOnlyList<Item> Import => import;


    [System.Serializable]
    public struct ItemPriceRule
    {
        public Item item;
        public float probability;
        [Range(0, 1)]
        public float price;
    }
}
