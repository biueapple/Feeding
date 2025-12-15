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

    public bool Critical
    {
        get
        {
            float cc = StatValue(DerivationKind.CC);
            return UnityEngine.Random.Range(1, 101) < cc;
        }
    }

    public int level;

    //���� ���Ŀ� ȣ���ϴ� �̺�Ʈ
    public event Action<AttackEventArgs> OnAttackBefore;
    public event Action<AttackEventArgs> OnAttackAfter;

    //������ ���� ���Ŀ� ȣ���ϴ� �̺�Ʈ
    public event Action<AttackEventArgs> OnHitBefore;
    public event Action<AttackEventArgs> OnHitAfter;

    //�±� ���Ŀ� ȣ���ϴ� �̺�Ʈ
    public event Action<AttackEventArgs> OnTakeDamageBefore;
    public event Action<AttackEventArgs> OnTakeDamageAfter;

    //���ط� ��� ����
    public event Action<AttackEventArgs> OnCalculateDamage;

    //ȸ�� ���� ��
    public event Action<RecoveryEventArgs> OnHealingBefore;
    public event Action<RecoveryEventArgs> OnHealingAfter;

    //ȸ�� �Ǿ��� ��
    public event Action<RecoveryEventArgs> OnRecoveryBefore;
    public event Action<RecoveryEventArgs> OnRecoveryAfter;

    //�׾�����
    public event Action<AttackEventArgs> OnDeath;

    //ü���� ��ȭ�Ҷ� ȣ��(������� �ްų� ȸ���ϰų�)
    public event Action<Unit> OnChangeHP;


    private void Awake()
    {
        if (stat == null) return;

        foreach (var m in stat.Stats)
        {
            derivation[m.Derivation.Kind] = m.Figure;
        }
        CurrentHP = StatValue(DerivationKind.HP);
        OnChangeHP?.Invoke(this);
    }

    //
    // unit�� ��ɿ� ����
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
        damages.Add(new(DamageType.Physical, $"{name} �� �⺻����", args.IsCritical ? ad * (1 + cd) : ad));
        args.Damages.AddRange(damages);
    }

    public virtual void PerformAttack(AttackEventArgs args)
    {
        OnAttackBefore?.Invoke(args);

        args.Defender.Hit(args);

        OnAttackAfter?.Invoke(args);
    }

    //������ ���� �� ȣ��
    public virtual void Hit(AttackEventArgs args)
    {
        OnHitBefore?.Invoke(args);

        TakeDamage(args);

        OnHitAfter?.Invoke(args);
    }

    //������� ����ؼ� ����
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

    //ȸ�� (�ٸ� ���𰡿��� ȸ���� ����)
    public void Healing(RecoveryEventArgs args)
    {
        OnHealingBefore?.Invoke(args);

        Recovery(args);

        OnHealingAfter?.Invoke(args);
    }

    //ȸ�� ���� (ȸ�� ���� �ʾƵ� ��� ������ ���� �� ��Ȱ�� ü���� ���ٴ��� �ڿ� ȸ���̶����)
    public void Recovery(RecoveryEventArgs args)
    {
        OnRecoveryBefore?.Invoke(args);

        foreach (var pack in args.Recovery)
        {
            CurrentHP += pack.Value;
        }

        OnChangeHP?.Invoke(this);
        OnRecoveryAfter?.Invoke(args);
    }



    //
    //������ ��ȭ
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
        if (statModifiers.ContainsKey(id))
        {
            StatModifier modifier = statModifiers[id];
            statModifiers.Remove(id);
            if (modifiersValue.ContainsKey(modifier.Kind))
                modifiersValue[modifier.Kind] -= modifier.Value;
        }
    }

    //������ ����
    public float StatValue(DerivationKind kind)
    {
        var total = DictExt.SumDicts(derivation, modifiersValue);
        return total.GetOrZero(kind);
    }

    //��Ʈ ������� (������ �������� �����Ϸ� ������ ������ �ٸ� �������̶��� �޶� {�����̳� �����} �����ϱ� ������� ����)
}

//������ ������ ���� �� dic�� ��ġ�� Ŭ����
public static class DictExt
{
    public static float GetOrZero(this IReadOnlyDictionary<DerivationKind, float> dict, DerivationKind k)
    {
        return dict != null && dict.TryGetValue(k, out var v) ? v : 0f;
    }

    public static IReadOnlyDictionary<DerivationKind, float> SumDicts(params IReadOnlyDictionary<DerivationKind, float>[] dicts)
    {
        var res = new Dictionary<DerivationKind, float>();
        foreach (var d in dicts)
        {
            if (d == null) continue;
            foreach (var (k, v) in d)
            {
                res[k] = res.TryGetValue(k, out var cur) ? cur + v : v;
            }
        }
        return res;
    }
}

