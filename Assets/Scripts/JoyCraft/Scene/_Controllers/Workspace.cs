using UnityEngine;

namespace JoyCraft.Scene {

    [RequireComponent (typeof (LineRenderer))]
    public class Workspace : MonoBehaviour {

        [Header("Settings")]
        [SerializeField] public Vector2 size = new Vector2 (40, 40);
        [SerializeField] public float padding = -2f;
        [SerializeField] public float borderThickness = .2f;

        [Header ("Refrences")]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Material backgroundMaterial;

        [Space (20)]
        [Header ("Optional")]
        [SerializeField] private RoundedLine roundedLine;

        public bool IsWithinBounds (Vector2 point) {
            var sizeHalved = size / 2f;

            if (point.x < -sizeHalved.x) return false;
            if (point.x > sizeHalved.x) return false;

            if (point.y < -sizeHalved.y) return false;
            if (point.y > sizeHalved.y) return false;
            return true;
        }

        public Vector2 ClampCameraPosition (Vector2 position) {

            position.x  = Mathf.Clamp (position.x, (size.x + padding) * -.5f, (size.x + padding) * .5f );
            position.y  = Mathf.Clamp (position.y, (size.y + padding) * -.5f, (size.y + padding) * .5f );
            return position;
        }

        public void UpdateWorkspace () {

            if (roundedLine) DrawRoundedBorder();
            else DrawBorder();

            backgroundMaterial.SetVector ("_Bounds", size + Vector2.one);
        }

        private void DrawBorder() {
            Vector3[] points = new Vector3[] {
                new Vector3 (0,(size.y + 1+borderThickness) * .5f,.3f),
                new Vector3 ((size.x + 1+  borderThickness) * .5f,(size.y +1 +  borderThickness) * .5f,.3f),
                new Vector3 ((size.x + 1+  borderThickness) * .5f,(size.y + 1+  borderThickness) * -.5f,.3f),
                new Vector3 ((size.x + 1+  borderThickness) * -.5f,(size.y + 1+ borderThickness) * -.5f,.3f),
                new Vector3 ((size.x + 1+  borderThickness) * -.5f,(size.y + 1+ borderThickness) * .5f,.3f),
                new Vector3 (0,(size.y + 1+borderThickness) * .5f,.3f),
            };
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
            lineRenderer.startWidth = lineRenderer.endWidth = borderThickness;
        }

        private void DrawRoundedBorder() {
            Vector2[] points = new Vector2[] {
                new Vector2 (0,(size.y + 1+borderThickness) * .5f),
                new Vector2 ((size.x + 1+  borderThickness) * .5f,(size.y +1 +  borderThickness) * .5f),
                new Vector2 ((size.x + 1+  borderThickness) * .5f,(size.y + 1+  borderThickness) * -.5f),
                new Vector2 ((size.x + 1+  borderThickness) * -.5f,(size.y + 1+ borderThickness) * -.5f),
                new Vector2 ((size.x + 1+  borderThickness) * -.5f,(size.y + 1+ borderThickness) * .5f),
                new Vector2 (0,(size.y + 1+borderThickness) * .5f),
            };
            roundedLine.SetPoints (points);
        }

        private void OnDrawGizmos () {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube (Vector3.zero, new Vector3 (size.x, size.y, 0));

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube (Vector3.zero, new Vector3 (size.x + padding, size.y + padding, 0));
        }

#if UNITY_EDITOR
        private void OnValidate () {
            name = "Workspace";
            UpdateWorkspace ();
        }
#endif
    }
}