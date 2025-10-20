using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//�̰� ȿ���� ������ٴ��� �Ҷ� ���������� 
public enum BuffKind
{
    StatRise,
    StatDrop,
    DOT_
}

//������ ���� tick�� ȣ�������� ���⼭ ��������
public abstract class Buff : ScriptableObject
{
    [SerializeField, HideInInspector]
    private string buffID;
    public virtual string BuffID => buffID;

    [SerializeField]
    private string buffName;
    public string BuffName => buffName;

    [SerializeField]
    private float duration;                   //���ӽð�
    public virtual float Duration => duration;

    [SerializeField]
    private BuffKind kind;
    public BuffKind Kind => kind;

    [SerializeField]
    private Sprite icon;
    public Sprite Icon => icon;

    [SerializeField, TextArea(2,4)]
    private string description;
    public virtual string Description { get => description; }
    public virtual string BuildDescription(BuffInstance inst)
    {
        string s = Description;
        s = s.Replace("{name}", BuffName);
        s = s.Replace("{duration}", Mathf.CeilToInt(Mathf.Max(0, inst.Duration)).ToString());
        return s;
    }

    //reapply�� list�ε� apply�� list�� �ƴ� ������ reapply�� �̹� inst�� ������� ���� ȣ��� �� ������
    //apply�� �ݵ�� instance�� �ϳ��� ���� ȣ��Ǳ⿡ list�� ������ ����
    public abstract void Apply(Unit caster, BuffAdministrator target, BuffInstance inst);
    public abstract void Remove(BuffAdministrator administrator, BuffInstance inst);
    public abstract void Reapply(Unit caster, BuffAdministrator target, List<BuffInstance> list);
    public virtual BuffInstance CreateInstance(Unit caster, BuffAdministrator target)
    {
        return new(this, caster, target);
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(buffID))
        {
            buffID = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
            Debug.Log($"buff {name} �� ���ο� id {buffID}");
        }
    }
#endif
}

public class BuffInstance
{
    public readonly Buff Buff;
    public BuffKind Kind => Buff.Kind;
    public readonly Unit Caster;              //�̰� ���� Ÿ������ �ؾ����� �𸣰ھ �ϴ��� object
    public readonly BuffAdministrator Target;
    public float Duration { get; set; }
    public int Stacks { get; private set; } = 0;
    public void AddStack(int amount = 1) => Stacks += amount;
    

    public BuffInstance(Buff buff, Unit caster, BuffAdministrator target)
    {
        if (buff == null || target == null) return;

        Buff = buff;
        Caster = caster;
        Target = target;
        Duration = buff.Duration;
    }

    public bool Tick(float duration)
    {
        Duration -= duration;
        if (Duration < 1) return true;
        return false;
    }
}
