using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineGraphic : Image
{
    [Header("UI Line Options")]
    public float Thickness = 1f;
    public List<Vector2> Positions = new List<Vector2>();
    public bool UseAnchors;
    public List<RectTransform> PositionAnchors = new List<RectTransform>();
    public Vector2 UVScale = Vector2.one;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        GenerateLine(vh);
    }

    public virtual bool ShouldRebuild()
    {
        if(UseAnchors)
        {
            // If the number of anchors doesnt match the number of positions, rebuild
            if(PositionAnchors.Count != Positions.Count)
            {
                return true;
            }
            // If any anchor has moved, rebuild
            foreach(RectTransform rtx in PositionAnchors)
            {
                if(!Positions.Contains(transform.InverseTransformPoint(rtx.position)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void AltGenerateLine(VertexHelper vh)
    {
        vh.Clear();

        if (Positions.Count < 2 && !UseAnchors)
        {
            return;
        }

        if(UseAnchors && PositionAnchors.Count < 2)
        {
            return;
        }

        UIVertex vert = UIVertex.simpleVert;

        if (UseAnchors)
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

        List<Vector2> vertsLeft = new List<Vector2>();
        List<Vector2> vertsRight = new List<Vector2>();

        Vector2 lastPos = Positions[0];
        // Add first pair of verts
        {
            Vector2 nextPos = Positions[1];
            Vector3 widthDir3 = Vector3.Cross(Vector3.back, nextPos - lastPos).normalized;

            Vector2 widthDir = new Vector2(widthDir3.x, widthDir3.y);

            Vector2 v0 = lastPos - widthDir * Thickness / 2f + rectTransform.pivot;
            Vector2 v1 = lastPos + widthDir * Thickness / 2f + rectTransform.pivot;

            Vector2 uv = new Vector2(0f, 0f);


            vert.position = v0;
            vert.color = color;
            vert.uv0 = uv;
            vh.AddVert(vert);

            vert.position = v1;
            uv.x = 1f;
            vert.uv0 = uv;
            vh.AddVert(vert);

            vertsLeft.Add(v0);
            vertsRight.Add(v1);
        }


        // Add remaining verts
        for (int i = 1; i < Positions.Count; i++)
        {
            Vector2 pos = Positions[i];
            Vector3 widthDir3 = Vector3.Cross(Vector3.back, pos - lastPos).normalized;

            Vector2 widthDir = new Vector2(widthDir3.x, widthDir3.y);

            Vector2 v0 = pos - widthDir * Thickness / 2f;
            Vector2 v1 = pos + widthDir * Thickness / 2f;

            // Handle corners
            if (i + 1 < Positions.Count)
            {
                Vector2 nextPos = Positions[i + 1];
                Vector2 segmentDir = pos - lastPos;
                float halfAngle = SignedAngle(segmentDir, nextPos - pos, Vector3.forward) / 2f;
                v0 -= segmentDir.normalized * Mathf.Tan(halfAngle * Mathf.Deg2Rad) * Thickness / 2f;
                v1 += segmentDir.normalized * Mathf.Tan(halfAngle * Mathf.Deg2Rad) * Thickness / 2f;
            }
            v0 += rectTransform.pivot;
            v1 += rectTransform.pivot;
            // Calc UVs
            //float uvLengthL = Vector2.Distance(vertices[i * 2 - 2], v0) / totalLength;
            //uvCounterL += uvLengthL * UVScale.y;
            //Vector2 uvL = new Vector2(0f, uvCounterL);

            //float uvLengthR = Vector2.Distance(vertices[i * 2 - 1], v1) / totalLength;
            //uvCounterR += uvLengthR * UVScale.y;
            //Vector2 uvR = new Vector2(1f, uvCounterR);

            //vert.position = v0 + rectTransform.pivot;
            //vert.color = color;
            //vert.uv0 = uvL;
            //vh.AddVert(vert);

            //vert.position = v1 + rectTransform.pivot;
            //vert.uv0 = uvR;
            //vh.AddVert(vert);

            lastPos = pos;

            vertsLeft.Add(v0);
            vertsRight.Add(v1);
        }

        float uvCounterL = 0f;
        float uvCounterR = 0f;
        //float lengthL = GetLength(vertsLeft);
        //float lengthR = GetLength(vertsRight);
        //float totalLength = GetTotalLength();

        // Add vertices and calc uvs
        for (int i = 1; i < Positions.Count; i++)
        {
            float deltaVL = Mathf.Clamp(uvCounterR - uvCounterL, 0f, float.MaxValue) * 2f;
            float deltaVR = Mathf.Clamp(uvCounterL - uvCounterR, 0f, float.MaxValue) * 2f;

            vert.position = vertsLeft[i];
            float uvLengthL = Vector2.Distance(vertsLeft[i - 1], vertsLeft[i]);
            uvCounterL += (uvLengthL) * UVScale.y + deltaVL;
            Vector2 uvL = new Vector2(0f, uvCounterL);
            vert.uv0 = uvL;
            vh.AddVert(vert);

            vert.position = vertsRight[i];
            float uvLengthR = Vector2.Distance(vertsRight[i - 1], vertsRight[i]);
            uvCounterR += (uvLengthR) * UVScale.y + deltaVR;
            Vector2 uvR = new Vector2(1f, uvCounterR);
            vert.uv0 = uvR;
            vh.AddVert(vert);
        }

        // Create triangles
        for (int t = 0; t < Positions.Count * 2 - 3; t += 2)
        {
            vh.AddTriangle(t, t + 1, t + 3);
            vh.AddTriangle(t + 3, t + 2, t);
        }
    }

    public virtual void GenerateLine(VertexHelper vh)
    {

        if (Positions.Count < 2 && !UseAnchors)
        {
            return;
        }

        if (UseAnchors && PositionAnchors.Count < 2)
        {
            return;
        }

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

        GenerateLineMesh(vh, Positions);

    }

    public virtual void GenerateLineMesh(VertexHelper vh, List<Vector2> positions)
    {
        vh.Clear();

        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        List<Vector2> vertsLeft = new List<Vector2>();
        List<Vector2> vertsRight = new List<Vector2>();

        Vector2 lastPos = positions[0];
        // Add first pair of verts
        {
            Vector2 nextPos = positions[1];
            Vector3 widthDir3 = Vector3.Cross(Vector3.back, nextPos - lastPos).normalized;

            Vector2 widthDir = new Vector2(widthDir3.x, widthDir3.y);

            Vector2 v0 = lastPos - widthDir * Thickness / 2f + rectTransform.pivot;
            Vector2 v1 = lastPos + widthDir * Thickness / 2f + rectTransform.pivot;

            vertsLeft.Add(v0);
            vertsRight.Add(v1);
        }


        // Add remaining verts
        for (int i = 1; i < positions.Count; i++)
        {
            Vector2 pos = positions[i];
            Vector3 widthDir3 = Vector3.Cross(Vector3.back, pos - lastPos).normalized;

            Vector2 widthDir = new Vector2(widthDir3.x, widthDir3.y);

            Vector2 v0 = pos - widthDir * Thickness / 2f;
            Vector2 v1 = pos + widthDir * Thickness / 2f;

            // Handle corners
            if (i + 1 < positions.Count)
            {
                Vector2 nextPos = positions[i + 1];
                Vector2 segmentDir = pos - lastPos;
                float halfAngle = SignedAngle(segmentDir, nextPos - pos, Vector3.forward) / 2f;
                v0 -= segmentDir.normalized * Mathf.Tan(halfAngle * Mathf.Deg2Rad) * Thickness / 2f;
                v1 += segmentDir.normalized * Mathf.Tan(halfAngle * Mathf.Deg2Rad) * Thickness / 2f;
            }
            v0 += rectTransform.pivot;
            v1 += rectTransform.pivot;

            lastPos = pos;

            vertsLeft.Add(v0);
            vertsRight.Add(v1);
        }

        float uvCounterL = 0f;
        float uvCounterR = 0f;

        Vector3 lastPosL = vertsLeft[0];
        Vector3 lastPosR = vertsRight[0];

        int tri = 0;
        // Add quads and calc uvs
        for (int i = 0; i < positions.Count - 1; i++)
        {
            float deltaVL = Mathf.Clamp(uvCounterR - uvCounterL, 0f, float.MaxValue) * 2f;
            float deltaVR = Mathf.Clamp(uvCounterL - uvCounterR, 0f, float.MaxValue) * 2f;

            // Add bottom verts of quad
            vert.position = vertsLeft[i];
            uvCounterL += deltaVL;
            Vector2 uvL = new Vector2(0f, uvCounterL);
            vert.uv0 = uvL;
            vh.AddVert(vert);

            vert.position = vertsRight[i];
            uvCounterR += deltaVR;
            Vector2 uvR = new Vector2(1f, uvCounterR);
            vert.uv0 = uvR;
            vh.AddVert(vert);

            // Add top verts of quad
            vert.position = vertsLeft[i + 1];
            float uvLengthL = Vector2.Distance(vertsLeft[i + 1], vertsLeft[i]);
            uvCounterL += uvLengthL * UVScale.y;
            uvL.y = uvCounterL;
            vert.uv0 = uvL;
            vh.AddVert(vert);

            vert.position = vertsRight[i + 1];
            float uvLengthR = Vector2.Distance(vertsRight[i + 1], vertsRight[i]);
            uvCounterR += uvLengthR * UVScale.y;
            uvR.y = uvCounterR;
            vert.uv0 = uvR;
            vh.AddVert(vert);

            // Add tris
            vh.AddTriangle(tri, tri + 1, tri + 3);
            vh.AddTriangle(tri + 3, tri + 2, tri);
            tri += 4;

            lastPosL = vertsLeft[i];
            lastPosR = vertsRight[i];
        }
    }

    public float SignedAngle(Vector3 lhs, Vector3 rhs, Vector3 planeNormal)
    {
        float angle = Vector3.Angle(lhs, rhs);
        Vector3 cross = Vector3.Cross(lhs, rhs);
        if(Vector3.Dot(cross, planeNormal) < 0)
        {
            angle *= -1f;
        }

        return angle;
    }

    public float GetLength(List<Vector2> path)
    {
        float length = 0f;
        for (int i = 0; i < path.Count - 1; i++)
        {
            length += Vector2.Distance(path[i], path[i + 1]);
        }

        return length;
    }

    public float GetTotalLength()
    {
        float length = 0f;
        for(int i = 0; i < Positions.Count - 1; i++)
        {
            length += Vector2.Distance(Positions[i], Positions[i + 1]);
        }

        return length;
    }

    public virtual void Refresh()
    {
#if UNITY_EDITOR
        OnValidate();
#else
        Validate();
#endif
    }

    protected virtual void Validate() {}
}
