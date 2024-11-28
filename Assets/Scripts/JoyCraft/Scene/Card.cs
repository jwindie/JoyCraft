using System.Collections;
using UnityEngine;

namespace JoyCraft.Scene {
    public class Card : Grabbit {
        // Constants
        private static readonly Vector3 PARENT_POSITION_OFFSET = new Vector3 (0, -0.4f, -0.02f);
        private static MaterialPropertyBlock propBlock;

        // Serialized Fields
        [SerializeField] private TMPro.TextMeshProUGUI label;
        [SerializeField] private MeshRenderer borderRenderer;

        // Private Fields
        private Animator animator;
        private Vector3 grabOffset;
        protected Rigidbody2D rb;
        private new BoxCollider2D collider;

        // Private Flags
        private bool lerpToZero = false;
        private bool selected = false;
        private Color color;
        private bool started = false;

        //pubic Properties
        public Card ParentCard { get; private set; }
        public Card ChildCard { get; private set; }
        public Card RootCard { get; private set; }
        public bool IgnoreOverlap { get; protected set; } = false;


        public Color Color {
            get {
                return color;
            }
        }

        // Unity Lifecycle Methods
        protected override void Start () {
            if (started) return;

            base.Start ();
            animator = GetComponent<Animator> ();
            rb = GetComponent<Rigidbody2D> ();
            collider = GetComponent<BoxCollider2D> ();

            label.SetText ("-");
            if (propBlock == null) propBlock = new MaterialPropertyBlock ();
            color = GetComponent<Colorize> ().Color;

            //set root card to itself
            RootCard = this;
            started = true;
        }

        private void Update () {

            if (!ParentCard && lerpToZero) {
                transform.position = Vector3.Lerp (
                    transform.position,
                    new Vector3 (transform.position.x, transform.position.y, 0),
                    Time.deltaTime * 20);
            }

            if (ChildCard) ChildCard.OnUpdatePosition ();
        }

        private void OnTriggerEnter2D (Collider2D collision) {
            if (!selected) return;

            if (collision.TryGetComponent (out Card otherCard)) {
                CardHandler.Current.OnSelectedTriggerEnter (otherCard);
            }
        }

        private void OnTriggerExit2D (Collider2D collision) {
            if (!selected) return;

            if (collision.TryGetComponent (out Card otherCard)) {
                CardHandler.Current.OnSelectedTriggerExit (otherCard);
            }
        }

        // Public Methods
        public override void EnableOutline () {
            SetMatPropBlockOnBorder (1, -1);
            if (ChildCard) ChildCard.EnableOutline ();
        }

        public override void DisableOutline () {
            SetMatPropBlockOnBorder (0, -1);
            if (ChildCard) ChildCard.DisableOutline ();
        }

        public void EnableOverlapTriggerHighlight () {
            SetMatPropBlockOnBorder (-1, 1);
        }

        public void DisableOverlapTriggerHighlight () {
            SetMatPropBlockOnBorder (-1, 0);
        }

        public void TriggerStart () {
            Start ();
        }

        public override Grabbit Grab (Vector3 mousePoint) {
            selected = true;
            AnimateWiggle ();
            lerpToZero = false;
            return base.Grab (mousePoint - grabOffset);
        }

        public override Grabbit Release () {
            selected = false;
            lerpToZero = true;
            AnimateWiggle ();
            return base.Release ();
        }

        public void SetColliderAsTrigger (bool state) {
            collider.isTrigger = state;
        }

        public void SetParent (Card parent) {
            ParentCard = parent;
        }

        public void SetChild (Card child) {
            ChildCard = child;
        }

        public void SetRoot (Card root) {
            RootCard = root;
            //GetComponent<Colorize> ().SetColor (root.Color);
            if (ChildCard) ChildCard.SetRoot (root);
        }

        public void SetLayer (int layer) {
            gameObject.layer = layer;
            borderRenderer.gameObject.layer = layer;
            label.gameObject.layer = layer;
            label.transform.parent.gameObject.layer = layer;
            if (ChildCard) ChildCard.SetLayer (layer);
        }

        public void BreakParentConnection () {
            if (ParentCard) {
                ParentCard.SetChild (null);
                ParentCard = null;
                SetRoot (this);
            }
        }

        public void SetLabel (string label) {
            this.label.SetText (label);
        }

        public void SetColor (Color color) {
            this.color = color;
            GetComponent<Colorize> ().SetColor (color);
        }

        public void ShrinkCollider (Vector2 factor) {
            collider.size -= factor;
        }

        public void GrowCollider (Vector2 factor) {
            collider.size += factor;
        }
        public void NotifySelectionStatus (bool state) {
            selected = state;
        }

        public void OnUpdatePosition () {
            transform.position = Vector3.Lerp (
                transform.position,
                ParentCard.transform.position + PARENT_POSITION_OFFSET,
                Time.deltaTime * 60f);
        }

        public void ClearChildCard () {
            ChildCard = null;
            StopAllCoroutines ();
        }

        // Private Helper Methods
        private void AnimateWiggle () {
            animator.ResetTrigger ("Wiggle");
            animator.SetTrigger ("Wiggle");
            if (ChildCard) StartCoroutine (WiggleChildWithDelay (.02f));
            if (ParentCard) {
                animator.ResetTrigger ("WiggleChild");
                animator.SetTrigger ("WiggleChild");
            }
        }

        private char GenerateRandomUppercaseLetter () {
            System.Random random = new System.Random ();
            return (char) random.Next (65, 91); // ASCII range for uppercase letters: 65 (A) to 90 (Z)
        }

        private void SetMatPropBlockOnBorder (int highlight, int interact) {
            borderRenderer.GetPropertyBlock (propBlock);
            if (highlight >= 0) propBlock.SetInt ("_Highlight", highlight);
            if (interact >= 0) propBlock.SetInt ("_Interact", interact);
            borderRenderer.SetPropertyBlock (propBlock);
        }

        private IEnumerator WiggleChildWithDelay (float delay) {
            yield return new WaitForSecondsRealtime (delay);
            ChildCard.AnimateWiggle ();
        }
    }
}
