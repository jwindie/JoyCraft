using UnityEngine;

namespace JoyCraft.Scene {
    public abstract class Grabbit : MonoBehaviour {

        protected Vector3 grabbedPosition;
        protected bool grabbed;

        public Transform Transform {
            get {
                return transform;
            }
        }
        public virtual void OnUpdate (OnUpdateData data) { }
        public virtual void DisableOutline () {}
        public virtual void EnableOutline () {}
        public virtual Grabbit Grab (Vector3 mousePoint) {
            grabbed = true;
            grabbedPosition = transform.position;
            //calculate the offset position of the grabbit's center and the mouse position
            return this;
        }

        public virtual Grabbit Release () {
            //place objects back where they were grabbed from
            //if dropped out ofn bounds
            if (Context.Current.Workspace.IsWithinBounds (transform.position) == false) {
                transform.position = grabbedPosition;
            }
            grabbed = false;
            return this;
        }
        public void SetPosition (Vector3 position) {
            transform.position = position;
        }

        protected virtual void Start() {
        }
    }
}