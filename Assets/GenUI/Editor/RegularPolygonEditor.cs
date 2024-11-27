using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RegularPolygonGraphic))]
public class RegularPolygonEditor : Editor
{
    RegularPolygonGraphic Target;

    public override void OnInspectorGUI()
    {
        if(target != null)
        {
            Target = target as RegularPolygonGraphic;
        }

        DrawDefaultInspector();

        if(Target.Puncture)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Thickness");
            Undo.RecordObject(Target, "Set Thickness");
            Target.Thickness = EditorGUILayout.Slider(Target.Thickness, 0f, 1f);
            Target.Refresh();
            EditorUtility.SetDirty(Target);
            EditorGUILayout.EndHorizontal();
        }
    }
}
