using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hero : Unit
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float speed = 300;
    [SerializeField]
    private Vector3[] chestRoute;
    [SerializeField]
    private Vector3[] outRoute;
    [SerializeField]
    private Vector3[] inDungeon;
    [SerializeField]
    private Vector3[] tableRoute;


    public override void BasicAttack(Unit target)
    {
        animator.SetTrigger("Attack");
        base.BasicAttack(target);
    }

    //침대에서 일어나기
    public IEnumerator WakeUp()
    {
        yield return new WaitForSeconds(1);
    }

    //상자까지 이동하기
    public IEnumerator MoveToChest()
    {
        yield return Move(chestRoute);
    }

    //집밖으로 나가기
    public IEnumerator OutHome()
    {
        yield return Move(outRoute);
    }

    //던전에 들어가기
    public IEnumerator InDungeon()
    {
        yield return Move(inDungeon);
    }

    public IEnumerator OutDungeon()
    {
        Vector3[] outDungeon = inDungeon.Reverse().ToArray();
        yield return Move(outDungeon);
    }

    public IEnumerator InHome()
    {
        Vector3[] inHome = outRoute.Reverse().ToArray();
        yield return Move(inHome);
    }

    public IEnumerator MoveToTable()
    {
        yield return Move(tableRoute);
    }

    public IEnumerator MoveToBad()
    {
        List<Vector3> list = new()
        {
            transform.localPosition,
            chestRoute[0]
        };
        yield return Move(list.ToArray());
    }

    private IEnumerator Move(Vector3[] route)
    {
        if (route == null || route.Length == 0) yield break;

        transform.localPosition = route[0];
        int index = 1;

        animator.SetBool("Move", true);
        while(index < route.Length)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.localPosition, route[index], Time.deltaTime * speed);
            if (newPosition.x < transform.localPosition.x)
                transform.localScale = new(-1, 1, 1);
            else
                transform.localScale = new(1, 1, 1);

            transform.localPosition = newPosition;
            yield return null;

            if (Vector2.Distance(transform.localPosition, route[index]) < 0.1f)
            {
                index++;
            }
        }
        animator.SetBool("Move", false);
    }

    public void SetAnimationBool(string key, bool b)
    {
        animator.SetBool(key, b);
    }
}
