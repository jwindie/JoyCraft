using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace IdleHands.UI {
    public class ToolIndicator : JoyCraft.SingletonComponent<ToolIndicator> {

        [SerializeField] private TextBubble textBubble;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI label;

        [Header ("Sub Icon")]
        [SerializeField] private RectTransform subIconGroup;
        [SerializeField] private Image subIcon;
        [SerializeField] private float subIconRotation;

        [Header ("Close Icon")]
        [SerializeField] private RectTransform closeIconGroup;
        [SerializeField] private Image closeIcon;
        [SerializeField] private float closeIconRotation;

        private Animator mainIconAnimator;

        public void SetIconAndLabel (Sprite sprite, string message) {
            icon.enabled = true;
            mainIconAnimator.SetTrigger ("Pressed");
            icon.sprite = sprite;
            label.SetText (message);
            textBubble.SetLabelText (message);
        }

        public void ClearIcon () {
            icon.enabled = false;
            label.SetText ("No Tool");
        }

        private void OnSetInteractionTool (Interaction.InteractionTool tool) {
            SetIconAndLabel (tool.ToolIcon, tool.ToolName);
        }

        public void SetPrompt (string message) {
            //use the tooltip system
        }

        private void Awake () {
            instance = this;
            mainIconAnimator = icon.GetComponent<Animator> ();
            //GameEventHandler.Interaction.SetInteractionTool.RegisterListener (OnSetInteractionTool);
        }

#if UNITY_EDITOR
        private void OnValidate () {
            if (subIconGroup && subIcon) {
                subIconGroup.localRotation = Quaternion.Euler (0, 0, subIconRotation);
                subIcon.rectTransform.localRotation = Quaternion.Euler (0, 0, -subIconRotation);
            }

            if (closeIconGroup && closeIcon) {
                closeIconGroup.localRotation = Quaternion.Euler (0, 0, closeIconRotation);
                closeIcon.rectTransform.localRotation = Quaternion.Euler (0, 0, -closeIconRotation);
            }
        }
#endif
    }
}