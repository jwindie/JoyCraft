using UnityEngine;

namespace IdleHands.Interaction {

    [CreateAssetMenu (fileName = "Hand Tool", menuName = "Interaction Tool/Hand Tool", order = 0)]
    public class HandTool : InteractionTool {

        [SerializeField] private float reach;
        [SerializeField] private LayerMask mask;

        public override void OnUpdate (Camera camera, bool useSmartRaycasting) {

            if (Input.GetMouseButtonDown (0)) {
                Physics.Raycast (camera.ScreenPointToRay (Input.mousePosition), out RaycastHit hit, reach, mask);
                //if (hit.collider && hit.collider.TryGetComponent (out IInspectable inspectable)) {
                //    //inspect the object
                //}
                if (hit.collider) {
                    var name = hit.collider.name;
                    Indev.DebugTooltip.Current.SetPositionFixed (hit.point);
                    Indev.DebugTooltip.Current.SetLabelText (name);
                }
            }
            else if (Input.GetMouseButtonDown (1)) {
                Indev.DebugTooltip.Current.Hide ();
            }
        }

        public void InteractWith(Collider c) {

        }

        public override void OnCloseTool () {
            Indev.DebugTooltip.Current.Hide ();
        }
    }
}