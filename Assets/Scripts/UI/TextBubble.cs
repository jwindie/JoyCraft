using System.Collections;
using UnityEngine;

namespace IdleHands.UI {
    public class TextBubble : MonoBehaviour {


        [SerializeField] private Vector2 offset;
        [SerializeField] private float height;
        [SerializeField] private RectTransform bubble;
        [SerializeField] private TMPro.TextMeshProUGUI labelText;
        [SerializeField] private TextAnchor alignment;
        [Header ("Animation")]
        [SerializeField] private float animationSpeed;


        private float targetBubbleSize;

        public void SetLabelText (string message) {
            labelText.SetText (message);
            labelText.ForceMeshUpdate (true, true);
            RefreshTooltipSize ();
            StartCoroutine (LerpBubbleSize ());
        }

        private void RefreshTooltipSize () {
            Vector2 textBounds = labelText.textBounds.size;
            Vector2 padding = new Vector2 (20, 9);
            var targetWindowSize = textBounds + padding;
            targetBubbleSize = targetWindowSize.x;
        }

        /// <summary>
        /// Sets the bubble size and adjust it's position to remain relative.
        /// </summary>
        /// <param name="size"></param>
        private void SetBubbleSize (Vector2 size) {
            bubble.sizeDelta = size;

            //calcualte the position of the bubble
            Vector2 half = size / 2f;
            Vector2 pos = Vector2.zero;
            switch (alignment) {
                case TextAnchor.UpperLeft:
                    pos = -half;
                    break;
                case TextAnchor.UpperCenter:
                    pos = new Vector2 (0, -half.y);
                    break;
                case TextAnchor.UpperRight:
                    pos = new Vector2 (half.x, -half.y);
                    break;

                case TextAnchor.MiddleLeft:
                    pos = new Vector2 (half.x, 0);
                    break;
                case TextAnchor.MiddleCenter:
                    pos = Vector2.zero;
                    break;
                case TextAnchor.MiddleRight:
                    pos = new Vector2 (-half.x, 0);
                    break;

                case TextAnchor.LowerLeft:
                    pos = half;
                    break;
                case TextAnchor.LowerCenter:
                    pos = new Vector2 (0, half.y);
                    break;
                case TextAnchor.LowerRight:
                    pos = new Vector2 (-half.x, half.y);
                    break;

            }
            bubble.anchoredPosition = pos + offset;
            //bubble.anchoredPosition = new Vector2 (bubble.sizeDelta.x / 2f, 0) + offset;
        }

        private IEnumerator LerpBubbleSize () {
            //Debug.Log ("Lerping text bubble");
            while (Mathf.Abs (bubble.sizeDelta.x - targetBubbleSize) > .001f) {
                var size = Mathf.Lerp (bubble.sizeDelta.x, targetBubbleSize, animationSpeed * Time.deltaTime);
                SetBubbleSize (new Vector2 (size, height));
                yield return null;
            }
            SetBubbleSize (new Vector2 (targetBubbleSize, height));
        }

#if UNITY_EDITOR
        private void OnDrawGizmos () {
            Gizmos.DrawSphere (transform.position, 3f);
        }
#endif
    }
}