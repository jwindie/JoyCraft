using UnityEngine;
using TMPro;
using JoyCraft;

public class TabSizer : MonoBehaviour {

    private static readonly float defaultSegmentSize = .2f;

    public Color color;
    public Color inactiveColor;
    public Color fontColor;    
    public Color inactiveFontColor;

    public float behindTint;
    
    public float width;
    public bool isActive;
    public MeshFilter leftSide;
    public TextMeshPro textMesh;

    public string displayName;

    public Mesh[] leftSides;
[SerializeField] private Transform[] parts;

    void OnValidate() {
        SetDisplayName(displayName);
    }

    public void SetDisplayName(string name) {
        textMesh.SetText(name);
        float _ = textMesh.GetPreferredValues().x;
        SetSize(_* 2);
    }
    public void SetSize(float f) {
        if (f < 0) f = 0;
        parts[1].localScale = new Vector3 (f/defaultSegmentSize,1f, 1f);
        parts[1].localPosition = new Vector3 (defaultSegmentSize, 0, 0);
        parts[2].localPosition = new Vector3 (defaultSegmentSize + (f/2f), 0, 0);
        width = f;
    }

    public void SetActive (bool state) {
        // leftSide.sharedMesh = state ? leftSides[0] : leftSides[1];
        textMesh.color = state ? fontColor : inactiveFontColor;
        
        var c = state ? color : inactiveColor;
        Colorize.SetColor (parts[0].GetComponent<MeshRenderer>(), c);
        Colorize.SetColor (parts[1].GetComponent<MeshRenderer>(), c);
        Colorize.SetColor (parts[2].GetComponent<MeshRenderer>(), c); 

        isActive = state;
    }
}