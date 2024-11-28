using UnityEngine;

namespace JoyCraft.Scene {
    public class SpawnCard : Card {
        [SerializeField] private Card cardPrefab;
        [SerializeField] private Color[] colors;

        // Unity Lifecycle Methods
        protected override void Start () {
            base.Start ();
            SetColor (Color.white);
            SetLabel ("X");
            rb.isKinematic = true;
            IgnoreOverlap = true;
        }

        private void OnTriggerEnter2D (Collider2D collision) {
            return;
        }

        private void OnTriggerExit2D (Collider2D collision) {
            return;
        }

        public override Grabbit Grab (Vector3 mousePoint) {
            return SpawnNewCardInstead ();
        }

        private Card SpawnNewCardInstead () {
            int index = Random.Range (0, colors.Length * 2);
            if (index >= colors.Length) index = 0;


            Card c = Instantiate (cardPrefab);
            c.gameObject.AddComponent<Colorize> ();
            c.TriggerStart ();
            c.SetColor (colors[index]);
            c.transform.position = transform.position;

            //disable outline on object
            DisableOutline ();
            return c;
        }
    }
}
