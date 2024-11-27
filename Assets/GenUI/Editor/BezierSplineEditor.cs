using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierSplineGraphic))]
public class BezierSplineEditor : Editor
{

    public float TangentScale = 0.5f;
    private BezierSplineGraphic BezierGraphic;
    private const float HandleSize = 0.05f;
    private const float PickSize = 0.1f;

    private int SelectedPointIndex = -1;

    private static Color[] ModeColors = 
    {
        Color.white,
        Color.yellow,
        Color.cyan
    };


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (SelectedPointIndex >= 0 && SelectedPointIndex < BezierGraphic.Positions.Count)
        {
            DrawSelectedPointInspector();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(BezierGraphic, "Add Curve");
            BezierGraphic.AddCurve();
            BezierGraphic.Refresh();
            EditorUtility.SetDirty(BezierGraphic);
        }
    }

    private void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected Point", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();

        EditorGUI.indentLevel++;
        Vector3 point = EditorGUILayout.Vector3Field("Position", BezierGraphic.GetControlPoint(SelectedPointIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(BezierGraphic, "Move Point");
            BezierGraphic.SetControlPoint(SelectedPointIndex, point);
            BezierGraphic.Refresh();
            EditorUtility.SetDirty(BezierGraphic);
        }

        EditorGUI.BeginChangeCheck();
        BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", BezierGraphic.GetControlPointMode(SelectedPointIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(BezierGraphic, "Change Point Mode");
            BezierGraphic.SetControlPointMode(SelectedPointIndex, mode);
            BezierGraphic.Refresh();
            EditorUtility.SetDirty(BezierGraphic);
        }
        EditorGUI.indentLevel--;
    }

    void OnSceneGUI()
    {
        if (BezierGraphic == null)
        {
            BezierGraphic = target as BezierSplineGraphic;
        }

        if(BezierGraphic.Positions.Count < 4)
        {
            return;
        }

        Vector3 p0 = ShowPoint(0);

        for (int i = 1; i < BezierGraphic.Positions.Count; i += 3)
        {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);

            p0 = p3;
        }
    }

    private Vector3 ShowPoint(int index)
    {
        Vector3 point = BezierGraphic.transform.TransformPoint(BezierGraphic.Positions[index]);
        float size = HandleUtility.GetHandleSize(point);
        Handles.color = ModeColors[(int)BezierGraphic.GetControlPointMode(index)];

        if (Handles.Button(point, BezierGraphic.transform.rotation, size * HandleSize, size * PickSize, Handles.CircleHandleCap))
        {
            SelectedPointIndex = index;
            Repaint();
        }

        if (SelectedPointIndex == index)
        {
            EditorGUI.BeginChangeCheck();

            Vector2 tangentDir = BezierGraphic.GetTangent(index / (float)(BezierGraphic.Positions.Count - 1));
            
            // If this is the upper handle of a control point
            if ((index + 1) % 3 == 0)
            {
                tangentDir = BezierGraphic.Positions[index] - BezierGraphic.Positions[index + 1];
            }
            // If this is the lower handle of a control point
            else if((index - 1) % 3 == 0)
            {
                tangentDir = BezierGraphic.Positions[index] - BezierGraphic.Positions[index - 1];
            }
            // Align transform handle up to tangent
            Quaternion tangentRotation = Quaternion.FromToRotation(Vector3.up, tangentDir);
            point = Handles.DoPositionHandle(point, tangentRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(BezierGraphic, "Move Point");
                BezierGraphic.SetControlPoint(index, BezierGraphic.transform.InverseTransformPoint(point));
                BezierGraphic.Refresh();
                EditorUtility.SetDirty(BezierGraphic);
            }
        }

        return point;
    }

    private void ShowDirections()
    {
        Handles.color = Color.green;
        Vector3 point = BezierGraphic.GetPoint(0f);
        Handles.DrawLine(point, point + BezierGraphic.GetTangent(0f) * TangentScale);
        for (int i = 1; i <= BezierGraphic.SmoothingSteps; i++)
        {
            point = BezierGraphic.GetPoint(i / (float)BezierGraphic.SmoothingSteps);
            Handles.DrawLine(point, point + BezierGraphic.GetTangent(i / (float)BezierGraphic.SmoothingSteps) * TangentScale);
        }
    }

    private void ShowDirection(float t)
    {
        Handles.color = Color.green;
        Vector3 point = BezierGraphic.GetPoint(t);
        Handles.DrawLine(point, point + BezierGraphic.GetTangent(t) * TangentScale);
    }
}
