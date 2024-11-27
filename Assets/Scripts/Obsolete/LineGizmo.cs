using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGizmo : MonoBehaviour {
#if UNITY_EDITOR
    [SerializeField] private Vector2 direction;
    [SerializeField] private float length;
    [SerializeField] private Color color;
    private void OnDrawGizmos () {
        Vector2 a = (Vector2) transform.position - (direction * length);
        Vector2 b = (Vector2) transform.position + (direction * length);

        Gizmos.color = color;
        Gizmos.DrawLine (a, b);
    }
#endif
}
