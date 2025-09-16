using UnityEngine;
using TMPro;

public class ItemDEquipment : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text_Level;

    [SerializeField]
    private TextMeshProUGUI text_Stats;            //장착시 올라가는 스탯

    [SerializeField]
    private TextMeshProUGUI text_Part;             //어디에 장착할 수 있는지

    [SerializeField]
    private TextMeshProUGUI text_EquipmentSet;                    //어떤 아이템과 세트인지 (다른 장비 아이템에 같은 equipmentSetSO가 있는지로 알 수 있음)

    [SerializeField]
    private TextMeshProUGUI text_Effects;

    private RectTransform rect;
    public RectTransform Rect { get { if (rect == null) rect = GetComponent<RectTransform>(); return rect; } }

    private void Awake()
    {
        if (rect == null) rect = GetComponent<RectTransform>();
    }

    public void Setting(EquipmentAttribute attr)
    {
        float height = 0;
        
        text_Level.text = attr.Level.ToString();
        height += text_Level.preferredHeight;

        text_Part.text = attr.Part.ToString();
        height += text_Part.preferredHeight;

        text_EquipmentSet.text = attr.EquipmentSet.SetName;
        height += text_EquipmentSet.preferredHeight;

        text_Stats.text = "";
        foreach (var stat in attr.Stats)
        {
            text_Stats.text += stat.Derivation + ": " + stat.Figure + "\n";
        }
        height += text_Stats.preferredHeight;

        text_Effects.text = "";
        foreach (var effect in attr.Effects)
        {
            text_Effects.text += effect.EffectName + "\n";
        }
        height += text_Effects.preferredHeight;
        Rect.sizeDelta = new(Rect.rect.width, height);
    }
}
