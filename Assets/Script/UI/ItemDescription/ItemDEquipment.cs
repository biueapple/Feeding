using UnityEngine;
using TMPro;
using static UnityEditor.Progress;

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

        height += TextSetting(text_Level, attr.Level.ToString());

        height += TextSetting(text_Part, attr.Part.ToString());

        height += TextSetting(text_EquipmentSet, attr.EquipmentSet.SetName);

        string strStat = "";
        foreach (var stat in attr.Stats)
        {
            strStat += stat.Derivation.Kind + ": " + stat.Figure + "\n";
        }
        height += TextSetting(text_Stats, strStat);

        string strEffect = "";
        foreach (var effect in attr.Effects)
        {
            strEffect += effect.EffectName + "\n";
        }
        height += TextSetting(text_Effects, strEffect);

        // 5) 컨테이너 높이 반영(수동 사이즈)
        Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    private float TextSetting(TextMeshProUGUI text, string str)
    {
        // 2) 현재 컨테이너 가로폭 확보
        float width = Rect.rect.width; // Rect 는 당신의 RectTransform 필드

        // 3) 텍스트를 반영하기 전에 요구 높이를 선계산
        Vector2 pref = text.GetPreferredValues(str, width, Mathf.Infinity);
        float neededHeight = pref.y;

        // 4) 텍스트 적용 + 즉시 메쉬 갱신
        text.text = str;
        text.ForceMeshUpdate();

        // 5) 컨테이너 높이 반영(수동 사이즈)
        //Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, neededHeight);
        return neededHeight;
    }
}
