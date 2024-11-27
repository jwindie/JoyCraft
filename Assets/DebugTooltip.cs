using JoyCraft.Application;
using JoyCraft.Scene;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace IdleHands.Indev {
    public class DebugTooltip : JoyCraft.SingletonComponent<DebugTooltip> {

        [SerializeField] private Vector2 offset;
        [SerializeField] private RectTransform bubble;
        [SerializeField] private TMPro.TextMeshProUGUI labelText;

        [Header ("Animation")]
        [SerializeField] private float animationSpeed;

        private RectTransform anchor;
        private Graphic bubbleGraphic;
        private Canvas canvas;

        private float targetBubbleSize;
        private float startingBubbleHeight;
        private bool visible = true;
        private bool fixedPosition;
        private Vector3 fixedWorldPosition;

        private Coroutine routine_lerpBubbleSize, routine_fixPosition;

        public void SetPosition (Vector3 position) {
            fixedPosition = false;
            anchor.anchoredPosition = position + (Vector3) offset;
        }

        /// <summary>
        /// Fixes the tooltip to a point in space rather than the mouse position.
        /// </summary>
        /// <param name="worldPosition"></param>
        public void SetPositionFixed (Vector3 worldPosition) {
            fixedWorldPosition = worldPosition;
            fixedPosition = true;
            if (routine_fixPosition != null) StopCoroutine (routine_fixPosition);
            routine_fixPosition = StartCoroutine (StickToFixedWorldPosition ());
        }

        public void SetLabelText (string message) {
            if (!visible) Show ();
            labelText.SetText (message);
            labelText.ForceMeshUpdate (true, true);
            RefreshTooltipSize ();
        }

        public void Hide () {
            if (!visible) return;
            StopAllCoroutines ();
            canvas.enabled = false;
            visible = false;
            fixedPosition = false;
        }

        public void Show () {
            if (visible) return;
            labelText.enabled = true;
            canvas.enabled = true;
            visible = true;
        }

        private void Awake () {
            instance = this;
            anchor = GetComponent<RectTransform> ();
            bubbleGraphic = bubble.GetComponent<Graphic> ();
            startingBubbleHeight = bubble.sizeDelta.y;
            canvas = GetComponent<Canvas> ();
        }

        private void RefreshTooltipSize () {
            Vector2 textBounds = labelText.textBounds.size;
            Vector2 padding = new Vector2 (20, 9);
            var targetWindowSize = textBounds + padding;
            targetBubbleSize = targetWindowSize.x;
            if (routine_lerpBubbleSize != null) StopCoroutine (routine_lerpBubbleSize);
            routine_lerpBubbleSize = StartCoroutine (LerpBubbleSize ());
        }
        private void Start () {
            Hide ();
        }
        private void Close () {
            StopAllCoroutines ();
            StartCoroutine (CloseAnimation ());
        }

        /// <summary>
        /// Sets the bubble size and adjust it's position to remain relative.
        /// </summary>
        /// <param name="size"></param>
        private void SetBubbleSize (Vector2 size) {
            bubble.sizeDelta = size;
            bubble.anchoredPosition = bubble.sizeDelta / 2f + new Vector2 (-4, +5);
        }

        private IEnumerator LerpBubbleSize () {
            while (Mathf.Abs (bubble.sizeDelta.x - targetBubbleSize) > .001f) {
                var size = Mathf.Lerp (bubble.sizeDelta.x, targetBubbleSize, animationSpeed * Time.deltaTime);
                SetBubbleSize (new Vector2 (size, startingBubbleHeight));
                yield return null;
            }
        }

        private IEnumerator StickToFixedWorldPosition () {
            while (fixedPosition) {
                var screenPosition = Context.Current.CameraController.MainCamera.WorldToScreenPoint (fixedWorldPosition);
                anchor.anchoredPosition = screenPosition;
                //Debug.Log ("Fixing position");
                yield return null;
            }
        }

        private IEnumerator CloseAnimation () {
            int phase = 0;
            labelText.enabled = false;
            while (bubble.sizeDelta.x > startingBubbleHeight) {
                var size = bubble.sizeDelta;
                size.x = Mathf.Lerp (size.x, 30, animationSpeed * Time.deltaTime);
                if (size.x <= startingBubbleHeight) {
                    size.x = startingBubbleHeight;
                    phase++;
                }
                SetBubbleSize (size);
                yield return null;
            }
            float wait = .05f;
            while (phase == 1) {
                wait -= Time.deltaTime;
                if (wait <= 0) {
                    phase++;
                    bubbleGraphic.enabled = false;
                }
                yield return null;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos () {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere (transform.position - (Vector3) offset, 3f);
        }
#endif
    }
}