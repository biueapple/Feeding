using System.Collections.Generic;
using UnityEngine;

public class TooltipView : MonoBehaviour
{
    [SerializeField]
    private RectTransform root;
    [SerializeField]
    private ProviderSections sections;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (root == null) root = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        HideImmediate();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        Clear();
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public void Clear()
    {
        if (sections != null)
            sections.Clear();
    }

    private void HideImmediate()
    {
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public RectTransform Root => root;
}
