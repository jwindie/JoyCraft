using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleHands.Interaction {

    /// <summary>
    /// Represents anything interactable by the player.
    /// </summary>
    public interface IInteractable {
        void BeginHover ();
        void EndHover ();
        void Select ();
        void Deselect ();
    }
}
