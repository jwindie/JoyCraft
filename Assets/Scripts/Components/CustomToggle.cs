using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace IdleHands.Components {
    public class CustomToggle : MonoBehaviour {

        private bool state = false;
        [field: SerializeField] public UnityEvent onToggleOn { get; set; }
        [field: SerializeField] public UnityEvent onToggleOff { get; set; }

        private void Awake () {
        }

        public void Toggle () {
            state = !state;
            if (state) onToggleOn?.Invoke ();
            else onToggleOff?.Invoke ();
        }
    }
}