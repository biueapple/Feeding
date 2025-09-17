using UnityEditor;
using UnityEngine;

//이게 효과가 사라진다던가 할때 쓰여가지고 정화로 풀린다던가
public enum DebuffKind
{
    Dot,
    StatModifier,
}


public abstract class Debuff : ScriptableObject
{
    [SerializeField, HideInInspector]
    private string debuffID;
    public string DebuffID => debuffID;

    [SerializeField]
    private string debuffName;
    public string DebuffName => debuffName;

    [SerializeField]
    private float duration;                   //지속시간
    public float Duration => duration;

    [SerializeField]
    private DebuffKind kind;
    public DebuffKind Kind => kind;

    [SerializeField]
    private Sprite icon;
    public Sprite Icon => icon;

    public virtual string Description { get => "{name}\n남은시간: {duration}s\n"; }
    public virtual string BuildDescription(DebuffInstance inst)
    {
        string s = Description;
        s = s.Replace("{name}", DebuffName);
        s = s.Replace("{duration}", Mathf.CeilToInt(Mathf.Max(0, inst.Duration)).ToString());
        return s;
    }

    public abstract void Apply(BuffAdministrator administrator);
    public abstract void Remove(BuffAdministrator administrator);
    public virtual void Tick(BuffAdministrator administrator) { }
    public abstract DebuffInstance CreateInstance(BuffAdministrator administrator);

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(debuffID))
        {
            debuffID = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
            Debug.Log($"buff {name} 에 새로운 id {debuffID}");
        }
    }
#endif
}

public class DebuffInstance
{
    public readonly Debuff Debuff;
    public DebuffKind Kind => Debuff.Kind;
    public readonly BuffAdministrator Target;
    public float Duration { get; private set; }
    public int Stack { get; set; }

    public DebuffInstance(Debuff debuff, BuffAdministrator target)
    {
        if (debuff == null || target == null) return;

        Debuff = debuff;
        Target = target;
    }

    public void Tick(float duration)
    {
        Duration -= duration;
        Debuff.Tick(Target);
        if (duration <= 0)
            Debuff.Remove(Target);
    }
}