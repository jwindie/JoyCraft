using UnityEngine;

[CreateAssetMenu]
public class CursorPreset : ScriptableObject {
    [field: SerializeField] public Texture2D Icon { get; private set; }
    [field: SerializeField] public Vector2 Hotspot { get; private set; }
}
