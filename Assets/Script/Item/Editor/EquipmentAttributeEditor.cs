using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(EquipmentAttribute))]    �̰� �־ �������� �ǹ� ���°Ͱ��⵵
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
