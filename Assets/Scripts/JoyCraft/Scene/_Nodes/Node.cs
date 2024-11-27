using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoyCraft.Scene {
    public class Node : AbstractGrabbit, ISerialize {

        private static Transform nodeRotateHelper;

        [SerializeField] Animator animator;

        private Vector3 grabOffset;

        public object[] Serialize () {
            return null;
        }

        public void Deserialize (object[] data) {
        }

        void Start () {
            nodeRotateHelper = new GameObject ("Node Helper").transform;
        }

        public override AbstractGrabbit Grab (Vector3 mousePoint) {

            //orient the node rotation helper
            nodeRotateHelper.transform.position = mousePoint;
            nodeRotateHelper.transform.rotation = transform.rotation;
            transform.SetParent (nodeRotateHelper);

            if (Config.Animation.NODE_GRAB_RELEASE) AnimateWiggle ();

            return base.Grab (mousePoint - grabOffset);
        }

        public override AbstractGrabbit Release () {
            transform.SetParent (null);
            if (Config.Animation.NODE_GRAB_RELEASE) AnimateWiggle ();

            return base.Release ();
        }

        public override void OnUpdate (JoyCraft.Scene.OnUpdateData data) {

        }

        public void AnimateWiggle () {
            animator.ResetTrigger ("Wiggle");
            animator.SetTrigger ("Wiggle");
        }
    }
}