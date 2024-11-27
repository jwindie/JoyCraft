using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor (typeof (RoundedRectGraphic))]
public class RoundedRectEditor : Editor {
    private RoundedRectGraphic TargetGraphic;

    public override void OnInspectorGUI () {

        if (target != null) {
            TargetGraphic = target as RoundedRectGraphic;
        }

        DrawDefaultInspector ();

        // Show 4 corner radius sliders
        if (TargetGraphic.UseSeparateCornerRadii) {
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck ();
            for (int i = 0 ; i < TargetGraphic.CornerRadii.Length ; i++) {
                TargetGraphic.CornerRadii[i] = EditorGUILayout.FloatField ("Radius" + (i + 1), TargetGraphic.CornerRadii[i]);
                //TargetGraphic.CornerRadii[i] = EditorGUILayout.Slider ("Radius" + (i + 1), TargetGraphic.CornerRadii[i], 0.001f, TargetGraphic.GetMinCornerRadius ());
            }
            if (EditorGUI.EndChangeCheck ()) {
                Undo.RecordObject (TargetGraphic, "Change Corner Radius");
                TargetGraphic.Refresh ();
                EditorUtility.SetDirty (TargetGraphic);
            }
            EditorGUI.indentLevel--;
            GUILayout.Space (10);
        }
        // Show single corner radius slider
        else {
            EditorGUI.BeginChangeCheck ();
            TargetGraphic.CornerRadius = EditorGUILayout.FloatField ("Corner Radius", TargetGraphic.CornerRadius);
            //TargetGraphic.CornerRadius = EditorGUILayout.Slider("Corner Radius", TargetGraphic.CornerRadius, 0.001f, TargetGraphic.GetMinCornerRadius());
            if (EditorGUI.EndChangeCheck ()) {
                Undo.RecordObject (TargetGraphic, "Change Corner Radius");
                TargetGraphic.Refresh ();
                EditorUtility.SetDirty (TargetGraphic);
            }
        }

        EditorGUI.BeginChangeCheck ();
        TargetGraphic.CornerResolution = EditorGUILayout.IntSlider ("Corner Resolution", TargetGraphic.CornerResolution, 0, 128);
        if (EditorGUI.EndChangeCheck ()) {
            Undo.RecordObject (TargetGraphic, "Change corner resolution");
            TargetGraphic.Refresh ();
            EditorUtility.SetDirty (TargetGraphic);
        }

        if (TargetGraphic.Puncture) {
            EditorGUI.BeginChangeCheck ();
            TargetGraphic.ThicknessMultiplier = EditorGUILayout.Slider ("Thickness", TargetGraphic.ThicknessMultiplier, 0f, 1f);
            if (EditorGUI.EndChangeCheck ()) {
                Undo.RecordObject (TargetGraphic, "Change thickness");
                TargetGraphic.Refresh ();
                EditorUtility.SetDirty (TargetGraphic);
            }
        }
    }


}
