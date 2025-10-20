using System.Collections.Generic;
using UnityEngine;

public class TooltipKeyValueRenderer : TooltipElementRenderer
{
    public override TooltipElementType Type => TooltipElementType.KeyValueList;

    [SerializeField]
    private RectTransform container;
    [SerializeField]
    private TooltipKeyValueRow rowPrefab;

    private readonly Stack<TooltipKeyValueRow> pool = new();
    private readonly List<TooltipKeyValueRow> active = new();

    public override void Bind(TooltipElementModel model)
    {
        OnDespawn();


        if (model.Pairs == null) return;
        foreach (var (k, v) in model.Pairs)
        {
            var row = pool.Count > 0 ? pool.Pop() : Instantiate(rowPrefab);
            row.transform.SetParent(container, false);
            row.Set(k, v);
            active.Add(row);
        }
    }

    public override void OnDespawn()
    {
        foreach (var r in active) { r.transform.SetParent(transform, false); pool.Push(r); }
        active.Clear();
    }
}
