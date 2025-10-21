using UnityEditor;
using UnityEngine;

public abstract class EquipmentEffect : ScriptableObject
{
    [SerializeField, HideInInspector]
    private string effectID;
    public string EffectID => effectID;

    [SerializeField]
    private string effectName;
    public string EffectName => effectName;

    [SerializeField, TextArea(2, 4)]
    private string description;

    public virtual string BuildDescription(EquipmentEffect effect)
    {
        string s = "{name}  " + description;
        s = s.Replace("{name}", EffectName);
        s = s.Replace("{description}", description);
        return s;
    }
    public abstract void Apply(Unit target);
    public abstract void Remove(Unit target);

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(effectID))
        {
            effectID = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
            Debug.Log($"effect {name} 에 새로운 id {effectID}");
        }
    }
#endif
}
