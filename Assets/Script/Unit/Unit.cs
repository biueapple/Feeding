using System;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    private List<StatMapping> stats;

    private readonly Dictionary<FoundationKind, float> foundation = new();
    public IReadOnlyDictionary<FoundationKind, float> Foundation => foundation;

    private readonly Dictionary<DerivationKind, float> derivation = new();
    public IReadOnlyDictionary<DerivationKind, float> Derivaton => derivation;

    private readonly Dictionary<string, StatModifier> statModifiers = new();
    public IReadOnlyDictionary<string, StatModifier> StatModifiers => statModifiers;
    private readonly Dictionary<DerivationKind, float> modifiersValue = new();
    public IReadOnlyDictionary<DerivationKind, float> ModifiersValue => modifiersValue;
    private bool isDirty = false;

    [SerializeField]
    private float currentHP;
    public float CurrentHP { get => currentHP; set { currentHP = Mathf.Max(0, value); } }

    public bool Critical {
        get
        {
            ModifiersCalc();
            var total = DictExt.SumDicts(derivation, modifiersValue);
            float cc = total.GetOrZero(DerivationKind.CC);
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
        StatCollector collector = Resources.Load<StatCollector>("StatCollector");
        foreach (var f in collector.FoundationStats)
        {
            foundation[f.Kind] = 0;
        }
        foreach (var m in stats)
        {
            derivation[m.Derivation.Kind] = m.Figure;
        }
        var total = DictExt.SumDicts(derivation, modifiersValue);
        CurrentHP = total.GetOrZero(DerivationKind.HP);
    }

    private void ModifiersCalc()
    {
        if (!isDirty) return;
        modifiersValue.Clear();
        foreach (var (_, mod) in statModifiers)
        {
            if (!modifiersValue.ContainsKey(mod.Kind)) modifiersValue[mod.Kind] = 0;
            modifiersValue[mod.Kind] += mod.Value;
        }
        isDirty = false;
    }

    public virtual void Attack(Unit target, List<DamagePacket> damages, bool isExtraAttack)
    {
        ModifiersCalc();

        damages ??= new();
        var total = DictExt.SumDicts(derivation, modifiersValue);
        float ad = total.GetOrZero(DerivationKind.AD);
        bool critical = Critical;
        if (critical)
        {
            float cd = total.GetOrZero(DerivationKind.CD);
            ad *= 1.5f + cd;
        }
        damages.Add(new (DamageType.Physical, $"{name}", ad));

        AttackEventArgs args = new(this, target, isExtraAttack, critical);
        args.Damages.AddRange(damages);

        OnBeforeAttack?.Invoke(args);

        target.TakeDamage(args);

        OnAfterAttack?.Invoke(args);
    }

    public void TakeDamage(AttackEventArgs args)
    {
        ModifiersCalc();

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
        statModifiers[id] = modifier;
        isDirty = true;
    }

    public void RemoveStatModifier(string id)
    {
        statModifiers.Remove(id);
        isDirty = true;
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