using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffAdministrator : MonoBehaviour
{
    public Unit Owner { get; private set; }

    //���ڴ� �޶��� �� ����
    public event Action<Buff> OnApplyBefore;
    public event Action<Buff> OnApplyAfter;

    //���� �ν��Ͻ��� ������ ��
    public event Action<BuffInstance> OnCreateInstanceAfter;
    //�ν��Ͻ��� ������ ��
    public event Action<BuffInstance> OnDeleteInstanceAfter;

    public event Action<Buff> OnBeforeRemove;
    public event Action<Buff> OnAfterRemove;
    //���� ������� �����ϰ� ��Ʈ ������� ���� ������ ���� ������ �ؾ����� ����ؾ� �ҵ�

    //������
    private readonly Dictionary<string, BuffInstance> buffs = new();
    public IReadOnlyDictionary<string, BuffInstance> Buffs => buffs;

    private readonly Dictionary<BuffInstance, List<IDisposable>> disposable = new(); 

    private void Awake()
    {
        Owner = GetComponent<Unit>();
    }

    //�ν��Ͻ� ���
    private IDisposable Track(BuffInstance inst, IDisposable d)
    {
        if(!disposable.TryGetValue(inst, out var list))
        {
            list = new();
            disposable[inst] = list;
        }
        list.Add(d);
        return d;
    }

    //�ν��Ͻ� ����
    private void DisposeSubscriptions(BuffInstance inst)
    {
        if(disposable.TryGetValue(inst, out var list))
        {
            for(int i  = list.Count - 1; i >= 0; i--)
            {
                list[i]?.Dispose();
            }
            list.Clear();
            disposable.Remove(inst);
        }
    }



    public void ApplyBuff(Buff buff)
    {
        OnApplyBefore?.Invoke(buff);
        if (buffs.TryGetValue(buff.BuffID, out var value))
        {
            value.Buff.Reapply(this, value);
        }
        else
        {
            BuffInstance inst = buff.CreateInstance(this);
            buffs[buff.BuffID] = inst;
            OnCreateInstanceAfter?.Invoke(inst);
            buff.Apply(this, inst);
        }
        OnApplyAfter?.Invoke(buff);
    }

    public void RemoveBuff(Buff buff)
    {
        if (buff == null || !buffs.TryGetValue(buff.BuffID, out var inst)) return;
        
        //�̺�Ʈ
        OnBeforeRemove?.Invoke(buff);

        //���� ����(������ ȿ���� �����ϴ� �̺�Ʈ�� ����)
        DisposeSubscriptions(inst);

        //������ �������� �ؾ��� �ൿ�� ��
        buff.Remove(this, inst);
        OnDeleteInstanceAfter?.Invoke(inst);

        //dic���� ���ֱ�
        buffs.Remove(buff.BuffID);

        //�̺�Ʈ
        OnAfterRemove?.Invoke(buff);
    }


    //
    //�̺�Ʈ ������ ���ִ� �޼ҵ��
    //

    //�㸶��
    public IDisposable SubscribeOnNight(BuffInstance inst, Action action)
    {
        DayCycleManager.Instance.OnNight += action;
        return Track(inst, new ActionDisposable(() => DayCycleManager.Instance.OnNight -= action));
    }

    //���迡�� �����Ҷ�����
    public IDisposable SubscribeOnAfterAttack(BuffInstance inst, Action<AttackEventArgs> args)
    {
        Owner.OnAttackAfter += args;
        return Track(inst, new ActionDisposable(() => Owner.OnAttackAfter -= args));
    }

    //���迡�� 1�ʸ���
    public IDisposable SubscribeOnSecond(BuffInstance inst, Action action)
    {
        AdventureManager.Instance.OnSecond += action;
        return Track(inst, new ActionDisposable(() => AdventureManager.Instance.OnSecond -= action));
    }

    //ȸ���ϱ� ��
    public IDisposable SubscribeOnRecoveryBefore(BuffInstance inst, Action<RecoveryEventArgs> action)
    {
        Owner.OnRecoveryBefore += action;
        return Track(inst, new ActionDisposable(() => Owner.OnRecoveryBefore -= action));
    }
}


public sealed class ActionDisposable : IDisposable
{
    private Action _onDispose;
    public ActionDisposable(Action onDipose) => _onDispose = onDipose;
    public void Dispose()
    {
        _onDispose?.Invoke();
        _onDispose = null;
    }
}