using UnityEngine;
using TMPro;

public class TooltipFooterRenderer : TooltipElementRenderer
{
    public override TooltipElementType Type => TooltipElementType.Footer;

    [SerializeField]
    private TextMeshProUGUI label;

    public override void Bind(TooltipElementModel model)
    {
        label.text = model.Text ?? string.Empty;
        if (model.LeftColor != default) label.color = model.LeftColor;
    }
}
