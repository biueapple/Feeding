using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PointerGestureRaycaster : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private int hitMask = ~0;
    [SerializeField]
    private float maxDistance = 100;

    [SerializeField]
    private bool blockPointerOverUI = true;

    [SerializeField]
    private float doubleClickInterval = 0.25f;
    [SerializeField]
    private float longPressDuration = 0.5f;
    [SerializeField]
    private float dragStartPixels = 8;

    //길게 누르면 단일 클릭은 발생하지 않음
    [SerializeField]
    private bool consumeClickOnLongPress = true;

    private InputAction pressAction;
    private InputAction posAction;

    //state
    private bool pressing;
    private Vector2 pressScreenPos;
    private float pressTime;
    private bool longPressed;

    private bool dragging;
    private IDraggable dragTarget;
    private RaycastHit dragHitAsStart;

    private IClickable pendingClickTarget;
    private RaycastHit pendingClickHit;
    private float pendingClickTime;
    private bool hasPendingClick;

    private IClickable lastClickTarget;
    private float lastClickTime;
    private RaycastHit lastClickHit;


    private void Awake()
    {
        if (cam == null) cam = Camera.main;

        pressAction = new("PointerPress", InputActionType.Button, "<pointer>/press");
        posAction = new("PointerPos", InputActionType.Value, "<pointer>/position");
    }

    private void OnEnable()
    {
        pressAction.performed += OnPressPerformed;
        pressAction.canceled += OnPressCanceled;
        pressAction.Enable();
        posAction.Enable();
    }

    private void OnDisable()
    {
        pressAction.performed -= OnPressPerformed;
        pressAction.canceled -= OnPressCanceled;
        pressAction.Disable();
        posAction.Disable();
    }

    private void OnPressPerformed(InputAction.CallbackContext ctx)
    {
        pressing = true;
        longPressed = false;
        dragging = false;
        dragTarget = null;

        pressScreenPos = ReadPointer();
        pressTime = Time.unscaledTime;
    }

    private void OnPressCanceled(InputAction.CallbackContext ctx)
    {
        if (!pressing) return;
        pressing = false;
        RaycastHit hit;

        //드래그 중이였다면
        if (dragging)
        {
            dragging = false;
            if(dragTarget != null)
            {
                if (PhysicsRayAt(ReadPointer(), out hit))
                    dragTarget.OnDragEnd(hit);
                else
                    dragTarget.OnDragEnd(default);
            }
            dragTarget = null;
            return;
        }

        //길게 누르는 중
        if (longPressed && consumeClickOnLongPress) return;

        //UI에 가려짐
        var releasePos = ReadPointer();
        if (blockPointerOverUI && IsUIAt(releasePos)) return;

        if(PhysicsRayAt(releasePos, out hit))
        {
            if(hit.collider.TryGetComponent<IClickable>(out var clickable))
            {
                //더블클릭
                if(hasPendingClick && clickable == pendingClickTarget &&
                    (Time.unscaledTime - pendingClickTime) <= doubleClickInterval)
                {
                    hasPendingClick = false;
                    pendingClickTarget = null;
                    clickable.OnDoubleClick(hit);

                    lastClickTarget = clickable;
                    lastClickTime = Time.unscaledTime;
                    lastClickHit = hit;
                }
                else
                {
                    pendingClickTarget = clickable;
                    pendingClickHit = hit;
                    pendingClickTime = Time.unscaledTime;
                    hasPendingClick = true;
                }
            }
        }
    }

    private void Update()
    {
        var now = Time.unscaledTime;

        //유예된 클릭을 확정
        if(hasPendingClick && (now - pendingClickTime) > doubleClickInterval)
        {
            pendingClickTarget?.OnClick(pendingClickHit);
            hasPendingClick = false;
            pendingClickTarget = null;
        }

        if (!pressing) return;

        var currentPos = ReadPointer();

        //길게 누르기
        if(!longPressed && (now - pressTime) >= longPressDuration)
        {
            if(!(blockPointerOverUI && IsUIAt(currentPos)))
            {
                if(PhysicsRayAt(currentPos, out var hit) && hit.collider.TryGetComponent<ILongPressable>(out var lp))
                {
                    lp.OnLongPress(hit);
                    longPressed = true;
                }
            }
        }

        //드래그
        if(!dragging)
        {
            float pixelDist = (currentPos - pressScreenPos).magnitude;
            if(pixelDist >= dragStartPixels)
            {
                if(!(blockPointerOverUI && IsUIAt(pressScreenPos)))
                {
                    if(PhysicsRayAt(pressScreenPos, out var hit) && hit.collider.TryGetComponent<IDraggable>(out var d))
                    {
                        dragging = true;
                        dragTarget = d;
                        dragHitAsStart = hit;
                        d.OnDragStart(hit);
                    }
                }
            }
        }
        else
        {
            //드래그 중
            if(dragTarget != null)
            {
                if (PhysicsRayAt(currentPos, out var hit))
                    dragTarget.OnDrag(hit);
                else
                    dragTarget.OnDrag(default);
            }
        }
    }



    private Vector2 ReadPointer()
    {
        return posAction.ReadValue<Vector2>();
    }

    private bool PhysicsRayAt(Vector2 screenPos, out RaycastHit hit)
    {
        hit = default;
        if (cam == null) return false;
        var ray = cam.ScreenPointToRay(screenPos);
        return Physics.Raycast(ray, out hit, maxDistance, hitMask, QueryTriggerInteraction.Ignore);
    }

    private static bool IsUIAt(Vector2 screenPos)
    {
        if (EventSystem.current == null) return false;
        var data = new PointerEventData(EventSystem.current) { position = screenPos };
        var result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, result);
        return result.Count > 0;
    }
}
