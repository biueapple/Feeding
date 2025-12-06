using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProviderSections : MonoBehaviour
{
    [System.Serializable]
    public struct Entry { public TooltipElementType type; public TooltipElementRenderer prefab; }

    [SerializeField]
    private RectTransform content;
    [SerializeField]
    private Entry[] registry;

    private readonly Dictionary<TooltipElementType, TooltipElementRenderer> map = new();
    private readonly Dictionary<TooltipElementType, Stack<TooltipElementRenderer>> pool = new();

    private void Awake()
    {
        foreach(var e in registry)
        {
            map[e.type] = e.prefab;
            pool[e.type] = new();
        }
    }

    public RectTransform Rect => (RectTransform)transform;

    public void Clear()
    {
        for(int i = content.childCount - 1; i >= 0; i--)
        {
            var r = content.GetChild(i).GetComponent<TooltipElementRenderer>();
            if(r != null)
            {
                r.OnDespawn();
                r.transform.SetParent(transform, false);
                r.gameObject.SetActive(false);
                pool[r.Type].Push(r);
            }
        }
    }

    public void Render(IEnumerable<TooltipElementModel> elements)
    {
        Clear();

        foreach (var m in elements)
        {
            if (m.Type == TooltipElementType.Sound)
            {
                SoundManager.Instance.Play(m.soundType);
            }
            else
            {
                var r = Spawn(m.Type);
                r.transform.SetParent(content, false);
                r.gameObject.SetActive(true);
                r.Bind(m);
            }
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    private TooltipElementRenderer Spawn(TooltipElementType t)
    {
        if (pool[t].Count > 0) return pool[t].Pop();
        return Instantiate(map[t]);
    }
}
