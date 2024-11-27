using UnityEngine;

namespace IdleHands.UI.Views {

    [RequireComponent (typeof (Canvas))]
    public class UIView : MonoBehaviour {

        protected Canvas canvas;

        [Header ("UI VIew Properties")]
        [Tooltip ("Will be ignored when handling stack transparency")]
        [SerializeField] protected bool ignoreInStack;
        [Tooltip ("Blocks UI Views underneath")]
        [SerializeField] protected bool opaque;
        [Tooltip ("Can be closed with the escape key")]
        [SerializeField] protected bool escapable;

        /// <summary>
        /// Does the view block things below it?
        /// </summary>
        public bool IsOpaque {
            get {
                return opaque;
            }
        }

        /// <summary>
        /// Is the UIView a modal?
        /// </summary>
        public virtual bool IsModal {
            get {
                return false;
            }
        }


        /// <summary>
        /// Can you hot escape out of the view?
        /// </summary>
        public bool IsEscapable {
            get {
                return escapable;
            }
        }

        /// <summary>
        /// Should the view be ignored when handling stack transparency?
        /// </summary>
        public bool IgnoreInStack {
            get {
                return ignoreInStack;
            }
        }

        /// <summary>
        /// Enables or disables the canvas.
        /// </summary>
        /// <param name="state"></param>
        public virtual void SetVisibility (bool state) {
            canvas.enabled = state;
        }

        protected virtual void Awake () {
            canvas = GetComponent<Canvas> ();
        }
    }
}
