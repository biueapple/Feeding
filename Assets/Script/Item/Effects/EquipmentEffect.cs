using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class EquipmentEffect : ScriptableObject
{
    [SerializeField, HideInInspector]
    private string effectID;
    public string EffectID => effectID;

    [SerializeField]
    private string effectNameKey;
    public string EffectNameKey => effectNameKey;

    [SerializeField]
    private string descriptionKey;
    public string DescriptionKey => descriptionKey;

    // 번역/문자열 빌드는 안 하고, 그냥 토큰 값만 채운다
    public abstract void CollectTokens(Dictionary<string, string> tokens);

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
