using UnityEngine;

using JoyCraft.Scene;

public class WorldSpaceCanvasUtility : MonoBehaviour {
    void Start () {
        GetComponent<Canvas> ().worldCamera = Context.Current.CameraController.MainCamera;
        Destroy (this);
    }
}
