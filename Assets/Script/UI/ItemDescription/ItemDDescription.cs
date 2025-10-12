using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemDDescription : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text_Description;
    private RectTransform rect;
    public RectTransform Rect { get { if (rect == null) rect = GetComponent<RectTransform>(); return rect; }  }

    private void Awake()
    {
        if (rect == null) rect = GetComponent<RectTransform>();
    }

    public void Setting(ITooltipBottomProvider provider, TooltipView tooltipView)
    {
        if (!provider.TooltipBottom(out string text, out Color color)) return;

        // 2) ���� �����̳� ������ Ȯ��
        float width = Rect.rect.width; // Rect �� ����� RectTransform �ʵ�

        // 3) �ؽ�Ʈ�� �ݿ��ϱ� ���� �䱸 ���̸� �����
        Vector2 pref = text_Description.GetPreferredValues(text, width, Mathf.Infinity);
        float neededHeight = pref.y;

        // 4) �ؽ�Ʈ ���� + ��� �޽� ����
        text_Description.text = text;
        text_Description.color = color;
        text_Description.ForceMeshUpdate();

        // 5) �����̳� ���� �ݿ�(���� ������)
        Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, neededHeight);
        //text_Description.text = item.Description; 
        //Rect.sizeDelta = new(Rect.rect.width, text_Description.preferredHeight);

        gameObject.SetActive(true);
        tooltipView.Attaching(rect);
    }
}
