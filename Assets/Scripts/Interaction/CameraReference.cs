using UnityEngine;

[RequireComponent (typeof (Camera))]
public class CameraReference : JoyCraft.SingletonComponent<CameraReference> {


    new private Camera camera;

    public Camera MainCamera {
        get {
            if (camera == null) camera = GetComponent<Camera> ();
            return camera;
        }
    }

    public Vector3 GetMousePosition () {
        return MainCamera.ScreenToWorldPoint (Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
    }
}
