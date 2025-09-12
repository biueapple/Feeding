using System.Collections.Generic;
using UnityEngine;

public class BuffAdministrator : MonoBehaviour
{
    public Unit Owner { get; private set; }

    //버프들
    private readonly Dictionary<string, BuffInstance> dayBuffs = new();
    public IReadOnlyDictionary<string, BuffInstance> Buffs => dayBuffs;

    private readonly Dictionary<string, BuffInstance> timeBuffs = new();
    public IReadOnlyDictionary<string, BuffInstance> TimeBuffs => timeBuffs;

    //디버프들
    private readonly Dictionary<string, BuffInstance> dayDebuffs = new();
    public IReadOnlyDictionary<string, BuffInstance> Debuffs => dayDebuffs;

    private readonly Dictionary<string, BuffInstance> timeDebuffs = new();
    public IReadOnlyDictionary<string, BuffInstance> TimeDebuffs => timeDebuffs;

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
