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

public enum TooltipElementType
{
    Header,
    Text,
    KeyValueList,
    BulletList,
    TowColumn,
    Divider,
    Footer,
    Sprite
}

public sealed class TooltipElementModel
{
    public TooltipElementType Type;

    //°ø¿ë
    public string Text;
    public Sprite Icon;

    //header
    public string LeftText;
    public Color LeftColor;
    public string RightText;
    public Color RightColor;

    //
    public IReadOnlyList<string> Items;
    public IReadOnlyList<(string key, string val)> Pairs;
    public (IReadOnlyList<string> left, IReadOnlyList<string> right) TwoCol;

    public IReadOnlyList<Sprite> sprites;

    public object Data;
}

public interface ITooltipProvider
{
    public Transform Transform { get; }
    public Vector2 Offset { get; }
    IEnumerable<TooltipElementModel> GetTooltipElements();
}
