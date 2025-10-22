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
                ("������", instance.Caster switch
                {
                    GameObject go => go.name,
                    Component comp => comp.gameObject.name,
                    _ => "�� �� ����"
                })
            }
        };

        yield return new TooltipElementModel
        {
            Type = TooltipElementType.Footer,
            Text = instance.Buff.BuildDescription(instance)
        };
    }
}
