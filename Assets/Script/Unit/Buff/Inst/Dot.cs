using System;
using UnityEngine;

//� ��Ʈ���� �󸶳� �߰����� ���ٸ� �����
//�� ��Ʈ������� stack�� ������̸� du�� ���ӽð��� �ǹ�
//�̹� �ɷ��ִ� ��Ȳ���� �ٽ� �ɸ��� �Ǹ� du�� �ʱ�ȭ stack�� �״� ���İ� du�� ������ stack�� �״� ���
//Ȥ�� �׳� �� dot�� �߰��ؼ� 2���� ��Ʈ ������� ������ �Ǵ� ��� �̷��� ������ ������ stack�� �״� �����
//�ᱹ du�� ��� �Ұ��̳İ� ����
/* gpt�� �˷��ذ�
| ��å                        | ����                                                                     | ���� ���� ����             |
| --------------------------- | ----------------------------------------------------------------------- | --------------            |
| **A. Stack+Refresh**        | `stacks += x; remaining = baseDuration`                                 | ��/ȭ��(�ð� ���)           |
| **B. Stack+Keep**           | `stacks += x; remaining = max(remaining, minRefresh?)`                  | ����(�ൿƽ)               |
| **C. Stack+Extend(capped)** | `stacks += x; remaining = min(remaining + extendPerApply, maxDuration)` | ȭ��/���� ����              |
| **D. Strongest Wins**       | `stacks,potency,dur = max rule`                                         | ����(���)ó�� ���� ����    |
| **E. Per-Source Merge UI**  | ���� ���� �ν��Ͻ�, ǥ�ô� �ջ�                                           | MMO/ũ���� �߿��� ����       |
*/
//�̷��� ���Ѱ� �ƴϿ��µ� 
//������ �̹� �ɷ��ִ� ���¿��� �ٽ� �ɸ� �̹� �ɷ��ִ� ������� ���� �ް� ������ �ɸ��ٴ���
//���� ���� ���� �̹����� �ִµ� ��������� �����ʿ��� ������ �ִ°�
//������ �ݴ�� ������ ���ú��� ������ʿ�
//ȭ���� �� �߰� ������ �ƿ� ��ø�� �ƴ� �������� ȭ���� �ɸ� �� �ְ� �Ѵٴ���
//�׷��ٸ� ���� ������� �޴����� �����ؾ� �ϴµ�
//������ ���ݸ��� ������� �޵��� �ϴ� ������µ�
//���� �� �ϸ���
//�׷� ������ ȭ����?
//ȭ���� ������ ���� �����ٷ� ����
//������ �� �ϸ��� �޵��� ����
//�ٵ� ���� ����� �� ������ �ϰ����� ���µ�
//�ϴ�� 1�ʸ��ٷ� �ؾ߰ڳ�

//���� �ջ� du�� ������
public class Dot : Buff
{
    public override string BuffID => "DOT";
    [SerializeField]
    protected DamageType type;
    [SerializeField]
    protected int stack;
    public virtual int Stack { get => stack; }

    public override string BuildDescription(BuffInstance inst)
    {
        string s = base.BuildDescription(inst);
        s = s.Replace("{stack}", inst.Stacks.ToString());
        s = s.Replace("{type}", type.ToString());
        return s;
    }

    public override void Apply(BuffAdministrator administrator, BuffInstance inst)
    {
        if (administrator.Owner == null)
            return;

        void action()
        {
            Debug.Log($"dot�� ���� ���� {inst.Stacks}");
            administrator.Owner.CurrentHP -= inst.Stacks;
            if (inst.Tick(1))
            {
                Debug.Log("dot ����");
                administrator.RemoveBuff(this);
            }
        }

        administrator.SubscribeOnSecond(inst, action);
    }

    public override void Reapply(BuffAdministrator administrator, BuffInstance inst)
    {
        if (administrator.Owner == null)
            return;

        inst.AddStack(stack);
        if (inst.Duration < Duration) inst.Duration = Duration;
        
        //void action(AttackEventArgs args)
        //{
        //    Debug.Log($"dot�� ���� ���� {inst.Stacks}");
        //    administrator.Owner.CurrentHP -= inst.Stacks;
        //    if (inst.Tick(1))
        //    {
        //        Debug.Log("dot ����");
        //        administrator.RemoveBuff(this);
        //    }
        //}

        //administrator.SubscribeOnAfterAttack(inst, action);
    }

    public override void Remove(BuffAdministrator administrator, BuffInstance inst)
    {

    }

    public override BuffInstance CreateInstance(BuffAdministrator administrator)
    {
        BuffInstance inst = new(this, administrator)
        { 
            Duration = Duration 
        };
        
        inst.AddStack(stack);
        return inst;
    }
}
