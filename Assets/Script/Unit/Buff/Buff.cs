using UnityEditor;
using UnityEngine;

//이게 효과가 사라진다던가 할때 쓰여가지고 
public enum BuffKind
{
    StatModifier,

}

public abstract class Buff : ScriptableObject
{
    [SerializeField, HideInInspector]
    private string buffID;
    public string BuffID => buffID;

    [SerializeField]
    private string buffName;
    public string BuffName => buffName;

    [SerializeField]
    private float duration;                   //지속시간
    public float Duration => duration;

    [SerializeField]
    private BuffKind kind;
    public BuffKind Kind => kind;

    [SerializeField]
    private Sprite icon;
    public Sprite Icon => icon;

    public abstract void Apply(BuffAdministrator administrator);
    public abstract void Remove(BuffAdministrator administrator);
    public virtual void Tick(BuffAdministrator administrator) { }
    public abstract BuffInstance CreateInstance(BuffAdministrator administrator);

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(buffID))
        {
            buffID = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
            Debug.Log($"buff {name} 에 새로운 id {buffID}");
        }
    }
#endif
}

public class BuffInstance
{
    public readonly Buff Buff;
    public BuffKind Kind => Buff.Kind;
    public readonly BuffAdministrator Target;
    public float Duration { get; private set; }

    public BuffInstance(Buff buff, BuffAdministrator target)
    {
        if (buff == null || target == null) return;

        Buff = buff;
        Target = target;
    }

    public void Tick(float duration)
    {
        Duration -= duration;
        Buff.Tick(Target);
        if (Duration <= 0)
            Buff.Remove(Target);
    }
}