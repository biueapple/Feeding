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

    public void Setting(Item item)
    {
        // 2) ���� �����̳� ������ Ȯ��
        float width = Rect.rect.width; // Rect �� ����� RectTransform �ʵ�

        // 3) �ؽ�Ʈ�� �ݿ��ϱ� ���� �䱸 ���̸� �����
        Vector2 pref = text_Description.GetPreferredValues(item.Description, width, Mathf.Infinity);
        float neededHeight = pref.y;

        // 4) �ؽ�Ʈ ���� + ��� �޽� ����
        text_Description.text = item.Description;
        text_Description.ForceMeshUpdate();

        // 5) �����̳� ���� �ݿ�(���� ������)
        Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, neededHeight);
        //text_Description.text = item.Description; 
        //Rect.sizeDelta = new(Rect.rect.width, text_Description.preferredHeight);
    }
}
