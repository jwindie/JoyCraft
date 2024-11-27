using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurveGraphic))]
public class BezierCurveEditor : Editor
{
    public float TangentScale = 0.5f;
    private BezierCurveGraphic BezierGraphic;
    private const float HandleSize = 0.05f;
    private const float PickSize = 0.1f;

    private int SelectedPointIndex = -1;
    private int SelectedHandleIndex = -1;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }


    
    void OnSceneGUI()
    {
        if(BezierGraphic == null)
        {
            BezierGraphic = target as BezierCurveGraphic;
        }

        Vector3 p0 = ShowPoint(0);
        Vector3 p1 = ShowTangentHandle(0);
        Vector3 p2 = ShowTangentHandle(1);
        Vector3 p3 = ShowPoint(1);

        Handles.color = Color.gray;
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p2, p3);

        //ShowDirections();
        Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
    }

    private Vector3 ShowPoint(int index)
    {
        Vector3 point = BezierGraphic.transform.TransformPoint(BezierGraphic.Positions[index]);
        float size = HandleUtility.GetHandleSize(point);
        Handles.color = Color.white;

        if (Handles.Button(point, BezierGraphic.transform.rotation, size * HandleSize, size * PickSize, Handles.CircleHandleCap))
        {
            SelectedPointIndex = index;
            SelectedHandleIndex = -1;
        }

        if(SelectedPointIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, BezierGraphic.transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(BezierGraphic, "Move Point");
                EditorUtility.SetDirty(BezierGraphic);
                BezierGraphic.Positions[index] = BezierGraphic.transform.InverseTransformPoint(point);
            }
        }
        
        return point;
    }

    private Vector3 ShowTangentHandle(int index)
    {
        Vector3 point = BezierGraphic.transform.TransformPoint(BezierGraphic.TangentHandles[index]);
        float size = HandleUtility.GetHandleSize(point);
        Handles.color = Color.white;

        if (Handles.Button(point, BezierGraphic.transform.rotation, size * HandleSize, size * PickSize, Handles.CircleHandleCap))
        {
            SelectedHandleIndex = index;
            SelectedPointIndex = -1;
        }

        if (SelectedHandleIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, Quaternion.FromToRotation(Vector3.up, BezierGraphic.GetTangent(index == 0 ? 0f : 1f)));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(BezierGraphic, "Move Point");
                EditorUtility.SetDirty(BezierGraphic);
                BezierGraphic.TangentHandles[index] = BezierGraphic.transform.InverseTransformPoint(point);
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
}
