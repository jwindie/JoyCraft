using System.Collections;
using UnityEngine;

public class WaveParticleMeshGenerator : MonoBehaviour {

    public const float MAX_NODE_DISTANCE = .2f;
    public const int MAX_POINTS = 30;
    public const float MAX_RADIUS = 5f;
    public const float SPEED = 3.5f;

    public static Mesh[] meshes = new Mesh[256];


    static Vector3[] GetPoints (int n, float r) {
        Vector3[] points = new Vector3[n];
        float angle = Mathf.PI * 2 / n;

        for (int i = 0 ; i < n ; i++) {
            points[i] = new Vector3 (
                r * Mathf.Cos (angle * i),
                r * Mathf.Sin (angle * i),
                0
                );
        }

        return points;
    }


    [SerializeField] LineRenderer lineRenderer;

    private Vector3[] points;

    public float radius;

    private void Start () {
        lineRenderer.positionCount = MAX_POINTS + 1;
        GenerateMeshes ();
    }
    private void GenerateMeshes () {
        float interval = MAX_RADIUS / meshes.Length;
        for (int i = 0 ; i < meshes.Length ; i++) {
            radius = interval * i;

            UpdateLineRenderer ();
            lineRenderer.BakeMesh (meshes[i] = new Mesh ());
        }
    }

    public void UpdateLineRenderer () {
        points = GetPoints (MAX_POINTS, radius);
        lineRenderer.SetPositions (points);
        lineRenderer.SetPosition (points.Length, points[0]);

    }
}
