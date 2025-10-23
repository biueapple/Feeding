using UnityEngine;
using UnityEngine.EventSystems;

public class UIDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private bool bringToFrontOnDrag = true;
    [SerializeField]
    private bool clampToParent = false;

    private Vector2 m_Difference;

    private RectTransform rect;
    [SerializeField]
    private Vector2 minOffset;
    [SerializeField]
    private Vector2 maxOffset;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //가장 위로
        if (bringToFrontOnDrag)
            rect.SetAsLastSibling();

        m_Difference = ReadPointerScreenPos() - (Vector2)transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = ReadPointerScreenPos() - m_Difference;
        //clamp
        if (clampToParent)
            UIManager.Instance.ClampPosition(rect, minOffset, maxOffset);
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    private Vector2 ReadPointerScreenPos()
    {
        if (UnityEngine.InputSystem.Pointer.current != null)
            return UnityEngine.InputSystem.Pointer.current.position.ReadValue();
        if (UnityEngine.InputSystem.Mouse.current != null)
            return UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        return Vector2.zero;
    }
}
