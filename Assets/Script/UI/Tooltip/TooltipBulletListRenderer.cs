using System.Collections.Generic;
using UnityEngine;

public class TooltipBulletListRenderer : TooltipElementRenderer
{
    public override TooltipElementType Type => TooltipElementType.BulletList;

    [SerializeField]
    private RectTransform container;
    [SerializeField]
    private TooltipBulletItem itemPrefab;

    private readonly Stack<TooltipBulletItem> pool = new();
    private readonly List<TooltipBulletItem> active = new();

    public override void Bind(TooltipElementModel model)
    {
        OnDespawn();

        if (model.Items == null) return;
        foreach(var s in model.Items)
        {
            var i = pool.Count > 0 ? pool.Pop() : Instantiate(itemPrefab);
            i.transform.SetParent(container, false);
            i.Set(s);
            active.Add(i);
        }
    }

    public override void OnDespawn()
    {
        foreach(var i in active) { i.gameObject.SetActive(false); pool.Push(i); }
        active.Clear();
    }
}
