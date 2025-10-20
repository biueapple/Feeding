using TMPro;
using UnityEngine;

public class ProviderKeyValue : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI key;
    [SerializeField]
    private TextMeshProUGUI value;

    private RectTransform rect;
    public RectTransform Rect { get { if (rect == null) rect = GetComponent<RectTransform>(); return rect; } }

    private void Awake()
    {
        if (rect == null) rect = GetComponent<RectTransform>();
    }

    //public void Setting(ITooltipKeyValueProvider provider, TooltipView tooltipView)
    //{
    //    if (provider.TooltipKeyValue(out string key, out Color keyColor, out string value, out Color valueColor))
    //    {
    //        this.value.text = value;
    //        this.value.color = valueColor;
    //        this.key.text = key;
    //        this.key.color = keyColor;
    //        gameObject.SetActive(true);
    //        tooltipView.Attaching(rect);
    //    }
    //}
}
