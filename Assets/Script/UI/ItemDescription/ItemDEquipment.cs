using UnityEngine;
using TMPro;

public class ItemDEquipment : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text_Level;

    [SerializeField]
    private TextMeshProUGUI text_Stats;            //������ �ö󰡴� ����

    [SerializeField]
    private TextMeshProUGUI text_Part;             //��� ������ �� �ִ���

    [SerializeField]
    private TextMeshProUGUI text_EquipmentSet;                    //� �����۰� ��Ʈ���� (�ٸ� ��� �����ۿ� ���� equipmentSetSO�� �ִ����� �� �� ����)

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
