using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeshGraphic : Image {
    [Header ("Regular Polygon Options")]
    public Mesh mesh;

    protected override void OnPopulateMesh (VertexHelper vh) {
        vh.Clear ();
        UIVertex vert = UIVertex.simpleVert;

        DrawMesh (vh, vert);

    }

    private void DrawMesh (VertexHelper vh, UIVertex vert) {
        //vert.position = Vector2.zero;
        vert.color = color;
        vert.uv0 = new Vector2 (0.5f, 0.5f);
        //vh.AddVert (vert);

        for (int i = 0 ; i < mesh.vertices.Length ; i++) {
            //float angle = Mathf.Deg2Rad * i * 360f / Corners;

            //if (type == Type.Filled) {
            //    if (angle > fillAmount * 360f * Mathf.Deg2Rad) {
            //        angle = fillAmount * 360f * Mathf.Deg2Rad;
            //    }
            //}

            //float x = Mathf.Cos (angle + Mathf.Deg2Rad * OffsetAngle);
            //float y = Mathf.Sin (angle + Mathf.Deg2Rad * OffsetAngle);

            Vector2 v = Vector2.zero;
            v.x += rectTransform.rect.width / 2f;
            v.y += rectTransform.rect.width / 2f;

            //// When using radial fill, calculate the correct distance from the center to the edge of the polygon
            //float mag = Mathf.Cos (Mathf.PI / Corners) / Mathf.Cos (angle % (2f * Mathf.PI / Corners) - Mathf.PI / Corners);
            //v = v.normalized * mag * rectTransform.rect.width / 2f;

            //Vector2 uv = new Vector2 (x / 2f + 0.5f, y / 2f + 0.5f);

            vert.position = mesh.vertices[i];
            vert.uv0 = new Vector2 (0, 0);
            vh.AddVert (vert);
        }

        for (int t = 0 ; t <= mesh.triangles.Length ; t += 3) {
            vh.AddTriangle (t, t + 1, t + 2);
        }
    }

    public void Refresh () {
#if UNITY_EDITOR
        OnValidate ();
#endif
    }
}
