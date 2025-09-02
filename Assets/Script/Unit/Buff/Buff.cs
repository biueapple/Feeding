using UnityEditor;
using UnityEngine;

public enum BuffKind
{
    Buff,

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
    public virtual void Tick() { }
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
    private readonly Buff buff;
    public BuffKind Kind => buff.Kind;
    private readonly BuffAdministrator target;
    private float duration;

    public BuffInstance(Buff buff, BuffAdministrator target)
    {
        if (buff == null || target == null) return;

        this.buff = buff;
        this.target = target;
    }

    public void Tick()
    {
        duration--;
        Tick();
        if (duration <= 0)
            buff.Remove(target);
    }
}