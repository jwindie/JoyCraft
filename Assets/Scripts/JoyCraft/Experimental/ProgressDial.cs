using UnityEngine;
using UnityEngine.UI;

public class ProgressDial : MonoBehaviour {

    [SerializeField] private Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private int lastIndex;

    public void SetProgress (float value) {
        if (value > 1) value /= 100;
        int index = Mathf.FloorToInt ((sprites.Length - 1) * value);
        Debug.Log (Mathf.FloorToInt ((sprites.Length - 1) * value));
        Debug.Log (index);
        SetSprite (index);
    }

    private void SetSprite (int index) {
        if (index == lastIndex) return;
        spriteRenderer.sprite = sprites[index];
        animator.ResetTrigger ("Wiggle");
        animator.SetTrigger ("Wiggle");
        lastIndex = index;
    }

    private void Start () {
        spriteRenderer = GetComponent<SpriteRenderer> ();
        animator = GetComponent<Animator> ();
    }
}