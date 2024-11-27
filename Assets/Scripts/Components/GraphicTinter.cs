using UnityEngine;
using UnityEngine.UI;


namespace IdleHands.Components {
#if UNITY_EDITOR
    public class GraphicTinter : MonoBehaviour {
        [SerializeField] private Color color;

        [SerializeField] private Graphic[] graphics;

        private void OnValidate () {
            foreach (Graphic g in graphics) g.color = color;
        }
    }
#endif
}