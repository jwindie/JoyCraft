using JoyCraft.Scene.Experimental;
using UnityEditor;
using UnityEngine;

namespace JoyCraft {
    [CreateAssetMenu]
     public class ThemePreset : ScriptableObject {

        [field: SerializeField] public Color BackgroundColor {get; protected set;}      

        [field: SerializeField, Header ("Boundary")] public Color BoundaryColor {get; protected set;}      
        [field: SerializeField] public Color BorderColorA {get; protected set;}      
        [field: SerializeField] public Color BorderColorB {get; protected set;}      
        
        [field: SerializeField, Header ("Grid")] public Color GridColor {get; protected set;}        

        [field: SerializeField, Header ("Components")] public Color CardColor {get; protected set;}      
        [field: SerializeField] public Color CardOutlineColor {get; protected set;}        

        [Space (30)]
        [SerializeField] protected ThemePreset source;
        [SerializeField] protected bool copyFrom;


        public virtual void Load(ThemeController controller) {}
        public virtual void Unload(ThemeController controller) {}

#if UNITY_EDITOR
        protected virtual void OnValidate() {
            if (source && copyFrom) {
                Undo.RecordObject(this, "Copy Preset From");

                BackgroundColor = source.BackgroundColor;
                BoundaryColor = source.BoundaryColor;
                BorderColorA = source.BorderColorA;
                BorderColorB = source.BorderColorB;
                GridColor = source.GridColor;
                CardColor = source.CardColor;
                CardOutlineColor = source.CardOutlineColor;

                copyFrom = false;
            }
        }
#endif
    }
}