using UnityEngine;
using UnityEngine.UI;

namespace JoyCraft.Experimental {
    public class CustomMeshGraphic : Graphic {
        // Field to hold the mesh you want to display
        public Mesh mesh;
        public float geometryScale = 400f;

        // Field to control the color of the mesh
        public Color meshColor = Color.white;

        // Update the mesh whenever the Graphic is refreshed
        protected override void OnPopulateMesh (VertexHelper vh) {
            vh.Clear ();

            if (mesh == null)
                return;

            // Get mesh data
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;
            var colors = mesh.colors;
            var uv = mesh.uv;

            // Add vertices
            for (int i = 0 ; i < vertices.Length ; i++) {
                var vertexColor = i < colors.Length ? colors[i] : meshColor;
                var vertexUV = i < uv.Length ? uv[i] : Vector2.zero;

                vh.AddVert (vertices[i] * geometryScale, vertexColor, vertexUV);
            }

            // Add triangles
            for (int i = 0 ; i < triangles.Length ; i += 3) {
                vh.AddTriangle (triangles[i], triangles[i + 1], triangles[i + 2]);
            }
        }

        // Optional: Allow mesh updates from the Inspector
        public void SetMesh (Mesh newMesh) {
            mesh = newMesh;
            SetVerticesDirty (); // Forces the mesh to update
        }

        public void SetMeshColor (Color newColor) {
            meshColor = newColor;
            SetVerticesDirty (); // Forces the color to update
        }

#if UNITY_EDITOR
        new private void OnValidate () {
            base.OnValidate (); OnPopulateMesh (new VertexHelper ());
        }
#endif
    }
}