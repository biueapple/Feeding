using UnityEngine;
using TMPro;

public class TooltipHeaderRenderer : TooltipElementRenderer
{
    public override TooltipElementType Type => TooltipElementType.Header;

    [SerializeField]
    private TextMeshProUGUI leftText;
    [SerializeField]
    private TextMeshProUGUI rightText;

    public override void Bind(TooltipElementModel model)
    {
        leftText.text = model.LeftText ?? string.Empty;
        leftText.color = model.LeftColor == default ? Color.white : model.LeftColor;
        rightText.text = model.RightText ?? string.Empty;
        rightText.color = model.RightColor == default ? Color.white : model.RightColor;
    }
}
