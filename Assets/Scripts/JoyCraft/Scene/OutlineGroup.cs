using System.Linq;
using UnityEngine;

namespace JoyCraft.Scene {
    
    public class OutlineGroup : MonoBehaviour {
        private const int DEFAULT_LAYER = 0;
        private const int HIGHLIGHT_LAYER = 7;
 
        [SerializeField] private GameObject[] outlineObjects = new GameObject[0];

        private OutlineGroup[] childOutlineGroups = new OutlineGroup[0];

        private void Awake() {
            childOutlineGroups = GetComponentsInChildren<OutlineGroup>().Where(c => c != this).ToArray();
        } 

        public void EnableOutline (bool state) {
            //enable outlining on the outline objects and pass the call to child groups
            if (state) foreach (GameObject o in outlineObjects) o.layer = HIGHLIGHT_LAYER;
            else foreach (GameObject o in outlineObjects) o.layer = DEFAULT_LAYER;
            
            for (int i = 0 ; i < childOutlineGroups.Length; i++) childOutlineGroups[i].EnableOutline(state);
        } 
    }
}