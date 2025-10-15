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

    public event Action<BuffInstance> OnBeforeRemove;
    public event Action<BuffInstance> OnAfterRemove;
    //���� ������� �����ϰ� ��Ʈ ������� ���� ������ ���� ������ �ؾ����� ����ؾ� �ҵ�

    //������
    private readonly Dictionary<string, (Buff so, List<BuffInstance> list)> buffs = new();
    public IReadOnlyDictionary<string, (Buff so, List<BuffInstance> list)> Buffs => buffs;

    private readonly Dictionary<BuffInstance, List<IDisposable>> disposable = new(); 

    private void Awake()
    {
        Owner = GetComponent<Unit>();
    }

    //owner�� �׾��� �� ��� ������ ���شٴ��� �ϴ� OnDisable() OnDestroy() ������ �̿��ؼ� ����°͵� ������ �� ����

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
            disposable.Remove(inst);
        }
    }


    public void AddInstance(BuffInstance instance)
    {
        if(!buffs.ContainsKey(instance.Buff.BuffID)) buffs[instance.Buff.BuffID] = new(instance.Buff, new());
        buffs[instance.Buff.BuffID].list.Add(instance);

        OnCreateInstanceAfter?.Invoke(instance);

        instance.Buff.Apply(this, instance);
    }

    public void ApplyBuff(Buff buff)
    {
        OnApplyBefore?.Invoke(buff);
        if (buffs.TryGetValue(buff.BuffID, out var value))
        {
            Buff so = value.so;
            List<BuffInstance> list = value.list;
            so.Reapply(this, list);
        }
        else
        {
            BuffInstance inst = buff.CreateInstance(this);
            AddInstance(inst);
        }
        OnApplyAfter?.Invoke(buff);
    }

    //remove�� id�� overload �ϴ°͵� ��õ
    public void RemoveBuff(BuffInstance instance)
    {
        if (instance == null || !buffs.TryGetValue(instance.Buff.BuffID, out var value)) return;
        
        //�̺�Ʈ
        OnBeforeRemove?.Invoke(instance);

        foreach(var inst in value.list)
        {
            //���� ����(������ ȿ���� �����ϴ� �̺�Ʈ�� ����)
            //������ �ϳ��� SO�� �������� inst�� �����ϴ� ��� ���� �ϳ��� remove �ɶ� ��� inst�� �����Ǵ� ������ 
            if(inst == instance)
            {
                DisposeSubscriptions(inst);
                OnDeleteInstanceAfter?.Invoke(inst);
            }
        }

        //������ �������� �ؾ��� �ൿ�� ��
        instance.Buff.Remove(this, instance);

        //dic���� ���ֱ�
        buffs.Remove(instance.Buff.BuffID);

        //�̺�Ʈ
        OnAfterRemove?.Invoke(instance);
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
    public ActionDisposable(Action onDispose) => _onDispose = onDispose;
    public void Dispose()
    {
        _onDispose?.Invoke();
        _onDispose = null;
    }
}