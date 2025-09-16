using UnityEngine;
using TMPro;

public class ItemDHeader : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text_Name;
    [SerializeField]
    private TextMeshProUGUI text_rating;

    private RectTransform rect;
    public RectTransform Rect { get { if (rect == null) rect = GetComponent<RectTransform>(); return rect; } }

    private void Awake()
    {
        if(rect == null) rect = GetComponent<RectTransform>();
    }

    public void Setting(Item item)
    {
        text_Name.text = item.ItemName;
        text_rating.text = item.Rarity.ToString();
        text_rating.color = RarityManager.Instance.RarityColor[item.Rarity];
    }
}
