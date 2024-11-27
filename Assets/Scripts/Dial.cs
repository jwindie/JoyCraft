using JoyCraft;
using JoyCraft.Scene;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoyCraft.Scene {
    public class Dial : AbstractGrabbit {

        private const float ROTATION_PADDING = 22.5f;
        private const float USABLE_ROTATION = 360 - (ROTATION_PADDING * 2);

        private const float PRECISION = .002f;

        [SerializeField] int intervals;

        [SerializeField] bool clamped = false;
        [SerializeField] Transform rotationGroup;
        [SerializeField] private Animator animator;

        private float twistTarget, twist, step;

        protected void Start () {
            twistTarget = twist = 0;
            UpdateDialRotation ();
        }

        public override AbstractGrabbit Grab (Vector3 mousePoint) {
            return this;
        }
        public override AbstractGrabbit Release () {

            //if the idealZ and internal value are not the same, start a coroutine to continue lerping them
            if (Mathf.Abs (twist - twistTarget) > PRECISION) {
                StartCoroutine (ContinueUpdateKnob ());
            }
            return this;
        }

        private void OnDisable () {
            StopAllCoroutines ();
        }

        public override void OnUpdate (JoyCraft.Scene.OnUpdateData data) {

            //clamp the input value so its not too crazy
            if (intervals > 0) data.rotation = Mathf.Clamp (data.rotation, -1, 1);

            Rotate (data.rotation);
        }

        private void Rotate (float r) {
            if (r != 0) {

                //for snapped rotations
                if (intervals > 0) {

                    twistTarget += r;

                    if (clamped) {
                        twistTarget = Mathf.Clamp (twistTarget, 0, intervals);
                        step = USABLE_ROTATION / intervals;
                    }
                    else {
                        //use the entire 360 degrees if we are unclamped
                        step = 360f / intervals;
                    }
                }
                //for free rotation
                else {
                }
            }
            UpdateDialRotation ();
        }

        private void UpdateDialRotation () {
            //lerp the idealZ with the internal dial
            twist = Mathf.Lerp (twist, twistTarget, Library.LERP_SPEED);
            //update the actual rotation of the knob
            rotationGroup.localRotation = Quaternion.Euler (
                0,
                0,
                360 - (twist * step + ROTATION_PADDING)
            );
        }

        private IEnumerator ContinueUpdateKnob () {
            while (Mathf.Abs (twist - twistTarget) > PRECISION) {
                UpdateDialRotation ();
                yield return null;
            }
        }
    }
}