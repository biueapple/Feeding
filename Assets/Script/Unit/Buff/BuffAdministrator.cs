using System;
using System.Collections.Generic;
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
    private readonly Dictionary<string, BuffInstance> dayBuffs = new();
    public IReadOnlyDictionary<string, BuffInstance> Buffs => dayBuffs;

    private readonly Dictionary<string, BuffInstance> timeBuffs = new();
    public IReadOnlyDictionary<string, BuffInstance> TimeBuffs => timeBuffs;

    //디버프들
    private readonly Dictionary<string, DebuffInstance> dayDebuffs = new();
    public IReadOnlyDictionary<string, DebuffInstance> Debuffs => dayDebuffs;

    private readonly Dictionary<string, DebuffInstance> timeDebuffs = new();
    public IReadOnlyDictionary<string, DebuffInstance> TimeDebuffs => timeDebuffs;

    private void Awake()
    {
        Owner = GetComponent<Unit>();
    }

    public void AddDayBuff(Buff buff)
    {
        buff.Apply(this);
        dayBuffs[buff.BuffID] = buff.CreateInstance(this);
    }

    public void AddDayDebuff(Debuff debuff)
    {
        debuff.Apply(this);
        dayDebuffs[debuff.DebuffID] = debuff.CreateInstance(this);
    }

    public void DayTick()
    {
        foreach (var (_, insatnce) in dayBuffs)
        {
            insatnce.Tick(1);
        }
    }

    public void TimeTick(float dalta)
    {
        foreach(var (_, instance) in timeBuffs)
        {
            instance.Tick(dalta);
        }
    }
}
