using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffAdministrator : MonoBehaviour
{
    public Unit Owner { get; private set; }

    //인자는 달라질 수 있음
    public event Action<Buff> OnApplyBefore;
    public event Action<Buff> OnApplyAfter;

    //버프 인스턴스가 생성될 때
    public event Action<BuffInstance> OnCreateInstanceAfter;
    //인스턴스를 삭제할 때
    public event Action<BuffInstance> OnDeleteInstanceAfter;

    public event Action<BuffInstance> OnBeforeRemove;
    public event Action<BuffInstance> OnAfterRemove;
    //버프 디버프를 통합하고 도트 대미지는 뭔가 수정만 할지 개편을 해야할지 고민해야 할듯

    //버프들
    private readonly Dictionary<string, (Buff so, List<BuffInstance> list)> buffs = new();
    public IReadOnlyDictionary<string, (Buff so, List<BuffInstance> list)> Buffs => buffs;

    private readonly Dictionary<BuffInstance, List<IDisposable>> disposable = new(); 

    private void Awake()
    {
        Owner = GetComponent<Unit>();
    }

    //owner가 죽었을 때 모든 버프를 없앤다던가 하는 OnDisable() OnDestroy() 같은걸 이용해서 만드는것도 생각해 볼 문제

    //인스턴스 등록
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

    //인스턴스 해재
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

    //remove를 id로 overload 하는것도 추천
    public void RemoveBuff(BuffInstance instance)
    {
        if (instance == null || !buffs.TryGetValue(instance.Buff.BuffID, out var value)) return;
        
        //이벤트
        OnBeforeRemove?.Invoke(instance);

        foreach(var inst in value.list)
        {
            //구독 해지(버프의 효과로 존재하는 이벤트를 해지)
            //문제가 하나의 SO에 여러개의 inst가 존재하는 경우 그중 하나가 remove 될때 모든 inst가 해지되는 문제가 
            if(inst == instance)
            {
                DisposeSubscriptions(inst);
                OnDeleteInstanceAfter?.Invoke(inst);
            }
        }

        //버프가 없어질때 해야할 행동을 해
        instance.Buff.Remove(this, instance);

        //dic에서 없애기
        buffs.Remove(instance.Buff.BuffID);

        //이벤트
        OnAfterRemove?.Invoke(instance);
    }


    //
    //이벤트 구독을 해주는 메소드들
    //

    //밤마다
    public IDisposable SubscribeOnNight(BuffInstance inst, Action action)
    {
        DayCycleManager.Instance.OnNight += action;
        return Track(inst, new ActionDisposable(() => DayCycleManager.Instance.OnNight -= action));
    }

    //모험에서 공격할때마다
    public IDisposable SubscribeOnAfterAttack(BuffInstance inst, Action<AttackEventArgs> args)
    {
        Owner.OnAttackAfter += args;
        return Track(inst, new ActionDisposable(() => Owner.OnAttackAfter -= args));
    }

    //모험에서 1초마다
    public IDisposable SubscribeOnSecond(BuffInstance inst, Action action)
    {
        AdventureManager.Instance.OnSecond += action;
        return Track(inst, new ActionDisposable(() => AdventureManager.Instance.OnSecond -= action));
    }

    //회복하기 전
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