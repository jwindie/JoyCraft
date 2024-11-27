using JoyCraft.Scene;
using UnityEngine;

namespace JoyCraft.Global {
    [CreateAssetMenu]
    public class ResourceList : ScriptableObject {

        [field:SerializeField] public Material BackgroundMaterial {get; private set;} 
        [field:SerializeField] public Material BoundaryBorderMaterial {get; private set;} 
        [field:SerializeField] public Material HightlightGridMaterial {get; private set;} 
        [field:SerializeField] public Material NodeMaterial {get; private set;} 
        [field:SerializeField] public Material NodeDarkMaterial {get; private set;} 
        [field:SerializeField] public Material HandleMaterial {get; private set;} 
        [field:SerializeField] public Material HandleShadowMaterial {get; private set;} 
        [field:SerializeField] public Material HandleIndicatorMaterial {get; private set;} 
        [field:SerializeField] public Material WireMaterial {get; private set;} 
        [field:SerializeField] public Material SplitFlapMaterial {get; private set;} 
        [field:SerializeField] public Material MarqueeMaterial {get; private set;} 
        
        [field:SerializeField, Space ] public OutlineMeshesInLayerFeature Outline {get; private set;} 
        [field:SerializeField] public OutlineMeshesInLayerFeature HighlightOutline {get; private set;} 
        [field:SerializeField] public OutlineMeshesInLayerFeature MarqueeOutline {get; private set;} 
    }
}