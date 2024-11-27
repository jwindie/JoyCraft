///////////////////////////////////////////////////////////////
//
// RoundedCornerLine
//
// Created by Nate Platt on 7/30/2020
//
///////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Use this to build lines for line renderers with rounded corners.
/// </summary>
public class RoundedLine : MonoBehaviour
{
    private List<Vector2> _points = new List<Vector2>();
    [SerializeField] private float _cornerRadius = 0.5f;
    [SerializeField] private int _segmentsPerNinetyDegrees = 4;

    [SerializeField] private LineRenderer _lineRenderer;
    private readonly List<Vector3> _lineRendererPoints = new List<Vector3>();

    public void SetPoints(Vector2[] points) {
        _points = points.ToList();
        BuildLine();
    }

    private void BuildLine()
    {
        if (_points.Count == 0) return;

        //convert to worldspace coordinates
        var localToWorld = transform.localToWorldMatrix;
        var prevPoint = _points[0];


        if (_lineRenderer == null)
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        //add point at 0,0
        //this is here so that the first point can actually produce an angle
        //instead, use the connector position given
        _lineRendererPoints.Clear();
        _lineRendererPoints.Add(prevPoint);


        //curve createion algorithm
        //iterate over each point
        for (int i = 1; i < _points.Count - 1; i++)
        {
            //create variables for algorithm
            var point = _points[i];
            var nextPoint = _points[i + 1];
            //directions between the points
            var toNext = nextPoint - point;
            var toPrev = prevPoint - point;

            var limitedCornerRadius = Mathf.Min (Vector2.Distance (point, nextPoint), _cornerRadius);

            //get the angle between the directions
            var cornerAngleDegrees = Vector2.SignedAngle(toNext, toPrev);
            //if angle is too small, dont do anything
            //i presume it avoids errors
            if (Math.Abs(cornerAngleDegrees) < 0.0001f) continue;

            // Split the angle to find where to place the pivot for the rounded corner.
            var halfCornerAngleRadians = cornerAngleDegrees / 2 * Mathf.Deg2Rad;
            var cosAngle = Mathf.Cos(halfCornerAngleRadians);
            var sinAngle = Mathf.Sin(halfCornerAngleRadians);
            // Rotate the toNext vector by half of the corner angle.
            var toPivot = new Vector2(toNext.x * cosAngle - toNext.y * sinAngle,
                                              toNext.x * sinAngle + toNext.y * cosAngle).normalized * limitedCornerRadius;
            var pivot = point + toPivot;
            // Get distance from the first line to the pivot point to know how far to push it so that the circle is tangent to both lines.
            var distanceToPivot = Mathf.Abs((point.y - prevPoint.y) * pivot.x - (point.x - prevPoint.x) * pivot.y + point.x * prevPoint.y - point.y * prevPoint.x) / toPrev.magnitude;
            pivot = point + toPivot * (limitedCornerRadius / distanceToPivot);

            // Rotate toPrev and toNext by +/-90 degrees to get the first and last points of the corner curve.
            Vector2 toCurvePoint;
            if (cornerAngleDegrees < 0)
            {
                toCurvePoint = new Vector2(toPrev.y, -toPrev.x).normalized * limitedCornerRadius;
            }
            else
            {
                toCurvePoint = new Vector2(-toPrev.y, toPrev.x).normalized * limitedCornerRadius;
            }

            _lineRendererPoints.Add(pivot + toCurvePoint);

            // The smaller the angle for the corner, the bigger the turn radius.
            var turnAngleDegrees = Mathf.Sign(cornerAngleDegrees) * 180 - cornerAngleDegrees;

            // Wrap around the circle to build the corner.
            int numSegments = Mathf.CeilToInt(Mathf.Abs(turnAngleDegrees / 90) * _segmentsPerNinetyDegrees);
            var rotation = Matrix4x4.Rotate(Quaternion.AngleAxis(turnAngleDegrees / numSegments, Vector3.forward));

            for (int j = 0; j < numSegments; j++)
            {
                toCurvePoint = rotation.MultiplyPoint(toCurvePoint);

                _lineRendererPoints.Add(pivot + toCurvePoint);
            }
       

            prevPoint = point;
        }
        _lineRendererPoints.Add(_points[_points.Count - 1]);

        _lineRenderer.positionCount = _lineRendererPoints.Count;
        _lineRenderer.SetPositions(_lineRendererPoints.ToArray());

    }
#if UNITY_EDITOR
    private void OnValidate() {
        BuildLine();
    }
#endif
}


// #if UNITY_EDITOR

//     private void OnDrawGizmosSelected()
//     {
//         BuildLine(BuildLineMode.Gizmos);
//     }

//     [CustomEditor(typeof(RoundedLine))]
//     public class CableLineEditor : Editor
//     {
//         public override void OnInspectorGUI()
//         {
//             var cableLine = (RoundedLine)target;

//             bool changed = DrawDefaultInspector();

//             EditorGUILayout.Space();

//             if (GUILayout.Button("Add Point"))
//             {
//                 if (cableLine._points.Count == 0)
//                 {
//                     cableLine._points.Add(Vector2.zero);
//                 }
//                 else
//                 {
//                     cableLine._points.Add(cableLine._points[cableLine._points.Count - 1] + Vector2.right);
//                 }

//                 EditorUtility.SetDirty(cableLine);
//                 changed = true;
//             }

//             if (GUILayout.Button("Force Rebuild"))
//             {
//                 cableLine.RebuildLine();
//             }

//             if (changed)
//             {
//                 cableLine.RebuildLine();
//             }
//         }

//         private void OnSceneGUI()
//         {
//             var cableLine = (RoundedLine)target;

//             var cableLineTransform = cableLine.transform;
//             var snap = new Vector3(0.5f, 0.5f, 0.5f);

//             for (int i = 0; i < cableLine._points.Count; i++)
//             {
//                 var prevPosition = cableLineTransform.localToWorldMatrix.MultiplyPoint(cableLine._points[i]);
//                 var fmh_188_72_638655495124925220 = Quaternion.identity; var newPosition = Handles.FreeMoveHandle(prevPosition, .1f, snap, Handles.DotHandleCap);

//                 // Snap manually because the snap parameter doesn't seem to do anything.
//                 if (newPosition != prevPosition)
//                 {
//                     newPosition.x = Mathf.Round(newPosition.x * 2) / 2;
//                     newPosition.y = Mathf.Round(newPosition.y * 2) / 2;
//                     cableLine._points[i] = cableLineTransform.worldToLocalMatrix.MultiplyPoint(newPosition);
//                     cableLine.RebuildLine();
//                     EditorUtility.SetDirty(cableLine);
//                 }
//             }
//         }
//     }
// #endif