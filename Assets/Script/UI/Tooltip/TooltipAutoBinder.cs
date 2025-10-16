using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipAutoBinder : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField]
    private ITooltipProvider provider;

    private void Awake()
    {
        provider = GetComponent<ITooltipProvider>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipService.Instance.TooltipOpen(provider);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipService.Instance.TooltipClose();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        //TooltipService.Instance.TooltipMove(provider);
    }

    private void OnDisable()
    {
        TooltipService.Instance.TooltipClose();
    }
}


public interface ITooltipProvider
{
    public Transform Transform { get; }
    public Vector2 Offset { get; }
}

public interface ITooltipHeaderProvider : ITooltipProvider
{
    public bool TooltipHeader(out string leftText, out Color leftColor, out string rightText, out Color rightColor);
}

public interface ITooltipBottomProvider : ITooltipProvider
{
    public bool TooltipBottom(out string text, out Color color);
}

//���ʿ� �̸� �����ʿ� ������ ��������
public interface ITooltipKeyValueProvider : ITooltipProvider
{
    public bool TooltipKeyValue(out string key, out Color keyColor, out string value, out Color valueColor);
}

//����Ʈ �������� �����ִ�
public interface ITooltipListProvider : ITooltipProvider
{
    public bool TooltipList(out List<(string, Color)> strings);
}

//�׳� �ؽ�Ʈ�� �����ֱ�
public interface ITooltipSectionsProvider : ITooltipProvider
{
    public bool TooltipSection(out string str, out Color color);
}
