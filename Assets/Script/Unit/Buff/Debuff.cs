using UnityEngine;

public enum DebuffKind
{
    Dot,
    StatModifier,
}

[CreateAssetMenu(menuName = "RPG/Debuff")]
public abstract class DebuffInt : ScriptableObject
{
    [SerializeField]
    private string debuffName;
    public string DebuffName => debuffName;

    [SerializeField]
    private float duration;                   //지속시간
    public float Duration => duration;

    [SerializeField]
    private DebuffKind kind;
    public DebuffKind Kind => kind;

    public abstract void Apply(Unit unit);
    public abstract void Remove(Unit unit);
}

public class DebuffInstanceInt
{
    private readonly DebuffInt debuff;
    public DebuffKind Kind => debuff.Kind;
    private readonly Unit target;
    private float duration;

    public DebuffInstanceInt(DebuffInt debuff, Unit target)
    {
        if (debuff == null || target == null) return;

        this.debuff = debuff;
        this.target = target;

        Apply();
    }

    public void Apply()
    {
        debuff.Apply(target);
    }
    public void Remove()
    {
        debuff.Remove(target);
    }
    public void Tick()
    {
        duration--;
        if (duration <= 0)
            Remove();
    }
}