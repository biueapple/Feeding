using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour, ITooltipProvider
{
    [SerializeField]
    private Image icon;

    private BuffInstance instance;

    public Transform Transform => transform;

    public Vector2 Offset => new(50, 100);

    //���콺�� �ø��� ���� ������ ��
    public void Init(BuffInstance instance)
    {
        this.instance = instance;
        icon.sprite = instance.Buff.Icon;
    }

    //public bool TooltipHeader(out string leftText, out Color leftColor, out string rightText, out Color rightColor)
    //{
    //    leftText = string.Empty;
    //    leftColor = default;
    //    rightText = string.Empty;
    //    rightColor = default;
    //    if (instance == null) return false;

    //    leftColor = Color.black;
    //    rightColor = Color.black;

    //    rightText = instance.Duration.ToString();
    //    leftText = instance.Buff.BuffName;

    //    return true;
    //}

    //public bool TooltipBottom(out string text, out Color color)
    //{
    //    text = string.Empty;
    //    color = default;
    //    if (instance == null) return false;

    //    text = instance.Buff.BuildDescription(instance);
    //    color = Color.red;
    //    return true;
    //}

    public IEnumerable<TooltipElementModel> GetTooltipElements()
    {
        if (instance == null) yield break;

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Header,
            LeftText = instance.Buff.BuffName,
            RightText = instance.Duration.ToString()
        };

        yield return new TooltipElementModel
        { 
            Type = TooltipElementType.KeyValueList,
            Pairs = new(string, string)[]
            {
                ("����", instance.Stacks.ToString()),
                ("������", instance.Caster.name != null ? instance.Caster.name : "�� �� ����")
            }
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Footer,
            Text = instance.Buff.BuildDescription(instance)
        };
    }
}
