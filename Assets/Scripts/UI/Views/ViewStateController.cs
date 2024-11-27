using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleHands.UI.Views {

    public class ViewStateController : JoyCraft.SingletonComponent<ViewStateController> {

        private const MenuView DEFAULT_VIEWSTATE = MenuView.Main;

        public enum MenuView {
            Main,
            ModeSelect,
            SaveSelect,
            ResourceBar,
            ResourceEditor,
            Logo,
            SettingsMenu,
            SettingsInGame,
            SaveGame,
            InGameDebugPanel
        }

        [SerializeField]
        private UIView logo,
            main,
            modeSelect,
            saveSelect,
            resourceBar,
            resourceEditor,
            InGameDebugPanel;

        [SerializeField] private CanvasGroup modalBlocker;

        [Header ("Options")]
        [SerializeField] private bool useEscapeToRevertStack;
        [SerializeField] private float modalBlockerAnimationSpeed;

        private LinkedList<UIView> viewStack = new LinkedList<UIView> ();
        private Stack<Transform> modalStack = new Stack<Transform> ();
        private Coroutine modalBlockerAnimationRoutine;

        /// <summary>
        /// Places a UI View onto the stack.
        /// </summary>
        public void PushView (UIView view) {

            //UnityEngine.Debug.Log ($"Break {view.name}");

            //push the view onto the stack
            viewStack.AddLast (view);


            view.SetVisibility (true);
            view.transform.SetParent (transform);
            view.transform.SetAsLastSibling ();

            //push the modal after we set its transform info
            if (view.IsModal) {
                OnPushModal (view.transform);
                view.transform.rotation = Quaternion.Euler (0, 0, Random.Range (-2, 2));
                view.transform.localScale = Vector3.one * Random.Range (.99f, 1.01f);
            }

            //handle the view stack
            HandleViewStackTransparency ();
        }

        /// <summary>
        /// Places a UI View onto the stack.
        /// </summary>
        public void PushMenuView (MenuView view) {
            PushView (GetViewRefrerence (view));
        }

        /// <summary>
        /// Removes a UI View from the stack.
        /// </summary>
        public void PopView () {
            //remove the view from the list and disable it
            var view = viewStack.Last.Value;
            view.SetVisibility (false);

            if (view.IsModal) OnPopModal ();

            //get the next topmost vierw
            var newTopView = viewStack.Last.Previous.Value;
            newTopView.transform.SetAsLastSibling ();

            //remove the view
            viewStack.RemoveLast ();


            //handle the view stack
            HandleViewStackTransparency ();
        }
        public void ClearAllViews () {
            foreach (UIView view in viewStack) {
                view.SetVisibility (false);
            }
            viewStack.Clear ();
            modalStack.Clear ();
            SetModalBlockerVisibility (false, false);
        }

        void Start () {
            //initially disable the canvas on all of the child canvas items
            //ignore first child as it should be the planet render feature
            //for (int i = 2 ; i < transform.childCount ; i++) {
            //    var _ = transform.GetChild (i).GetComponent<Canvas> ();
            //    if (_) _.enabled = false;
            //}

            //hide the modalBlocker
            SetModalBlockerVisibility (false, false);

        }
        void HandleViewStackTransparency () {
            //start at the topmost (last) view
            //if the view is opaque, disable all previous views
            //if not continue until end of list or opaque view is found

            bool foundOpaque = false;
            LinkedListNode<UIView> currentNode = viewStack.Last;
            for (int i = viewStack.Count - 1 ; i >= 0 ; i--) {
                if (currentNode.Value.IgnoreInStack) continue;

                if (!foundOpaque) {
                    //look for an opaque view
                    if (currentNode.Value.IsOpaque) {
                        currentNode.Value.SetVisibility (true);
                        foundOpaque = true;
                    }
                }
                else {
                    //set the view to be disable since we found an opaque previously
                    currentNode.Value.SetVisibility (false);
                }
                if (i > 0) currentNode = currentNode.Previous;
            }
        }
        void OnPushModal (Transform modal) {
            UnityEngine.Debug.Log ("Handling Modal");
            //place the modalBlocker as the transform underneath the view
            modalBlocker.transform.SetSiblingIndex (modal.GetSiblingIndex () - 1);
            if (modalStack.Count == 0) SetModalBlockerVisibility (true);
            modalStack.Push (modal);
            modal.name = $"Modal {modalStack.Count}";
            UnityEngine.Debug.Log ($"Modal Push(): {modalStack.Count}");
        }
        void OnPopModal () {
            //remove tha last modal
            //if there are modals o nthe stack
            //set the modal block sibling index top -1

            if (modalStack.Count > 0) {
                var modal = modalStack.Pop ();
                modal.name = $"Modal Pooled";

                //after popping what does our stack look like
                //if there are more modals, take the topmot one and shift the blocker
                if (modalStack.Count > 0) {
                    modalBlocker.transform.SetAsLastSibling ();
                }
                //nomore modals?
                //hide the blocker
                else {
                    SetModalBlockerVisibility (false);
                }
            }
            UnityEngine.Debug.Log ($"Modal Pop(): {modalStack.Count}");
        }
        void Update () {
            if (useEscapeToRevertStack && Input.GetKeyDown (KeyCode.Escape)) {
                var topView = viewStack.Last.Value;
                if (topView.IsEscapable) {
                    PopView ();
                }

                Debug.LogError ("FIX");
                //GameEventHandler.UI.TryEscapeUIView.Invoke (topView);
            }
        }
        void SetModalBlockerVisibility (bool state, bool animate = true) {

            //set values on the canvas group and start an animation coroutine
            if (state) {
                modalBlocker.blocksRaycasts = true;
                modalBlocker.alpha = 0;
                modalBlocker.interactable = true;

                //only animate if there is animaion speed > 0
                if (animate && modalBlockerAnimationSpeed > 0) {
                    if (modalBlockerAnimationRoutine != null) StopCoroutine (modalBlockerAnimationRoutine);
                    modalBlockerAnimationRoutine = StartCoroutine (LerpModalBlockerRoutine (1));
                }
                else {
                    modalBlocker.alpha = 1;
                }
            }
            else {
                modalBlocker.blocksRaycasts = false;
                modalBlocker.alpha = 1;
                modalBlocker.interactable = false;

                //only animate if there is animaion speed > 0
                if (animate && modalBlockerAnimationSpeed > 0) {
                    if (modalBlockerAnimationRoutine != null) StopCoroutine (modalBlockerAnimationRoutine);
                    modalBlockerAnimationRoutine = StartCoroutine (LerpModalBlockerRoutine (0));
                }
                else {
                    modalBlocker.alpha = 0;
                }
            }
        }
        UIView GetViewRefrerence (MenuView state) {
            switch (state) {
                case MenuView.Main: return main;
                case MenuView.ModeSelect: return modeSelect;
                case MenuView.SaveSelect: return saveSelect;
                case MenuView.ResourceBar: return resourceBar;
                case MenuView.ResourceEditor: return resourceEditor;
                case MenuView.Logo: return logo;
                case MenuView.InGameDebugPanel: return InGameDebugPanel;
                default: return null;
            }
        }
        IEnumerator LerpModalBlockerRoutine (float targetAlpha) {
            while (Mathf.Abs (modalBlocker.alpha - targetAlpha) > .001f) {
                modalBlocker.alpha = Utils.FrameAwareDamp (modalBlocker.alpha, targetAlpha, modalBlockerAnimationSpeed, Time.deltaTime);
                yield return null;
            }
            modalBlocker.alpha = targetAlpha;
        }
    }
}