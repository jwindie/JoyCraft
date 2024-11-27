using UnityEngine;

namespace IdleHands.Components {
    public class Rotator : MonoBehaviour {
        public Vector3 axis;
        public float speed;

        // Update is called once per frame
        void Update () {
            transform.Rotate (axis, speed * Time.deltaTime, Space.Self);
        }
    }
}