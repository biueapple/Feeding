using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TooltipTwoColumnRenderer : TooltipElementRenderer
{
    public override TooltipElementType Type => TooltipElementType.TowColumn;

    [SerializeField]
    private RectTransform leftCol, rightCol;
    [SerializeField]
    private TextMeshProUGUI itemPrefab;

    private readonly Stack<TextMeshProUGUI> pool = new();
    private readonly List<TextMeshProUGUI> active = new();

    public override void Bind(TooltipElementModel model)
    {
        OnDespawn();

        if (model.TwoCol.left != null)
            foreach (var s in model.TwoCol.left) Spawn(leftCol, s);

        if (model.TwoCol.right != null)
            foreach (var s in model.TwoCol.right) Spawn(rightCol, s);
    }

    public void Spawn(Transform parent, string s)
    {
        var t = pool.Count > 0 ? pool.Pop() : Instantiate(itemPrefab);
        t.transform.SetParent(parent, false);
        t.text = s;
        active.Add(t);
    }

    public override void OnDespawn()
    {
        foreach(var t in active) { t.transform.SetParent(transform, false); pool.Push(t); }
        active.Clear();
    }
}
