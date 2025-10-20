using UnityEngine;
using TMPro;

public class TooltipTextRenderer : TooltipElementRenderer
{
    public override TooltipElementType Type => TooltipElementType.Text;
    [SerializeField]
    private TextMeshProUGUI label;

    public override void Bind(TooltipElementModel model)
    {
        label.text = model.Text ?? string.Empty;
    }
}
