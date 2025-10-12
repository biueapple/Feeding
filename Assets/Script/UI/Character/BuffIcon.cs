using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour, ITooltipHeaderProvider, ITooltipBottomProvider
{
    [SerializeField]
    private Image icon;

    private Buff buff;

    public Transform Transform => transform;

    public Vector2 Offset => new(50, 100);

    //마우스를 올리면 설명도 보여야 함
    public void Init(Buff buff)
    {
        this.buff = buff;
        icon.sprite = buff.Icon;
    }

    public bool TooltipHeader(out string leftText, out Color leftColor, out string rightText, out Color rightColor)
    {
        leftText = string.Empty;
        leftColor = default;
        rightText = string.Empty;
        rightColor = default;
        if (buff == null) return false;

        leftColor = Color.black;
        rightColor = Color.black;

        rightText = buff.Duration.ToString();
        leftText = buff.BuffName;

        return true;
    }

    public bool TooltipBottom(out string text, out Color color)
    {
        text = string.Empty;
        color = default;
        if (buff == null) return false;

        text = buff.Description;
        color = Color.red;
        return true;
    }
}
