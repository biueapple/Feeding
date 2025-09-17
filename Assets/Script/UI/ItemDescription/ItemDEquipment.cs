using UnityEngine;
using TMPro;
using static UnityEditor.Progress;

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

        // 5) �����̳� ���� �ݿ�(���� ������)
        Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    private float TextSetting(TextMeshProUGUI text, string str)
    {
        // 2) ���� �����̳� ������ Ȯ��
        float width = Rect.rect.width; // Rect �� ����� RectTransform �ʵ�

        // 3) �ؽ�Ʈ�� �ݿ��ϱ� ���� �䱸 ���̸� �����
        Vector2 pref = text.GetPreferredValues(str, width, Mathf.Infinity);
        float neededHeight = pref.y;

        // 4) �ؽ�Ʈ ���� + ��� �޽� ����
        text.text = str;
        text.ForceMeshUpdate();

        // 5) �����̳� ���� �ݿ�(���� ������)
        //Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, neededHeight);
        return neededHeight;
    }
}
