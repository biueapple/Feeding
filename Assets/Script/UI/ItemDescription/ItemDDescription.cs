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

        // 2) 현재 컨테이너 가로폭 확보
        float width = Rect.rect.width; // Rect 는 당신의 RectTransform 필드

        // 3) 텍스트를 반영하기 전에 요구 높이를 선계산
        Vector2 pref = text_Description.GetPreferredValues(text, width, Mathf.Infinity);
        float neededHeight = pref.y;

        // 4) 텍스트 적용 + 즉시 메쉬 갱신
        text_Description.text = text;
        text_Description.color = color;
        text_Description.ForceMeshUpdate();

        // 5) 컨테이너 높이 반영(수동 사이즈)
        Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, neededHeight);
        //text_Description.text = item.Description; 
        //Rect.sizeDelta = new(Rect.rect.width, text_Description.preferredHeight);

        gameObject.SetActive(true);
        tooltipView.Attaching(rect);
    }
}
