using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Visitor : Unit
{
    [SerializeField]
    private VisitorSO so;
    public VisitorSO SO { get => so; set { so = value; } }

    [SerializeField]
    private Animator animator;

    public void Init()
    {
        // image.sprite = so.Portrait;
        var overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

        overrideController["Idle"] = so.IdleAnimation;
        overrideController["Walk"] = so.WalkAnimation;

        animator.runtimeAnimatorController = overrideController;
    }

    private Coroutine moveCoroutine;
    public void Move(Vector3 targetPosition, Action onComplete = null)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveCoroutine(targetPosition, onComplete));
    }

    private IEnumerator MoveCoroutine(Vector3 route, Action onComplete)
    {
        animator.Play("Walk");
        while (Vector3.Distance(transform.localPosition, route) > 0.1f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, route, Time.deltaTime * 200);
            yield return null;
        }
        // 정확한 위치로 보정
        transform.localPosition = route;
        animator.Play("Idle");

        // 이동 완료 신호 보냄
        onComplete?.Invoke();
        moveCoroutine = null;
    }
}
