using UnityEngine;

namespace JoyCraft.Scene {
    public struct OnUpdateData {
        
        public bool LMB;
        public bool snapToGrid;

        public float rotation;
        public float snapPositionResolution;
        public float snapRotationResolution;

        public Vector3 position;
    }
}