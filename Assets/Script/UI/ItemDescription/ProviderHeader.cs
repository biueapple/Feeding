using UnityEngine;
using TMPro;

public class ProviderHeader : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI leftText;
    [SerializeField]
    private TextMeshProUGUI rightText;

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
            this.rightText.text = rightText;
            this.rightText.color = rightColor;
            this.leftText.text = leftText;
            this.leftText.color = leftColor;
            gameObject.SetActive(true);
            tooltipView.Attaching(rect);
        }
    }
}
