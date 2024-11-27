using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlugNPlay.Interaction {

    public class MultiClicker 
    {

        private class ClickInfo {
            public float lastClick;
            public int clickCount;
        }
        private float clickTime = .5f; //click delay in milliseconds

        private Dictionary <string, ClickInfo> clicktionary = new Dictionary<string, ClickInfo>();
        private HashSet<ClickInfo> activeClickInfos = new HashSet<ClickInfo>();
        private HashSet<ClickInfo> expiredClickInfos = new HashSet<ClickInfo>();

        /// <summary>
        /// Returns the number of clicks currently being updated.
        /// </summary>
        public int ActiveClickInfoCount {
            get {
                return activeClickInfos.Count;
            }
        }

        /// <summary>
        /// Sets the maximume amout of time in seconds for a multi click to be detected.
        /// </summary>
        /// <param name="f"></param>
        public void SetClickMaxTime(float f) {
            clickTime = f;
        }

        /// <summary>
        /// Records an input press.
        /// </summary>
        /// <param name="key"></param>
        public void RecordInput(string key) {
            if (clicktionary.ContainsKey (key)) {
                //query existing entry
                ClickInfo _ = clicktionary[key];
                //increment or check the entry
                _.lastClick = Time.time;
                _.clickCount ++;

                //add to active set
                //if not contained, add
                if (!activeClickInfos.Contains (_)) {
                    activeClickInfos.Add (_);
                }
            }
            else {
                //create a new entry
                ClickInfo _ = new ClickInfo();
                _.lastClick = Time.time;
                _.clickCount ++;

                clicktionary.Add (key, _);
                activeClickInfos.Add (_);
            }
        } 

        /// <summary>
        /// Updates all the timers.
        /// </summary>
        public void Tick() {
            foreach (ClickInfo _ in activeClickInfos) {
                //if its been too long since the last click reset the 
                //click count to 0
                if (Time.time - _.lastClick > clickTime){
                    _.clickCount = 0;
                    expiredClickInfos.Add (_);
                }
            }

            foreach (ClickInfo _ in expiredClickInfos) activeClickInfos.Remove (_);
            expiredClickInfos.Clear();
        }

        /// <summary>
        /// Retutns the number of clicks for a given saved input.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int QueryClicks (string key) {
            //if dict not have key, return 0
            if (!clicktionary.ContainsKey (key)) return 0;

            //if it does, return the clickCount
            else return clicktionary[key].clickCount;
        }

        public void ResetClicks (string key) {
            //if dict not have key, return 0
            if (clicktionary.ContainsKey (key)) {
                var _ = clicktionary[key];
                _.clickCount = 0;
                _.lastClick = 0;
                activeClickInfos.Remove (_);
            }
        }
    }
}