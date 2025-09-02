using System.Collections.Generic;
using UnityEngine;

public class BuffAdministrator : MonoBehaviour
{
    public Unit Owner { get; private set; }

    private readonly Dictionary<string, BuffInstance> dayBuffs = new();
    public IReadOnlyDictionary<string, BuffInstance> Buffs => dayBuffs;

    private readonly Dictionary<string, BuffInstance> timeBuffs = new();
    public IReadOnlyDictionary<string, BuffInstance> TimeBuffs => timeBuffs;

    private void Awake()
    {
        Owner = GetComponent<Unit>();
    }

    public void AddDayBuff(Buff buff)
    {
        buff.Apply(this);
        dayBuffs[buff.BuffID] = buff.CreateInstance(this);
    }

    public void RemoveDayBuff(string buffID)
    {
        dayBuffs.Remove(buffID);
    }

    public void DayTick()
    {
        foreach (var (_, insatnce) in dayBuffs)
        {
            insatnce.Tick();
        }
    }

}
