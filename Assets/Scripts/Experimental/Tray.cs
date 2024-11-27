using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tray : MonoBehaviour {

    public enum AnchorMode {
        None,
        Top,
        Left,
        Bottom,
        Right
    }


    [SerializeField] protected Vector3 anchorPosition;
    [SerializeField] protected AnchorMode anchorMode = AnchorMode.Top;


    public void Update () {
        //does notnneed to run every frame, just every frame that the camera moved
        //consider using an event system of sorts

        switch (anchorMode) {
            case AnchorMode.Top:
                AnchorToTop ();
                break;
        }
    }

    void AnchorToTop () {
        //get the position of the top of the screen
        //float yPositionInWorld = CameraController.Current.MainCamera.ScreenToWorldPoint (new Vector2 (0, Screen.height), Camera.MonoOrStereoscopicEye.Mono).y;

        //snap the tray to its position overriding the y value
        //transform.position = new Vector3 (anchorPosition.x, yPositionInWorld, anchorPosition.z);
    }
}
