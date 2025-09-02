#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var item = (Item)target;

        if(item.attributes != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Attributes Details", EditorStyles.boldLabel);

            foreach(var attr in item.attributes)
            {
                if (attr == null) continue;
                
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.ObjectField(attr.name, attr, attr.GetType(), false);

                var so = new SerializedObject(attr);
                var prop = so.GetIterator();

                bool expanded = true;
                if(prop.NextVisible(expanded))
                {
                    do
                    {
                        if (prop.name == "m_Script") continue;  //½ºÅ©¸³Æ® ÇÊµå ¼û±è
                        EditorGUILayout.PropertyField(prop, true);
                    }
                    while (prop.NextVisible(false));
                }

                EditorGUILayout.EndVertical();
            }
        }
    }
}

#endif