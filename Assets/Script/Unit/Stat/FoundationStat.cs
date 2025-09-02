using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum FoundationKind
{
    Strength,
    Agility,
    Intelligence,
    Vitality,
    Wisdom,
    Luck,

}


[CreateAssetMenu(menuName = "Stat/Foundation")]
public class FoundationStat : ScriptableObject
{
    [SerializeField]
    private FoundationKind kind;
    public FoundationKind Kind => kind;
    [SerializeField, TextArea(2, 10)]
    private string description;
    public string Description => description;
    [SerializeField]
    private List<StatMapping> modifiers;
    public IReadOnlyList<StatMapping> Modiriers => modifiers;

#if UNITY_EDITOR
    private void OnValidate()
    {
        var collector = AssetDatabase.LoadAssetAtPath<StatCollector>("Assets/Resources/StatCollector.asset");
        if (collector == null) return;
        collector.AddFoundationStat(this);
        EditorUtility.SetDirty(collector);
    }
#endif
}
