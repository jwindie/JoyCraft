using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LineGraphic))]
public class LineGraphicEditor : Editor
{

    LineGraphic TargetLine;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (target != null)
        {
            TargetLine = target as LineGraphic;
        }

        if(TargetLine.UseAnchors)
        {
            if(!EditorApplication.isPlaying)
            {
                TargetLine.SetAllDirty();
            }
        }
    }
}
