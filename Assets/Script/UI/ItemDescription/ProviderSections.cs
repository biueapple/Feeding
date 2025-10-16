using TMPro;
using UnityEngine;

public class ProviderSections : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI section;

    private RectTransform rect;
    public RectTransform Rect { get { if (rect == null) rect = GetComponent<RectTransform>(); return rect; } }

    private void Awake()
    {
        if (rect == null) rect = GetComponent<RectTransform>();
    }

    public void Setting(ITooltipSectionsProvider provider, TooltipView tooltipView)
    {
        if (provider.TooltipSection(out string str, out Color color))
        {
            section.text = str;
            section.color = color;
            gameObject.SetActive(true);
            tooltipView.Attaching(rect);
        }
    }
}
