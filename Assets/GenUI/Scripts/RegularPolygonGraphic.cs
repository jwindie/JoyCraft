using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegularPolygonGraphic : Image
{
    [Header("Regular Polygon Options")]
    [Range(3, 128)]
    public int Corners = 3;
    public float OffsetAngle;

    public bool Puncture;
    [HideInInspector]
    public float Thickness = 0.5f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        UIVertex vert = UIVertex.simpleVert;

        if(Puncture)
        {
            DrawPuncturedPoly(vh, vert);

        }
        else
        {
            DrawPoly(vh, vert);
        }
    }

    private void DrawPoly(VertexHelper vh, UIVertex vert)
    {
        vert.position = Vector2.zero;
        vert.color = color;
        vert.uv0 = new Vector2(0.5f, 0.5f);
        vh.AddVert(vert);

        for (int i = 0; i <= Corners; i++)
        {
            float angle = Mathf.Deg2Rad * i * 360f / Corners;

            if (type == Type.Filled)
            {
                if (angle > fillAmount * 360f * Mathf.Deg2Rad)
                {
                    angle = fillAmount * 360f * Mathf.Deg2Rad;
                }
            }

            float x = Mathf.Cos(angle + Mathf.Deg2Rad * OffsetAngle);
            float y = Mathf.Sin(angle + Mathf.Deg2Rad * OffsetAngle);

            Vector2 v = Vector2.zero;
            v.x += x * rectTransform.rect.width / 2f;
            v.y += y * rectTransform.rect.width / 2f;

            // When using radial fill, calculate the correct distance from the center to the edge of the polygon
            float mag = Mathf.Cos(Mathf.PI / Corners) / Mathf.Cos(angle % (2f * Mathf.PI / Corners) - Mathf.PI / Corners);
            v = v.normalized * mag * rectTransform.rect.width / 2f;

            Vector2 uv = new Vector2(x / 2f + 0.5f, y / 2f + 0.5f);

            vert.position = v;
            vert.uv0 = uv;
            vh.AddVert(vert);
        }

        for (int t = 1; t <= Corners; t++)
        {
            vh.AddTriangle(0, t, t + 1);
        }
    }

    void DrawPuncturedPoly(VertexHelper vh, UIVertex vert)
    {
        float maxAngle = Mathf.Deg2Rad * 360f + Mathf.Deg2Rad * OffsetAngle;
        if (type == Type.Filled)
        {
            maxAngle = 360f * fillAmount * Mathf.Deg2Rad + Mathf.Deg2Rad * OffsetAngle;
        }

        float radius = rectTransform.rect.width / 2f;
        for (int i = 0; i <= Corners; i++)
        {
            float theta = Mathf.Deg2Rad * 360f * (float)i / Corners;
            theta = Mathf.Clamp(theta, float.MinValue, maxAngle);

            float mag = Mathf.Cos(Mathf.PI / Corners) / Mathf.Cos(theta % (2f * Mathf.PI / Corners) - Mathf.PI / Corners);

            float x_outer = Mathf.Cos(theta + Mathf.Deg2Rad * OffsetAngle) * radius;
            float y_outer = Mathf.Sin(theta + Mathf.Deg2Rad * OffsetAngle) * radius;

            float x_inner = Mathf.Cos(theta + Mathf.Deg2Rad * OffsetAngle) * radius * (1f - Thickness);
            float y_inner = Mathf.Sin(theta + Mathf.Deg2Rad * OffsetAngle) * radius * (1f - Thickness);

            Vector2 uv_outer = new Vector2(Mathf.Cos(theta) / 2f + 0.5f, Mathf.Sin(theta) / 2f + 0.5f);
            Vector2 uv_inner = new Vector2((1f - Thickness) * Mathf.Cos(theta) / 2f + 0.5f, (1f - Thickness) * Mathf.Sin(theta) / 2f + 0.5f);

            vert.position = new Vector2(x_outer, y_outer) * mag;
            vert.uv0 = uv_outer;
            vert.color = color;
            vh.AddVert(vert);

            vert.position = new Vector2(x_inner, y_inner) * mag;
            vert.uv0 = uv_inner;
            vert.color = color;
            vh.AddVert(vert);
        }

        for (int t = 0; t < Corners * 2; t++)
        {
            vh.AddTriangle(t, (t + 1), (t + 2));
            t++;
            vh.AddTriangle(t, (t + 2), (t + 1));
        }
    }

    public void SetCornerCount(int n)
    {
        if(n >= 3)
        {
            Corners = n;
            SetAllDirty();
        }
    }

    public void Refresh()
    {
#if UNITY_EDITOR
        OnValidate();
#endif
    }
}
