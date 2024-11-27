using IdleHands.UI;
using UnityEngine;

namespace IdleHands.Interaction {
    public abstract class InteractionTool : ScriptableObject {

        [field: SerializeField]
        public virtual string ToolName { get; protected set; }

        [field: SerializeField]
        public virtual Sprite ToolIcon { get; protected set; }
        public abstract void OnUpdate (Camera camera, bool useSmartRaycasting = true);
        public virtual void OnOpenTool () { }
        public virtual void OnCloseTool () { }
        public virtual void OnDrawGizmos () { }
    }
}