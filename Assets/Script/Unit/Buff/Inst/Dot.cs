using NUnit.Framework;
using System;
using System.Collections.Generic;
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
[CreateAssetMenu(menuName = "RPG/Debuff/DotType_DOT")]
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
        if (administrator == null || administrator.Owner == null || inst == null)
            return;

        void action()
        {
            Debug.Log($"dot�� ���� ���� {inst.Stacks}");
            administrator.Owner.CurrentHP -= inst.Stacks;
            if (inst.Tick(1))
            {
                Debug.Log("dot ����");
                administrator.RemoveBuff(inst);
            }
        }

        administrator.SubscribeOnSecond(inst, action);
    }

    //���� list�� �������� �� ����
    public override void Reapply(BuffAdministrator administrator, List<BuffInstance> list)
    {
        if (administrator == null || list == null)
            return;

        //Reapply�� ȣ���Ҷ��� CreateInstance ���� �ʰ� ȣ���ϱ⿡ ���� ���� �־��ֱ�
        BuffInstance inst = CreateInstance(administrator);
        Apply(administrator, inst);
        administrator.AddInstance(inst);
        //foreach(var inst in list)
        //{
        //    inst.AddStack(stack);
        //    if (inst.Duration < Duration) inst.Duration = Duration;
        //}
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
