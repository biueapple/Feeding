using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(EquipmentAttribute))]    이거 있어도 생각보다 의미 없는것같기도
public class EquipmentAttributeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var equipAttr = (EquipmentAttribute)target;

        if (equipAttr.Effects == null || equipAttr.Effects.Count == 0) return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Effects Preview", EditorStyles.boldLabel);

        foreach(var effect in equipAttr.Effects)
        {
            if (effect == null) continue;
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Effect: {effect.name}");
            EditorGUILayout.LabelField($"Type: {effect.GetType().Name}");
            EditorGUILayout.EndVertical();
        }
    }
}
