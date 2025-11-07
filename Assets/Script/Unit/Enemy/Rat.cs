using UnityEngine;

public class Rat : Enemy
{
    public override void BasicAttack(Unit target)
    {
        //애니메이션 재생
        animator.SetTrigger("Attack");

        base.BasicAttack(target);
    }

    public override void Hit(AttackEventArgs args)
    {
        //애니메이션 재생
        animator.SetTrigger("Hit");

        base.Hit(args);
    }
}
