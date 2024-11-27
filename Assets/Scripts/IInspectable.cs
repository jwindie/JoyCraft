using UnityEngine;

namespace IdleHands {
    public interface IInspectable {
        GameObject GameObject { get; }
        Vector3 AnchorPoint { get; }

        public string InspectableInfo { get; }
    }
}