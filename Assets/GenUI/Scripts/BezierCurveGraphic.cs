using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BezierCurveGraphic : LineGraphic
{
    public Vector2[] TangentHandles = new Vector2[2];
    private List<Vector2> CurvePoints = new List<Vector2>();

    [Range(1, 128)]
    public int SmoothingSteps;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if(UseAnchors)
        {
            if(ShouldRebuild())
            {
                Positions.Clear();
                foreach (RectTransform rtx in PositionAnchors)
                {
                    if (rtx != null)
                    {
                        Positions.Add(rectTransform.InverseTransformPoint(rtx.position));
                    }
                }
            }
        }

        CurvePoints.Clear();
        for(int i = 0; i <= SmoothingSteps; i++)
        {
            Vector3 point = GetPointLocal(i / (float)SmoothingSteps);
            CurvePoints.Add(point);
        }

        GenerateLineMesh(vh, CurvePoints);
    }

    public Vector3 GetPoint(float t)
    {
        //return transform.TransformPoint(Bezier.GetPoint(Positions[0], Positions[1], Positions[2], Positions[3], t));
        return transform.TransformPoint(Bezier.GetPoint(Positions[0], TangentHandles[0], TangentHandles[1], Positions[1], t));
    }

    public Vector3 GetTangent(float t)
    {
        return transform.TransformPoint(Bezier.GetFirstDerivative(Positions[0], TangentHandles[0], TangentHandles[1], Positions[1], t)) -
            transform.position;
    }

    public Vector3 GetPointLocal(float t)
    {
        return Bezier.GetPoint(Positions[0], TangentHandles[0], TangentHandles[1], Positions[1], t);
    }

    
}

public static class Bezier
{
    // Quadratic
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
    }

    // Quadratic
    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return
            2f * (1f - t) * (p1 - p0) +
            2f * t * (p2 - p1);
    }

    // Cubic
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * oneMinusT * p0 +
            3f * oneMinusT * oneMinusT * t * p1 +
            3f * oneMinusT * t * t * p2 +
            t * t * t * p3;
    }

    // Cubic
    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }
}
