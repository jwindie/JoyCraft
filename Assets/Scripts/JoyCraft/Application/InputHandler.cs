using UnityEngine;

namespace JoyCraft.Application {
    public class InputHandler {
        public InputHandler () {
            App.Current.LogHandler.Log ("Initialized Input Handler");
        }

        public bool ShiftKey {
            get {
                return Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);
            }
        }
        public bool ShiftKeyDown {
            get {
                return Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKeyDown (KeyCode.RightShift);
            }
        }

        public bool LeftMouse {
            get {
                return Input.GetMouseButton (0);
            }
        }
        public bool LeftMouseDown {
            get {
                return Input.GetMouseButtonDown (0);
            }
        }

        public bool LeftMouseUp {
            get {
                return Input.GetMouseButtonUp (0);
            }
        }

        public bool RightMouse {
            get {
                return Input.GetMouseButton (1);
            }
        }
        public bool RightMouseDown {
            get {
                return Input.GetMouseButtonDown (1);
            }
        }
        public bool RightMouseUp {
            get {
                return Input.GetMouseButtonUp(1);
            }
        }


        public bool PanButtonDown {
            get {
                return Input.GetMouseButtonDown (1);
            }
        }
        public bool PanButtonUp {
            get {
                return Input.GetMouseButtonUp (1);
            }
        }

        public float MouseScrollDelta {
            get {
#if UNITY_STANDALONE_OSX	              
                if (ShiftKey) return Input.mouseScrollDelta.x;
                else return Input.mouseScrollDelta.y;
#endif
                return Input.mouseScrollDelta.y;
            }
        }

    }
}