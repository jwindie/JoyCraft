using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class ProcGenCard : MonoBehaviour {

    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private BoxCollider2D collider;
    [Space]
    [SerializeField] private string meshName;
    [SerializeField] private bool build;

    public float CornerRadius;
    public float[] CornerRadii = new float[4];
    public int CornerResolution;

    [Header ("Rounded Rect Options")]
    public bool UseSeparateCornerRadii;
    [SerializeField] private bool refresh;

    public List<Vector3> vertex = new List<Vector3> ();
    public List<Vector3> normal = new List<Vector3> ();
    public List<int> triangle = new List<int> ();

    public void ClearVertices () {
        vertex.Clear ();
        normal.Clear ();
        triangle.Clear ();
    }


    private Dictionary<Vector2, int> IndicesByVertex;

    public void SetSize (float width, float height) {
        this.width = width;
        this.height = height;

        GenerateAndApplyMesh ();
    }

    public void OnValidate () {
        GenerateAndApplyMesh ();
        refresh = false;
    }

    void GenerateAndApplyMesh () {
        if (meshFilter) {
            GenerateMesh ();

            //add final triangle
            triangle.AddRange (new int[] { triangle[triangle.Count - 1], 0, 1 });


            Mesh m = new Mesh ();
            m.SetVertices (vertex);
            m.SetNormals (normal);
            m.SetTriangles (triangle, 0);
            ClearVertices ();

#if UNITY_EDITOR
            meshFilter.sharedMesh = m;
#endif
        }

        if (collider) collider.size = new Vector2 (width, height);

        if (build) {
            SaveMeshToFile (meshFilter.sharedMesh);
            build = false;
        }
    }

    public void SaveMeshToFile (Mesh mesh) {
        if (mesh == null) return;

        StringBuilder sb = new StringBuilder ($"o {meshName}\n");
        sb.AppendLine ($"g {meshName}");


        // Write vertices
        foreach (Vector3 vertex in mesh.vertices) {
            sb.AppendLine ($"v {vertex.x} {vertex.y} 0");
            
        }

        // Write normals
        foreach (Vector3 normal in mesh.normals) {
            sb.AppendLine ($"vn {normal.x} {normal.y} {normal.z}");
        }

        // Write UVs
        foreach (Vector2 uv in mesh.uv) {
            sb.AppendLine ($"vt {uv.x} {uv.y}");
        }

        // Write triangles
        for (int i = 0 ; i < mesh.triangles.Length ; i += 3) {
            sb.AppendLine ($"f {mesh.triangles[i] + 1} {mesh.triangles[i + 1] + 1} {mesh.triangles[i + 2] + 1}");
        }

        string path = Path.Combine (Application.dataPath, meshName + ".obj");
        File.WriteAllText (path, sb.ToString ());

        Debug.Log ($"Mesh saved to: {path}");

        System.Diagnostics.Process.Start (path);
    }

    public float GetMinCornerRadius () {
        float minLength = Mathf.Min (width, height) / 2f - 0.001f;
        return minLength;
    }

    public void GenerateMesh () {
        ClearVertices ();

        float minLength = Mathf.Min (width, height);
        float clampedCornerRadius = Mathf.Clamp (CornerRadius, 0.001f, minLength / 2f - 0.001f);
        float[] clampedCornerRadii = new float[] {
            Mathf.Clamp (CornerRadii[0], 0.001f, minLength / 2f - 0.001f),
            Mathf.Clamp (CornerRadii[1], 0.001f, minLength / 2f - 0.001f),
            Mathf.Clamp (CornerRadii[2], 0.001f, minLength / 2f - 0.001f),
            Mathf.Clamp (CornerRadii[3], 0.001f, minLength / 2f - 0.001f),
        };

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
        vertex.Add (Vector2.zero);
        normal.Add (Vector3.forward);

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
            vertex.Add (exteriorVerts[i]);
            normal.Add (Vector3.forward);
            //vert.uv0 = new Vector2 (0.5f * exteriorVerts[i].x / (width * 0.5f) + 0.5f, 0.5f * exteriorVerts[i].y / (height * 0.5f) + 0.5f);

            //vh.AddVert (vert);

            triangle.AddRange (new int[] { 0, (i + 2) % n, (i + 1) % n });
        }
    }

    void AltGenerateMesh () {
        ClearVertices ();

        float minLength = Mathf.Min (width, height);
        CornerRadius = Mathf.Clamp (CornerRadius, 0.001f, minLength / 2f - 0.001f);

        IndicesByVertex = new Dictionary<Vector2, int> ();

        Vector3 point = Vector3.zero;
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
            point = kvp.Key;
            normal.Add (Vector3.forward);

            //Vector2 uv = kvp.Key;
            //uv.x = uv.x / (width / 2f) + 0.5f;
            //uv.x = uv.y / (height / 2f) + 0.5f;
            //vert.uv0 = uv;

            //vh.AddVert (vert);
        }

        // Create rounded corner verts
        BuildCorner (point, 0f, 2, interiorCorners, c2x, c2y);
        BuildCorner (point, 90f, 3, interiorCorners, c3y, c3x);
        BuildCorner (point, 180f, 0, interiorCorners, c0x, c0y);
        BuildCorner (point, 270f, 1, interiorCorners, c1y, c1x);

        // Create Triangles

        // Center Rect
        triangle.AddRange (new int[] { IndicesByVertex[interiorCorners[0]], IndicesByVertex[interiorCorners[1]], IndicesByVertex[interiorCorners[2]] });
        triangle.AddRange (new int[] { IndicesByVertex[interiorCorners[0]], IndicesByVertex[interiorCorners[2]], IndicesByVertex[interiorCorners[3]] });


        triangle.AddRange (new int[] { IndicesByVertex[interiorCorners[0]], IndicesByVertex[c0y], IndicesByVertex[interiorCorners[1]] });
        triangle.AddRange (new int[] { IndicesByVertex[c0y], IndicesByVertex[c1y], IndicesByVertex[interiorCorners[1]] });


        triangle.AddRange (new int[] { IndicesByVertex[interiorCorners[1]], IndicesByVertex[c1x], IndicesByVertex[interiorCorners[2]] });
        triangle.AddRange (new int[] { IndicesByVertex[c1x], IndicesByVertex[c2x], IndicesByVertex[interiorCorners[2]] });


        triangle.AddRange (new int[] { IndicesByVertex[interiorCorners[2]], IndicesByVertex[c2y], IndicesByVertex[interiorCorners[3]] });
        triangle.AddRange (new int[] { IndicesByVertex[c2y], IndicesByVertex[c3y], IndicesByVertex[interiorCorners[3]] });


        triangle.AddRange (new int[] { IndicesByVertex[interiorCorners[3]], IndicesByVertex[c3x], IndicesByVertex[interiorCorners[0]] });
        triangle.AddRange (new int[] { IndicesByVertex[c3x], IndicesByVertex[c0x], IndicesByVertex[interiorCorners[0]] });
    }

    void BuildCorner (Vector3 point, float startAngle, int currentCornerIdx, Vector2[] interiorCorners, Vector2 startVertex, Vector2 endVertex) {
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

        // Add verts to stream
        foreach (Vector2 v in cornerVerts) {
            point = v;

            //Vector2 uv = v;
            //uv.x = uv.x / (width / 2f) + 0.5f;
            //uv.x = uv.y / (height / 2f) + 0.5f;
            //point.uv0 = uv;

            vertex.Add (point);
            normal.Add (Vector3.forward);
        }

        cornerVerts.Insert (0, startVertex);
        cornerVerts.Add (endVertex);

        for (int i = 0 ; i < cornerVerts.Count - 1 ; i++) {
            triangle.AddRange (new int[] { currentCornerIdx, IndicesByVertex[cornerVerts[i]], IndicesByVertex[cornerVerts[i + 1]] });
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