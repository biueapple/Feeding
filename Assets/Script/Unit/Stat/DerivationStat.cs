using UnityEditor;
using UnityEngine;


public enum DerivationKind
{
    HP,
    AD,
    AP,
    CC,
    CD,
    AS,
    DEF,
    RES,

}

[CreateAssetMenu(menuName = "Stat/Derivation")]
public class DerivationStat : ScriptableObject
{
    [SerializeField]
    private DerivationKind kind;
    public DerivationKind Kind => kind;


//#if UNITY_EDITOR
//    private void OnValidate()
//    {
//        var collector = AssetDatabase.LoadAssetAtPath<StatCollector>("Assets/Resources/StatCollector.asset");
//        if (collector == null) return;
//        collector.AddDerivationStat(this);
//        EditorUtility.SetDirty(collector);
//    }
//#endif
}