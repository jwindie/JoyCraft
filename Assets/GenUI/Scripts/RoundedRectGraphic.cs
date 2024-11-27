using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RoundedRectGraphic : Image {
    [HideInInspector]
    public float CornerRadius;
    [HideInInspector]
    public float[] CornerRadii = new float[4];
    [HideInInspector]
    public int CornerResolution;
    [Header ("Rounded Rect Options")]
    public bool Puncture;
    public bool UseSeparateCornerRadii;

    [HideInInspector]
    public float Thickness;
    [HideInInspector]
    public float ThicknessMultiplier;

    private Dictionary<Vector2, int> IndicesByVertex;

    protected override void OnPopulateMesh (VertexHelper vh) {
        vh.Clear ();

        if (Puncture) {
            GeneratePuncturedMesh (vh);
        }
        else {
            GenerateMesh (vh);
        }
    }

    public float GetMinCornerRadius () {
        float minLength = Mathf.Min (rectTransform.rect.width, rectTransform.rect.height) / 2f - 0.001f;
        return minLength;
    }

    public void GeneratePuncturedMesh (VertexHelper vh) {
        float clampedCornerRadius = Mathf.Clamp (CornerRadius, 0.001f, GetMinCornerRadius ());
        float[] clampedCornerRadii = new float[] {
            Mathf.Clamp (CornerRadii[0], 0.001f, GetMinCornerRadius ()),
            Mathf.Clamp (CornerRadii[1], 0.001f, GetMinCornerRadius ()),
            Mathf.Clamp (CornerRadii[2], 0.001f, GetMinCornerRadius ()),
            Mathf.Clamp (CornerRadii[3], 0.001f, GetMinCornerRadius ()),
        };

        IndicesByVertex = new Dictionary<Vector2, int> ();

        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        Vector2[] exteriorCornerCenters = new Vector2[4];

        float xOffset = (width / 2f - clampedCornerRadius);
        float yOffset = (height / 2f - clampedCornerRadius);

        Thickness = ThicknessMultiplier * GetMinCornerRadius ();

        // Create center points of external rounded corners starting from top right and moving counter clockwise
        exteriorCornerCenters[0] = new Vector2 (xOffset, yOffset);
        exteriorCornerCenters[1] = new Vector2 (-xOffset, yOffset);
        exteriorCornerCenters[2] = new Vector2 (-xOffset, -yOffset);
        exteriorCornerCenters[3] = new Vector2 (xOffset, -yOffset);

        if (UseSeparateCornerRadii) {

            xOffset = width / 2f;
            yOffset = height / 2f;

            exteriorCornerCenters[0] = new Vector2 ((xOffset - clampedCornerRadii[0]), (yOffset - clampedCornerRadii[0]));
            exteriorCornerCenters[1] = new Vector2 (-(xOffset - clampedCornerRadii[1]), (yOffset - clampedCornerRadii[1]));
            exteriorCornerCenters[2] = new Vector2 (-(xOffset - clampedCornerRadii[2]), -(yOffset - clampedCornerRadii[2]));
            exteriorCornerCenters[3] = new Vector2 ((xOffset - clampedCornerRadii[3]), -(yOffset - clampedCornerRadii[3]));
        }

        Vector2[] interiorCornerCenters = new Vector2[4];

        //interiorCornerCenters[0] = exteriorCornerCenters[0] + new Vector2(-Thickness, -Thickness);
        //interiorCornerCenters[1] = exteriorCornerCenters[1] + new Vector2(Thickness, -Thickness);
        //interiorCornerCenters[2] = exteriorCornerCenters[2] + new Vector2(Thickness, Thickness);
        //interiorCornerCenters[3] = exteriorCornerCenters[3] + new Vector2(-Thickness, Thickness);

        interiorCornerCenters[0] = exteriorCornerCenters[0];
        interiorCornerCenters[1] = exteriorCornerCenters[1];
        interiorCornerCenters[2] = exteriorCornerCenters[2];
        interiorCornerCenters[3] = exteriorCornerCenters[3];

        // Generate interior and exterior vertices
        List<Vector2> exteriorVerts = new List<Vector2> ();
        List<Vector2> interiorVerts = new List<Vector2> ();

        for (int c = 0 ; c < 4 ; c++) {
            float radius = clampedCornerRadius;
            if (UseSeparateCornerRadii) {
                radius = clampedCornerRadii[c];
            }
            exteriorVerts.AddRange (GenerateCornerVerts (exteriorCornerCenters[c], radius, 90f * c, CornerResolution));
            interiorVerts.AddRange (GenerateCornerVerts (interiorCornerCenters[c], radius - Thickness, 90f * c, CornerResolution));
        }

        // Add verts and triangles to mesh
        for (int i = 0, t = 0 ; i < exteriorVerts.Count ; i++, t += 4) {
            vert.position = interiorVerts[i];
            Vector2 uv = new Vector2 (0.5f * vert.position.x / (0.5f * width) + 0.5f, 0.5f * vert.position.y / (0.5f * height) + 0.5f);
            vert.uv0 = uv;
            vh.AddVert (vert);

            vert.position = exteriorVerts[i];
            uv = new Vector2 (0.5f * vert.position.x / (0.5f * width) + 0.5f, 0.5f * vert.position.y / (0.5f * height) + 0.5f);
            vert.uv0 = uv;
            vh.AddVert (vert);

            vert.position = interiorVerts[(i + 1) % exteriorVerts.Count];
            uv = new Vector2 (0.5f * vert.position.x / (0.5f * width) + 0.5f, 0.5f * vert.position.y / (0.5f * height) + 0.5f);
            vert.uv0 = uv;
            vh.AddVert (vert);

            vert.position = exteriorVerts[(i + 1) % exteriorVerts.Count];
            uv = new Vector2 (0.5f * vert.position.x / (0.5f * width) + 0.5f, 0.5f * vert.position.y / (0.5f * height) + 0.5f);
            vert.uv0 = uv;
            vh.AddVert (vert);

            vh.AddTriangle (t, t + 3, t + 1);
            vh.AddTriangle (t, t + 2, t + 3);
        }
    }

    public void GenerateMesh (VertexHelper vh) {
        float minLength = Mathf.Min (rectTransform.rect.width, rectTransform.rect.height);
        float clampedCornerRadius = Mathf.Clamp (CornerRadius, 0.001f, minLength / 2f - 0.001f);
        float[] clampedCornerRadii = new float[] {
            Mathf.Clamp (CornerRadii[0], 0.001f, minLength / 2f - 0.001f),
            Mathf.Clamp (CornerRadii[1], 0.001f, minLength / 2f - 0.001f),
            Mathf.Clamp (CornerRadii[2], 0.001f, minLength / 2f - 0.001f),
            Mathf.Clamp (CornerRadii[3], 0.001f, minLength / 2f - 0.001f),
        };
        UIVertex vert = UIVertex.simpleVert;

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        Vector2[] interiorCorners = new Vector2[4];

        float xOffset = (width / 2f - clampedCornerRadius);
        float yOffset = (height / 2f - clampedCornerRadius);

        interiorCorners[0] = new Vector2 (xOffset, yOffset);
        interiorCorners[1] = new Vector2 (-xOffset, yOffset);
        interiorCorners[2] = new Vector2 (-xOffset, -yOffset);
        interiorCorners[3] = new Vector2 (xOffset, -yOffset);

        if (UseSeparateCornerRadii) {
            xOffset = width / 2f;
            yOffset = height / 2f;
            interiorCorners[0] = new Vector2 ((xOffset - clampedCornerRadii[0]), (yOffset - clampedCornerRadii[0]));
            interiorCorners[1] = new Vector2 (-(xOffset - clampedCornerRadii[1]), (yOffset - clampedCornerRadii[1]));
            interiorCorners[2] = new Vector2 (-(xOffset - clampedCornerRadii[2]), -(yOffset - clampedCornerRadii[2]));
            interiorCorners[3] = new Vector2 ((xOffset - clampedCornerRadii[3]), -(yOffset - clampedCornerRadii[3]));
        }

        // Add central vert
        vert.position = Vector2.zero;
        vert.color = color;
        vert.uv0 = new Vector2 (0.5f, 0.5f);
        vh.AddVert (vert);

        // Generate exterior vertices
        List<Vector2> exteriorVerts = new List<Vector2> ();
        for (int c = 0 ; c < 4 ; c++) {
            float radius = clampedCornerRadius;
            if (UseSeparateCornerRadii) {
                radius = clampedCornerRadii[c];
            }

            exteriorVerts.AddRange (GenerateCornerVerts (interiorCorners[c], radius, 90f * c, CornerResolution));
        }

        int n = exteriorVerts.Count + 1;

        // Add verts and triangles to stream
        for (int i = 0 ; i < exteriorVerts.Count ; i++) {
            vert.position = exteriorVerts[i];
            vert.uv0 = new Vector2 (0.5f * exteriorVerts[i].x / (width * 0.5f) + 0.5f, 0.5f * exteriorVerts[i].y / (height * 0.5f) + 0.5f);
            vert.color = color;

            vh.AddVert (vert);

            vh.AddTriangle (0, (i + 2) % n, (i + 1) % n);
        }

        vh.AddTriangle (0, 1, exteriorVerts.Count);
    }

    void AltGenerateMesh (VertexHelper vh) {
        float minLength = Mathf.Min (rectTransform.rect.width, rectTransform.rect.height);
        CornerRadius = Mathf.Clamp (CornerRadius, 0.001f, minLength / 2f - 0.001f);

        IndicesByVertex = new Dictionary<Vector2, int> ();

        UIVertex vert = UIVertex.simpleVert;

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        Vector2[] interiorCorners = new Vector2[4];

        float xOffset = (width / 2f - CornerRadius);
        float yOffset = (height / 2f - CornerRadius);

        interiorCorners[0] = new Vector2 (-xOffset, -yOffset);
        interiorCorners[1] = new Vector2 (xOffset, -yOffset);
        interiorCorners[2] = new Vector2 (xOffset, yOffset);
        interiorCorners[3] = new Vector2 (-xOffset, yOffset);

        // Add vertices to index lookup map
        for (int i = 0 ; i < interiorCorners.Length ; i++) {
            IndicesByVertex.Add (interiorCorners[i], i);
        }

        // Create outer rect corners
        Vector2 c0x = interiorCorners[0];
        c0x.x -= CornerRadius;
        IndicesByVertex.Add (c0x, 4);

        Vector2 c0y = interiorCorners[0];
        c0y.y -= CornerRadius;
        IndicesByVertex.Add (c0y, 5);

        Vector2 c1x = interiorCorners[1];
        c1x.x += CornerRadius;
        IndicesByVertex.Add (c1x, 6);

        Vector2 c1y = interiorCorners[1];
        c1y.y -= CornerRadius;
        IndicesByVertex.Add (c1y, 7);

        Vector2 c2x = interiorCorners[2];
        c2x.x += CornerRadius;
        IndicesByVertex.Add (c2x, 8);

        Vector2 c2y = interiorCorners[2];
        c2y.y += CornerRadius;
        IndicesByVertex.Add (c2y, 9);

        Vector2 c3x = interiorCorners[3];
        c3x.x -= CornerRadius;
        IndicesByVertex.Add (c3x, 10);

        Vector2 c3y = interiorCorners[3];
        c3y.y += CornerRadius;
        IndicesByVertex.Add (c3y, 11);

        // Populate main rect verts
        foreach (var kvp in IndicesByVertex) {
            vert.position = kvp.Key;
            vert.color = color;

            Vector2 uv = kvp.Key;
            uv.x = uv.x / (width / 2f) + 0.5f;
            uv.x = uv.y / (height / 2f) + 0.5f;
            vert.uv0 = uv;

            vh.AddVert (vert);
        }

        // Create rounded corner verts
        BuildCorner (vh, vert, 0f, 2, interiorCorners, c2x, c2y);
        BuildCorner (vh, vert, 90f, 3, interiorCorners, c3y, c3x);
        BuildCorner (vh, vert, 180f, 0, interiorCorners, c0x, c0y);
        BuildCorner (vh, vert, 270f, 1, interiorCorners, c1y, c1x);

        // Create Triangles

        // Center Rect
        vh.AddTriangle (IndicesByVertex[interiorCorners[0]], IndicesByVertex[interiorCorners[1]], IndicesByVertex[interiorCorners[2]]);
        vh.AddTriangle (IndicesByVertex[interiorCorners[0]], IndicesByVertex[interiorCorners[2]], IndicesByVertex[interiorCorners[3]]);

        // Bottom Rect
        vh.AddTriangle (IndicesByVertex[interiorCorners[0]], IndicesByVertex[c0y], IndicesByVertex[interiorCorners[1]]);
        vh.AddTriangle (IndicesByVertex[c0y], IndicesByVertex[c1y], IndicesByVertex[interiorCorners[1]]);

        // Right Rect
        vh.AddTriangle (IndicesByVertex[interiorCorners[1]], IndicesByVertex[c1x], IndicesByVertex[interiorCorners[2]]);
        vh.AddTriangle (IndicesByVertex[c1x], IndicesByVertex[c2x], IndicesByVertex[interiorCorners[2]]);

        // Top Rect
        vh.AddTriangle (IndicesByVertex[interiorCorners[2]], IndicesByVertex[c2y], IndicesByVertex[interiorCorners[3]]);
        vh.AddTriangle (IndicesByVertex[c2y], IndicesByVertex[c3y], IndicesByVertex[interiorCorners[3]]);

        // Left Rect
        vh.AddTriangle (IndicesByVertex[interiorCorners[3]], IndicesByVertex[c3x], IndicesByVertex[interiorCorners[0]]);
        vh.AddTriangle (IndicesByVertex[c3x], IndicesByVertex[c0x], IndicesByVertex[interiorCorners[0]]);
    }

    void BuildCorner (VertexHelper vh, UIVertex vert, float startAngle, int currentCornerIdx, Vector2[] interiorCorners, Vector2 startVertex, Vector2 endVertex) {
        List<Vector2> cornerVerts = new List<Vector2> ();

        int startIndex = IndicesByVertex.Count;

        for (int i = 0 ; i < CornerResolution ; i++) {
            float theta = Mathf.Deg2Rad * 90f * (float) (i + 1) / (CornerResolution + 1) + Mathf.Deg2Rad * startAngle;
            float x = Mathf.Cos (theta) * CornerRadius;
            float y = Mathf.Sin (theta) * CornerRadius;
            Vector2 pos = interiorCorners[currentCornerIdx] + new Vector2 (x, y);
            cornerVerts.Add (pos);
            IndicesByVertex.Add (pos, startIndex + i);
        }

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        // Add verts to stream
        foreach (Vector2 v in cornerVerts) {
            vert.position = v;

            Vector2 uv = v;
            uv.x = uv.x / (width / 2f) + 0.5f;
            uv.x = uv.y / (height / 2f) + 0.5f;
            vert.uv0 = uv;

            vh.AddVert (vert);
        }

        cornerVerts.Insert (0, startVertex);
        cornerVerts.Add (endVertex);

        for (int i = 0 ; i < cornerVerts.Count - 1 ; i++) {
            vh.AddTriangle (currentCornerIdx, IndicesByVertex[cornerVerts[i]], IndicesByVertex[cornerVerts[i + 1]]);
        }
    }

    List<Vector2> GenerateCornerVerts (Vector2 center, float radius, float startAngle, float resolution) {
        List<Vector2> verts = new List<Vector2> ();
        resolution = resolution <= 0 ? 1 : resolution;

        for (int i = 0 ; i <= resolution ; i++) {
            float theta = Mathf.Deg2Rad * (90f * (float) i / resolution + startAngle);
            float x = Mathf.Cos (theta) * radius;
            float y = Mathf.Sin (theta) * radius;

            Vector2 v = center + new Vector2 (x, y);
            verts.Add (v);
        }

        return verts;
    }

    public void SetCornerRadii (float[] cornerRadii) {
        CornerRadii = cornerRadii;
    }

    public void Refresh () {
#if UNITY_EDITOR
        OnValidate ();
#endif
    }
}