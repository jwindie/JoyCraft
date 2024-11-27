using UnityEngine;

namespace IdleHands.Interaction {

    [CreateAssetMenu (fileName = "Identify Tool", menuName = "Interaction Tool/Identify Tool", order = 1)]
    public class IdentifyTool : InteractionTool {

        [SerializeField] private float reach;
        [SerializeField] private LayerMask mask;


        public override void OnOpenTool () {
            SmartRaycast.Mask = mask;
            SmartRaycast.Reach = reach;
        }

        public override void OnUpdate (Camera camera, bool useSmartRaycasting) {
            //shooot raycast into the scene
            //if hit object, open tooltip on object name

            var ray = camera.ScreenPointToRay (Input.mousePosition);

            if (useSmartRaycasting) Smartcast (ray);
            else Raycast (ray);

            void Smartcast (Ray ray) {
                Debug.Log ("Smart");
                SmartRaycast.Ray = ray;
                if (SmartRaycast.Raycast ()) SetTooltip (SmartRaycast.Collider);
                else SetTooltip (null);
            }

            void Raycast (Ray ray) {
                Debug.Log ("Dumb");
                if (Physics.Raycast (ray, out RaycastHit hit, reach, mask)) {
                    SetTooltip (hit.collider);
                }
                else SetTooltip (null);
            }
        }

        public void SetTooltip (Collider c) {
            if (c) {
                var name = c.name;
                Indev.DebugTooltip.Current.SetPosition (Input.mousePosition);
                Indev.DebugTooltip.Current.SetLabelText (name);

            }
            else {
                Indev.DebugTooltip.Current.Hide ();
            }
        }

        public override void OnCloseTool () {
            Indev.DebugTooltip.Current.Hide ();
        }
    }
}