using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PolygonGraphic : Image
{
    public List<Vector2> Vertices;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        Debug.Log ("Populating!");

        vh.Clear();

        if(Vertices == null || Vertices.Count < 3)
        {
            Debug.Log ("Returning!");
            return;
        }

        UIVertex vert = UIVertex.simpleVert;

        // Add vertex at center
        vert.position = rectTransform.pivot;
        vert.color = color;
        vert.uv0 = new Vector2(0.5f, 0.5f);

        vh.AddVert(vert);

        List<Vector2> verts = Vertices.OrderBy((x) => Mathf.Atan2(x.x, x.y)).ToList();

        for(int i = 0; i < verts.Count; i++)
        {
            vert.position = rectTransform.pivot + verts[i];
            vh.AddVert(vert);
        }     

        for(int t = 1; t < verts.Count; t++)
        {
            vh.AddTriangle(t + 1, 0, t);
        }

        vh.AddTriangle(verts.Count, 0, 1);
    }
}
