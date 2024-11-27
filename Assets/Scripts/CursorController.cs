using UnityEngine;

public class CursorController : JoyCraft.SingletonComponent<CursorController> {

    [field: SerializeField] public CursorPreset DefaultCursor { get; private set; }
    [field: SerializeField] public CursorPreset PanCursor { get; private set; }
    [field: SerializeField] public CursorPreset HandCursor { get; private set; }
    [field: SerializeField] public CursorPreset GrabCursor { get; private set; }
    [field: SerializeField] public CursorPreset MarqueeCursor { get; private set; }
    public void SetCursor (CursorPreset preset) {
        Cursor.SetCursor (preset.Icon, preset.Hotspot, CursorMode.Auto);
    }

    private void Start () {
        SetSingletonInstance (this);
        SetCursor (DefaultCursor);
    }
}
