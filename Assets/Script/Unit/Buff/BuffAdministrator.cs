using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffAdministrator : MonoBehaviour
{
    public Unit Owner { get; private set; }

    //���ڴ� �޶��� �� ����
    public event Action<Buff> OnBeforeApply;
    public event Action<Buff> OnAfterApply;

    public event Action<Buff> OnBeforeRemove;
    public event Action<Buff> OnAfterRemove;
    //���� ������� �����ϰ� ��Ʈ ������� ���� ������ ���� ������ �ؾ����� ����ؾ� �ҵ�

    //������
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
