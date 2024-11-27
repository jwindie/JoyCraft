using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParallelogramGraphic : Image
{
    [Range(-1f, 1f)]
    public float Skew;

    public bool Puncture;
    [HideInInspector]
    public float Thickness;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if(Puncture)
        {
            GeneratePuncturedMesh(vh);
        }
        else
        {
            GenerateMesh(vh);
        }
    }

    void GenerateMesh(VertexHelper vh)
    {
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        UIVertex vert = UIVertex.simpleVert;

        Vector2[] corners = new Vector2[4];

        corners[0] = new Vector2(width / 2f, height / 2f);
        corners[1] = new Vector2(-width / 2f, height / 2f);
        corners[2] = new Vector2(-width / 2f, -height / 2f);
        corners[3] = new Vector2(width / 2f, -height / 2f);

        float skewInset = Skew * width * 0.5f;
        if (Skew > 0)
        {
            corners[1].x += skewInset;
            corners[3].x -= skewInset;
        }
        else
        {
            corners[0].x += skewInset;
            corners[2].x -= skewInset;
        }

        // Add verts
        for (int i = 0; i < 4; i++)
        {
            vert.position = corners[i];
            vert.color = color;
            vert.uv0 = new Vector2(0.5f * corners[i].x / (0.5f * width) + 0.5f, 0.5f * corners[i].y / (0.5f * height) + 0.5f);
            vh.AddVert(vert);
        }

        // Create triangles
        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    void GeneratePuncturedMesh(VertexHelper vh)
    {
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        UIVertex vert = UIVertex.simpleVert;

        Vector2[] corners = new Vector2[4];

        // Define outer verts
        corners[0] = new Vector2(width / 2f, height / 2f);
        corners[1] = new Vector2(-width / 2f, height / 2f);
        corners[2] = new Vector2(-width / 2f, -height / 2f);
        corners[3] = new Vector2(width / 2f, -height / 2f);

        float skewInset = Skew * width * 0.5f;
        if (Skew > 0)
        {
            corners[1].x += skewInset;
            corners[3].x -= skewInset;
        }
        else
        {
            corners[0].x += skewInset;
            corners[2].x -= skewInset;
        }

        Vector2[] innerCorners = new Vector2[] {corners[0], corners[1], corners[2], corners[3]};

        float xOffset = Thickness * width / 2f;
        float yOffset = Thickness * height / 2f;

        // Define inner verts
        innerCorners[0] += new Vector2(-xOffset, -yOffset);
        innerCorners[1] += new Vector2(xOffset, -yOffset);
        innerCorners[2] += new Vector2(xOffset, yOffset);
        innerCorners[3] += new Vector2(-xOffset, yOffset);

        if(Skew > 0)
        {
            innerCorners[0].x -= skewInset * Thickness;
            innerCorners[2].x += skewInset * Thickness;
        }
        else
        {
            innerCorners[1].x -= skewInset * Thickness;
            innerCorners[3].x += skewInset * Thickness;
        }

        // Add vertices
        for(int i = 0; i < corners.Length; i++)
        {
            vert.color = color;
            vert.position = innerCorners[i];
            vert.uv0 = new Vector2(0.5f * vert.position.x / (0.5f * width) + 0.5f, 0.5f * vert.position.y / (0.5f * height) + 0.5f);
            vh.AddVert(vert);

            vert.color = color;
            vert.position = corners[i];
            vert.uv0 = new Vector2(0.5f * vert.position.x / (0.5f * width) + 0.5f, 0.5f * vert.position.y / (0.5f * height) + 0.5f);
            vh.AddVert(vert);
        }

        // Create triangles
        for(int t = 0; t < corners.Length * 2 - 2; t += 2)
        {
            vh.AddTriangle(t, t + 1, t + 3);
            vh.AddTriangle(t, t + 3, t + 2);
        }

        vh.AddTriangle(6, 7, 1);
        vh.AddTriangle(6, 1, 0);
    }

    public void Refresh()
    {
#if UNITY_EDITOR
        OnValidate();
#endif
    }
}
