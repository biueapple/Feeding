using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TooltipService : MonoBehaviour
{
    public static TooltipService Instance { get; private set; }

    [SerializeField]
    private TooltipView view;

    [SerializeField]
    private ProviderSections sections;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TooltipOpen(ITooltipProvider provider)
    {
        if (provider == null) return;

        IEnumerable<TooltipElementModel> models = provider.GetTooltipElements();
        if (models == null || !models.Any()) return;
        
        sections.Render(provider.GetTooltipElements());
        view.Show();

        var rect = view.Root;

        rect.position = provider.Transform.position + (Vector3)provider.Offset;

        // 4) (권장) 한 프레임 뒤에 한 번 더 클램프 — TMP 줄바꿈/폰트 fallback 등 대비
        StartCoroutine(ClampNextFrame(rect, provider.Transform.position, provider.Offset));
    }

    public void TooltipClose()
    {
        view.Hide();
        //sections.Clear();
        //view.Hide();
    }

    public void TooltipMove(ITooltipProvider provider)
    {
        view.transform.position = Mouse.current.position.value + provider.Offset;
    }

    private IEnumerator ClampNextFrame(RectTransform rect, Vector3 basePos, Vector2 offset)
    {
        yield return null; // 다음 프레임까지 대기 (레이아웃/텍스트 완전 확정)
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        rect.position = basePos + (Vector3)offset;
        UIManager.Instance.ClampPosition(rect);
    }
}
