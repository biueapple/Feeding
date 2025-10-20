using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//이게 효과가 사라진다던가 할때 쓰여가지고 
public enum BuffKind
{
    StatRise,
    StatDrop,
    DOT_
}

//버프가 언제 tick을 호출하지도 여기서 정해주자
public abstract class Buff : ScriptableObject
{
    [SerializeField, HideInInspector]
    private string buffID;
    public virtual string BuffID => buffID;

    [SerializeField]
    private string buffName;
    public string BuffName => buffName;

    [SerializeField]
    private float duration;                   //지속시간
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

    //reapply는 list인데 apply는 list가 아닌 이유는 reapply는 이미 inst가 만들어진 이후 호출될 수 있지만
    //apply는 반드시 instance가 하나일 때만 호출되기에 list일 이유가 없음
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
            Debug.Log($"buff {name} 에 새로운 id {buffID}");
        }
    }
#endif
}

public class BuffInstance
{
    public readonly Buff Buff;
    public BuffKind Kind => Buff.Kind;
    public readonly Unit Caster;              //이걸 무슨 타입으로 해야할지 모르겠어서 일단은 object
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
