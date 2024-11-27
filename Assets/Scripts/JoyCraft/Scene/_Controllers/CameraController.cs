using PlugNPlay.Interaction;
using JoyCraft.Application;
using System.Collections.Generic;
using UnityEngine;

namespace JoyCraft.Scene {
    public class CameraController : MonoBehaviour {

        const int LAYER_MASK = ~((1 << 2) + (1 << 6));

        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera overlayCamera;

        [Header ("Movement Settings")]
        [SerializeField] private float minZoom, maxZoom;
        [SerializeField] private float zoomSpeed;
        [SerializeField] private float smoothZoom;
        [SerializeField] private float rotationSpeed = 10;
        [SerializeField] private float panSpeed = .01f;

        [Header ("Snap Settings")]
        [SerializeField] private float snapGridResolution = 1;
        [SerializeField] private float snapRotationResolution = 360 / 8f;

        private float targetZoom = 6;
        private Vector3 mouseDragStartPosition;
        private bool panningCamera;
        private Vector3 lastCameraPosition;
        private Vector2 cameraPanBounds;
        private Vector3 lastMousePosition;
        private RaycastHit2D raycastHit;
        private Card card;
        private List<Transform> grabbitGroupTransformsWithoutParent = new List<Transform> ();
        private IOutline lastOutlineObject;
        private bool drawingMarquee, overCollider;
        private object lastClickedObject, lastRaycastedObject;
        private MultiClicker multiClicker = new MultiClicker ();

        public Camera MainCamera {
            get {
                return mainCamera;
            }
        }

        private void Start () {
            multiClicker.SetClickMaxTime (.5f);
            mainCamera.orthographicSize = minZoom + 1;
            overlayCamera.orthographicSize = minZoom + 1;
            targetZoom = minZoom;
        }

        private void Update () {
            Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint (Input.mousePosition);
            multiClicker.Tick ();

            if (!panningCamera && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ()) {
                UpdateInteractionUI ();
                return;
            }
            else UpdateInteractionScene (worldMousePosition);
        }

        private void LateUpdate () {
            mainCamera.orthographicSize = Mathf.Lerp (mainCamera.orthographicSize, targetZoom, Library.LERP_SPEED * 0.3f);
            mainCamera.transform.position = Context.Current.Workspace.ClampCameraPosition (mainCamera.transform.position);
            overlayCamera.orthographicSize = mainCamera.orthographicSize;
        }

        private void UpdateInteractionUI () { }

        private void UpdateInteractionScene (Vector3 worldMousePosition) {

            //handle mouse releases
            if (App.Current.InputHandler.LeftMouseUp) {
                CursorController.Current.SetCursor (CursorController.Current.HandCursor);
                SelectedCardHandler.Current.ClearSelection ();
                //if (grabbit) ReleaseGrabbit ();
            }

            if (!overCollider && App.Current.InputHandler.PanButtonDown) {
                BeginMousePan ();
                CursorController.Current.SetCursor (CursorController.Current.PanCursor);
            }
            if (App.Current.InputHandler.LeftMouseDown) HandleLeftClick ();

            //check for grabbit selection
            if (SelectedCardHandler.Current.HasSelection) {
                //}
                //if (grabbit) {
                //pass data to the grabbit's onUpdate function
                var updateData = new JoyCraft.Scene.OnUpdateData {
                    LMB = App.Current.InputHandler.LeftMouseDown,
                    snapToGrid = false,
                    rotation = App.Current.InputHandler.MouseScrollDelta * rotationSpeed,
                    snapPositionResolution = snapGridResolution,
                    snapRotationResolution = snapRotationResolution,
                    position = worldMousePosition
                };
                //grabbit.OnUpdate (onUpdateData);
                SelectedCardHandler.Current.OnUpdate (updateData);
            }
            //do a raycast into the scene if mouse has moved
            DoRaycast (worldMousePosition);
            HandleHover ();

            //update zooom targets
            targetZoom += App.Current.InputHandler.MouseScrollDelta * zoomSpeed;
            targetZoom = Mathf.Clamp (targetZoom, minZoom, maxZoom);

            if (panningCamera) {
                HandleCameraPan ();
                return;
            }
        }

        private void HandleCameraPan () {
            if (App.Current.InputHandler.PanButtonUp) {
                CursorController.Current.SetCursor (CursorController.Current.DefaultCursor);
                panningCamera = false;
            }
            else {
                //calculate new camera position
                Vector3 panDelta = Input.mousePosition - mouseDragStartPosition;
                Vector3 panOffset = lastCameraPosition - (panDelta * panSpeed * mainCamera.orthographicSize * .3f);
                Vector2 finalPanPosition = Context.Current.Workspace.ClampCameraPosition (panOffset);

                //lerp to new camera position
                mainCamera.transform.position = Vector3.Lerp (
                    mainCamera.transform.position,
                    finalPanPosition,
                    Library.LERP_SPEED * 1.5f
                );
            }
        }

        private void HandleLeftClick () {
            if (raycastHit.collider && raycastHit.collider.TryGetComponent (out card)) {

                CursorController.Current.SetCursor (CursorController.Current.GrabCursor);

                //reset the mouse click if this is a new object from last frame
                if ((object) raycastHit.collider != lastClickedObject) {
                    multiClicker.ResetClicks ("Mouse0");
                    lastClickedObject = raycastHit.collider;
                }

                //record the click
                multiClicker.RecordInput ("Mouse0");
                int mouseClicks = multiClicker.QueryClicks ("Mouse0");

                Vector3 worldSpaceMousePosition = mainCamera.ScreenToWorldPoint (Input.mousePosition);
                //grabbit.Grab (worldSpaceMousePosition);
                SelectedCardHandler.Current.SetSelection (card, worldSpaceMousePosition);
            }
        }

        private void HandleHover () {
            //if the raycast object is the same as last frame skip
            if ((object) raycastHit.collider == lastRaycastedObject) return;

            //do we have anything to hover over?
            if (raycastHit.collider) {
                CursorController.Current.SetCursor (CursorController.Current.HandCursor);
                overCollider = true;
                OutlineCollider (raycastHit.collider);
                //Debug.Log ("Highlight");
            }
            else {
                CursorController.Current.SetCursor (CursorController.Current.DefaultCursor);
                //grabbit = null;
                overCollider = false;
                ClearLastOutline ();
                //Debug.Log ("Remove Highlight");
            }

            lastRaycastedObject = raycastHit.collider;
        }

        private void DoRaycast (Vector3 worldMousePosition) {
            if (Input.mousePosition != lastMousePosition) {
                lastMousePosition = Input.mousePosition;

                //shoot a ray to see if the mouse cursor is over anything important
                //if the mouse position was the same as the last skip raycasting
                raycastHit = Physics2D.Raycast (worldMousePosition, Vector2.zero, 100, LAYER_MASK);
            }
        }

        private void BeginMousePan () {
            panningCamera = true;
            mouseDragStartPosition = Input.mousePosition;
            lastCameraPosition = transform.position;
        }

        private void ClearLastOutline () {
            lastOutlineObject?.DisableOutline ();
            lastOutlineObject = null;
        }

        private void OutlineCollider (Collider2D collider) {
            lastOutlineObject?.DisableOutline ();
            (lastOutlineObject = collider.GetComponent<IOutline> ())?.EnableOutline ();
        }

        private void ReleaseGrabbit () {
            card.Release ();
            card = null;
        }
    }
}