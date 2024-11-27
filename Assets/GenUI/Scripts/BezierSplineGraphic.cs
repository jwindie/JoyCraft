using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum BezierControlPointMode
{
    Free,
    Aligned,
    Mirrored
}

public class BezierSplineGraphic : LineGraphic
{
    [Header("Bezier Spline Options")]
    [Range(1, 128)]
    public int SmoothingSteps = 30;

    [SerializeField, HideInInspector]
    private BezierControlPointMode[] ControlPointModes = new BezierControlPointMode[4];

    private List<Vector2> CurvePoints = new List<Vector2>();
    

    public int CurveCount
    {
        get
        {
            return (Positions.Count - 1) / 3;
        }
    }

    public Vector3 GetControlPoint(int index)
    {
        return Positions[index];
    }

    

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (UseAnchors)
        {
            if (ShouldRebuild())
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

        if(Positions.Count < 4)
        {
            Positions.Add(Vector2.zero);
            AddCurve();
            return;
        }

        CurvePoints.Clear();
        int steps = SmoothingSteps * CurveCount;
        for (int i = 0; i <= steps; i++)
        {
            Vector3 point = GetPointLocal(i / (float)steps);
            CurvePoints.Add(point);
        }

        GenerateLineMesh(vh, CurvePoints);
    }

    public void AddCurve()
    {
        // Add new positions
        for(int i = 0; i < 3; i++)
        {
            Vector2 point = Positions[Positions.Count - 1];
            point.x += rectTransform.rect.width / 2f;
            point.y += rectTransform.rect.width / 2f;
            Positions.Add(point);
        }

        Array.Resize(ref ControlPointModes, ControlPointModes.Length + 1);
        ControlPointModes[ControlPointModes.Length - 1] = ControlPointModes[ControlPointModes.Length - 2];
        EnforceMode(Positions.Count - 4);
    }

    public void SetControlPoint(int index, Vector2 point)
    {
        if (index % 3 == 0)
        {
            Vector2 delta = point - Positions[index];
            if (index > 0)
            {
                Positions[index - 1] += delta;
            }
            if (index + 1 < Positions.Count)
            {
                Positions[index + 1] += delta;
            }
        }

        Positions[index] = point;
        EnforceMode(index);
    }

    public BezierControlPointMode GetControlPointMode(int index)
    {
        return ControlPointModes[(index + 1) / 3];
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
        ControlPointModes[(index + 1) / 3] = mode;
        EnforceMode(index);
    }

    private void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = ControlPointModes[modeIndex];
        if (mode == BezierControlPointMode.Free || modeIndex == 0 || modeIndex == ControlPointModes.Length - 1)
        {
            return;
        }

        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            enforcedIndex = middleIndex + 1;
        }
        else
        {
            fixedIndex = middleIndex + 1;
            enforcedIndex = middleIndex - 1;
        }

        if (enforcedIndex >= Positions.Count)
        {
            return;
        }

        Vector2 middle = Positions[middleIndex];
        Vector2 enforcedTangent = middle - Positions[fixedIndex];
        
        if (mode == BezierControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector2.Distance(middle, Positions[enforcedIndex]);
        }

        Positions[enforcedIndex] = middle + enforcedTangent;
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        Validate();
    }
#endif

    protected override void Validate()
    {
        // Enforce positions array size
        int m = (Positions.Count - 4) % 3;
        if (m != 0)
        {
            // Strip remaining points
            for (int i = 0; i < m; i++)
            {
                Positions.RemoveAt(Positions.Count - 1);
            }
        }
    }

    public Vector3 GetPoint(float t)
    {
        int i = 0;
        if(t >= 1f)
        {
            t = 1f;
            i = Positions.Count - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }

        return transform.TransformPoint(Bezier.GetPoint(Positions[i], Positions[i + 1], Positions[i + 2], Positions[i + 3], t));
    }

    public Vector3 GetPointLocal(float t)
    {
        int i = 0;
        if (t >= 1f)
        {
            t = 1f;
            i = Positions.Count - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }

        return Bezier.GetPoint(Positions[i], Positions[i + 1], Positions[i + 2], Positions[i + 3], t);
    }

    public Vector3 GetTangent(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = Positions.Count - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }

        return transform.TransformPoint(Bezier.GetFirstDerivative(Positions[i], Positions[i + 1], Positions[i + 2], Positions[i + 3], t)) -
            transform.position;
    }

    
}
