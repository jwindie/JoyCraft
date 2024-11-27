using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ParallelogramGraphic))]
public class ParallelogramEditor : Editor
{
    ParallelogramGraphic TargetGraphic;

    public float Thickness;

    public override void OnInspectorGUI()
    {
        if(target != null)
        {
            TargetGraphic = target as ParallelogramGraphic;
        }

        DrawDefaultInspector();

        if(TargetGraphic.Puncture)
        {
            Undo.RecordObject(TargetGraphic, "Change Thickness");
            TargetGraphic.Thickness = EditorGUILayout.Slider("Thickness", TargetGraphic.Thickness, 0f, 1f);
            TargetGraphic.Refresh();
            EditorUtility.SetDirty(TargetGraphic);
        }
    }
}
