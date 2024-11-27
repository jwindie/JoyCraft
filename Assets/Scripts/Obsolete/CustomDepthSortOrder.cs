using UnityEngine;

#if  UNITY_EDITOR
[ExecuteAlways]
public class CustomDepthSortOrder : MonoBehaviour {

    const float ORDER_SCALE = .001f;

    [SerializeField] private int order;

    void OnValidate () {
        Update ();
    }
    void Update () {
        SetDepth ();
    }
    void SetDepth () {
        transform.localPosition = new Vector3 (
        transform.localPosition.x,
        transform.localPosition.y,
        order * ORDER_SCALE
        );
    }
}
#endif