using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffAdministrator : MonoBehaviour
{
    public Unit Owner { get; private set; }

    //인자는 달라질 수 있음
    public event Action<Buff> OnBeforeApply;
    public event Action<Buff> OnAfterApply;

    public event Action<Buff> OnBeforeRemove;
    public event Action<Buff> OnAfterRemove;
    //버프 디버프를 통합하고 도트 대미지는 뭔가 수정만 할지 개편을 해야할지 고민해야 할듯

    //버프들
    private readonly Dictionary<string, BuffInstance> buffs = new(); 
    public IReadOnlyDictionary<string, BuffInstance> Buffs => buffs;

    private void Awake()
    {
        Owner = GetComponent<Unit>();
    }

    public void ApplyBuff(Buff buff)
    {
        OnBeforeApply?.Invoke(buff);
        if (buffs.TryGetValue(buff.BuffID, out var value))
        {
            value.Buff.Reapply(this, value);
        }
        else
        {
            buffs[buff.BuffID] = buff.CreateInstance(this);
            buff.Apply(this, buffs[buff.BuffID]);
        }
        OnAfterApply?.Invoke(buff);
    }

    public void RemoveBuff(Buff buff)
    {
        if (buff == null) return;
        if (!buffs.TryGetValue(buff.BuffID, out var value)) return;

        OnBeforeRemove?.Invoke(buff);
        buffs[buff.BuffID].Buff.Remove(this, value);
        buffs.Remove(buff.BuffID);
        OnAfterRemove?.Invoke(buff);
    }
}
