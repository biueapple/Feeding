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

    public void Setting(ITooltipHeaderProvider provider, TooltipView tooltipView)
    {
        if(provider.TooltipHeader(out string leftText, out Color leftColor, out string rightText, out Color rightColor))
        {
            text_rating.text = rightText;
            text_rating.color = rightColor;
            text_Name.text = leftText;
            text_Name.color = leftColor;
            gameObject.SetActive(true);
            tooltipView.Attaching(rect);
        }
    }
}
