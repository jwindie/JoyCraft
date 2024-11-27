using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleHands.Components {
    public class FPS : MonoBehaviour {

        [SerializeField] private TMPro.TextMeshProUGUI label;
        [SerializeField] private int historyLength;

        float[] values;
        float time;
        int index;

        private void Awake () {
            values = new float[historyLength];
        }
        private void LateUpdate () {
            values[index] = Time.deltaTime;
            index++;
            time += Time.deltaTime;
            if (index == historyLength) index = 0;



            float average = values[0];
            for (int i = 1 ; i < historyLength ; i++) {
                average += values[i];
            }
            average /= historyLength;

            if (time > .1f) {
                label.SetText ((average * 1000).ToString ("##.## ms"));
                time -= .1f;
            }
        }
    }
}