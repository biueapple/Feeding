using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class Unit : MonoBehaviour
{
    [SerializeField]
    private Stat stat;

    private readonly Dictionary<DerivationKind, float> derivation = new();
    //public IReadOnlyDictionary<DerivationKind, float> Derivaton => derivation;

    private readonly Dictionary<string, StatModifier> statModifiers = new();
    //public IReadOnlyDictionary<string, StatModifier> StatModifiers => statModifiers;
    private readonly Dictionary<DerivationKind, float> modifiersValue = new();
    //public IReadOnlyDictionary<DerivationKind, float> ModifiersValue => modifiersValue;

    [SerializeField]
    private float currentHP;
    public float CurrentHP { get => currentHP; set { currentHP = Mathf.Max(0, value); } }

    public bool Critical {
        get
        {
            float cc = StatValue(DerivationKind.CC);
            return UnityEngine.Random.Range(1, 101) < cc;
        }
    }

    public int level;

    //공격 전후에 호출하는 이벤트
    public event Action<AttackEventArgs> OnBeforeAttack;
    public event Action<AttackEventArgs> OnAfterAttack;

    //맞기 전후에 호출하는 이벤트
    public event Action<AttackEventArgs> OnBeforeTakeDamage;
    public event Action<AttackEventArgs> OnAfterTakeDamage;

    //피해량 계산 직전
    public event Action<AttackEventArgs> OnCalculateDamage;

    //죽었을때
    public event Action<AttackEventArgs> OnDeath;


    private void Start()
    {
        foreach (var m in stat.Stats)
        {
            derivation[m.Derivation.Kind] = m.Figure;
        }
        CurrentHP = StatValue(DerivationKind.HP);
    }

    public void BasicAttack(Unit target)
    {
        AttackEventArgs args = new(this, target, false);
        Attack(args);
        PerformAttack(args);
    }

    public virtual void Attack(AttackEventArgs args)
    {
        List<DamagePacket> damages = new();
        var total = DictExt.SumDicts(derivation, modifiersValue);
        float ad = total.GetOrZero(DerivationKind.AD);
        float cd = total.GetOrZero(DerivationKind.CD);
        damages.Add(new(DamageType.Physical, $"{name} 의 기본공격", args.IsCritical ? ad * (1 + cd) : ad));
        args.Damages.AddRange(damages);
    }

    public virtual void PerformAttack(AttackEventArgs args)
    {
        OnBeforeAttack?.Invoke(args);

        args.Defender.TakeDamage(args);

        OnAfterAttack?.Invoke(args);
    }

    public void TakeDamage(AttackEventArgs args)
    {
        OnBeforeTakeDamage?.Invoke(args);

        OnCalculateDamage?.Invoke(args);

        var total = DictExt.SumDicts(derivation, modifiersValue);
        foreach (var packet in args.Damages)
        {
            float reduction = packet.type switch
            {
                DamageType.Physical => total.GetOrZero(DerivationKind.DEF),
                DamageType.Magical => total.GetOrZero(DerivationKind.RES),
                _ => 0
            };

            packet.Value = Mathf.Max(1, packet.Value - reduction);
            CurrentHP -= packet.Value;
        }

        OnAfterTakeDamage?.Invoke(args);

        if (currentHP <= 0) OnDeath?.Invoke(args);
    }

    public void AddStatModifier(StatModifier modifier, string id)
    {
        if (statModifiers.ContainsKey(id))
            RemoveStatModifier(id);

        statModifiers[id] = modifier;

        if (modifiersValue.ContainsKey(modifier.Kind))
            modifiersValue[modifier.Kind] += modifier.Value;
    }

    public void RemoveStatModifier(string id)
    {
        if(statModifiers.ContainsKey(id))
        {
            StatModifier modifier = statModifiers[id];
            statModifiers.Remove(id);
            if(modifiersValue.ContainsKey(modifier.Kind))
                modifiersValue[modifier.Kind] -= modifier.Value;
        }
    }

    public float StatValue(DerivationKind kind)
    {
        var total = DictExt.SumDicts(derivation, modifiersValue);
        return total.GetOrZero(kind);
    }
}

public static class DictExt
{
    public static float GetOrZero(this IReadOnlyDictionary<DerivationKind, float> dict, DerivationKind k)
    {
        return dict != null && dict.TryGetValue(k , out var v) ? v : 0f; 
    }

    public static IReadOnlyDictionary<DerivationKind, float> SumDicts(params IReadOnlyDictionary<DerivationKind, float>[] dicts)
    {
        var res = new Dictionary<DerivationKind, float>();
        foreach(var d in dicts)
        {
            if (d == null) continue;
            foreach(var (k,v) in d)
            {
                res[k] = res.TryGetValue(k, out var cur) ? cur + v : v;
            }
        }
        return res;
    }
}