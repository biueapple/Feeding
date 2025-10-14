using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour, ITooltipHeaderProvider, ITooltipBottomProvider
{
    [SerializeField]
    private Image icon;

    private BuffInstance instance;

    public Transform Transform => transform;

    public Vector2 Offset => new(50, 100);

    //마우스를 올리면 설명도 보여야 함
    public void Init(BuffInstance instance)
    {
        this.instance = instance;
        icon.sprite = instance.Buff.Icon;
    }

    public bool TooltipHeader(out string leftText, out Color leftColor, out string rightText, out Color rightColor)
    {
        leftText = string.Empty;
        leftColor = default;
        rightText = string.Empty;
        rightColor = default;
        if (instance == null) return false;

        leftColor = Color.black;
        rightColor = Color.black;

        rightText = instance.Duration.ToString();
        leftText = instance.Buff.BuffName;

        return true;
    }

    public bool TooltipBottom(out string text, out Color color)
    {
        text = string.Empty;
        color = default;
        if (instance == null) return false;

        text = instance.Buff.BuildDescription(instance);
        color = Color.red;
        return true;
    }
}
