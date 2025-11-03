using System;
using System.Collections.Generic;
using UnityEngine;

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
    public event Action<AttackEventArgs> OnAttackBefore;
    public event Action<AttackEventArgs> OnAttackAfter;

    //공격을 받은 전후에 호출하는 이벤트
    public event Action<AttackEventArgs> OnHitBefore;
    public event Action<AttackEventArgs> OnHitAfter;

    //맞기 전후에 호출하는 이벤트
    public event Action<AttackEventArgs> OnTakeDamageBefore;
    public event Action<AttackEventArgs> OnTakeDamageAfter;

    //피해량 계산 직전
    public event Action<AttackEventArgs> OnCalculateDamage;

    //회복 받을 때
    public event Action<RecoveryEventArgs> OnHealingBefore;
    public event Action<RecoveryEventArgs> OnHealingAfter;

    //회복 되었을 때
    public event Action<RecoveryEventArgs> OnRecoveryBefore;
    public event Action<RecoveryEventArgs> OnRecoveryAfter;

    //죽었을때
    public event Action<AttackEventArgs> OnDeath;

    //체력이 변화할때 호출(대미지를 받거나 회복하거나)
    public event Action<Unit> OnChangeHP;


    private void Start()
    {
        foreach (var m in stat.Stats)
        {
            derivation[m.Derivation.Kind] = m.Figure;
        }
        CurrentHP = StatValue(DerivationKind.HP);
        OnChangeHP?.Invoke(this);
    }

    //
    // unit의 기능에 대해
    //

    public virtual void BasicAttack(Unit target)
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
        OnAttackBefore?.Invoke(args);

        args.Defender.Hit(args);

        OnAttackAfter?.Invoke(args);
    }

    //공격을 받을 시 호출
    public virtual void Hit(AttackEventArgs args)
    {
        OnHitBefore?.Invoke(args);

        TakeDamage(args);

        OnHitAfter?.Invoke(args);
    }

    //대미지를 계산해서 적용
    public void TakeDamage(AttackEventArgs args)
    {
        OnTakeDamageBefore?.Invoke(args);

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
        OnChangeHP?.Invoke(this);

        OnTakeDamageAfter?.Invoke(args);

        if (currentHP <= 0) OnDeath?.Invoke(args);
    }

    //회복 (다른 무언가에게 회복을 받음)
    public void Healing(RecoveryEventArgs args)
    {
        OnHealingBefore?.Invoke(args);

        Recovery(args);

        OnHealingAfter?.Invoke(args);
    }

    //회복 적용 (회복 받지 않아도 사용 가능함 예를 들어서 부활해 체력이 찬다던가 자연 회복이라던가)
    public void Recovery(RecoveryEventArgs args)
    {
        OnRecoveryBefore?.Invoke(args);

        foreach(var pack in args.Recovery)
        {
            CurrentHP += pack.Value;
        }

        OnChangeHP?.Invoke(this);
        OnRecoveryAfter?.Invoke(args);
    }



    //
    //스탯의 변화
    //

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

    //스탯의 리턴
    public float StatValue(DerivationKind kind)
    {
        var total = DictExt.SumDicts(derivation, modifiersValue);
        return total.GetOrZero(kind);
    }

    //도트 대미지들 (버프나 디퍼프로 구현하려 했지만 구조가 다른 버프들이랑은 달라서 {스택이나 대미지} 구현하기 어려움이 있음)
}

//스탯의 리턴을 위해 두 dic을 합치는 클래스
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

