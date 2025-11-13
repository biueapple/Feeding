using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipSpriteRenderer : TooltipElementRenderer
{
    public override TooltipElementType Type => TooltipElementType.Sprite;
    [SerializeField]
    private Image prefab;
    [SerializeField]
    private RectTransform container;


    private readonly Stack<Image> pool = new();
    private readonly List<Image> active = new();

    public override void Bind(TooltipElementModel model)
    {
        OnDespawn();

        if (model.sprites != null)
            foreach (var s in model.sprites) Spawn(container, s);
    }

    public void Spawn(Transform parent, Sprite s)
    {
        var t = pool.Count > 0 ? pool.Pop() : Instantiate(prefab);
        t.transform.SetParent(parent, false);
        t.sprite = s;
        t.gameObject.SetActive(true);
        active.Add(t);
    }

    public override void OnDespawn()
    {
        foreach (var t in active) { t.transform.SetParent(transform, false); t.gameObject.SetActive(false); pool.Push(t); }
        active.Clear();
    }
}
