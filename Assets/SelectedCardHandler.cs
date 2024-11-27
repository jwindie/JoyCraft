using JoyCraft.Scene;
using UnityEngine;
using JoyCraft;
using System.Collections.Generic;

public class SelectedCardHandler : JoyCraft.Singleton<SelectedCardHandler> {

    private const float PICKUP_ROTATION = 6;
    private const float PICKUP_COLLIDER_SIZE_FACTOR = .45f;
    private static readonly Vector3 PICKUP_Z_OFFSET = new Vector3 (0, 0, 0);
    private static Transform helper;

    [SerializeField] private Card selection;
    private HashSet<Card> validOverlapingCards = new HashSet<Card> ();
    private List<Card> sortedOverlapingCards = new List<Card> ();
    private Card potentialParent;

    public bool HasSelection {
        get {
            return selection;
        }
    }

    public void SetSelection (Card card, Vector3 worldSpaceMousePosition) {
        selection = card.Grab (worldSpaceMousePosition) as Card;
        //selection can be overridden on grab
        if (selection == null) return;

        selection.BreakParentConnection ();
        selection.NotifySelectionStatus (true);
        selection.SetColliderAsTrigger (true);
        selection.ShrinkCollider (new Vector2 (PICKUP_COLLIDER_SIZE_FACTOR, PICKUP_COLLIDER_SIZE_FACTOR));
        selection.SetLabel ("?");
        selection.SetLayer (3);

        //orient the transform helper
        helper.transform.position = worldSpaceMousePosition;
        helper.transform.rotation = selection.transform.rotation;

        //place selection on helper
        selection.transform.SetParent (helper);
        helper.transform.localRotation = Quaternion.Euler (0, 0, GetPickupRotation ());
    }

    public void ClearSelection () {
        if (!selection) return;

        helper.transform.localRotation = Quaternion.Euler (0, 0, 0);

        selection.GrowCollider (new Vector2 (PICKUP_COLLIDER_SIZE_FACTOR, PICKUP_COLLIDER_SIZE_FACTOR));
        selection.transform.SetParent (null);
        selection.NotifySelectionStatus (false);
        selection.SetLabel ("-");
        selection.SetLayer (0);
        selection.Release ();

        if (potentialParent) ParentCard ();
        else selection.SetColliderAsTrigger (false);

        selection = null;

        PurgeOverlapCards ();

    }

    public void OnUpdate (OnUpdateData updateData) {
        if (updateData.snapToGrid) {
            updateData.position = Library.SnapToGrid (updateData.position, updateData.snapPositionResolution);

            //selection.transform.position = updateData.position + PICKUP_Z_OFFSET;
            selection.transform.position = Vector3.Lerp (
                selection.transform.position,
                updateData.position + PICKUP_Z_OFFSET,
                Library.LERP_SPEED * 2
            );
        }
        else {
            //helper.position = updateData.position + PICKUP_Z_OFFSET;
            helper.position = Vector3.Lerp (
                helper.position,
                updateData.position + PICKUP_Z_OFFSET,
                Library.LERP_SPEED * 2
            );
        }


        //sort the valid colliders OONLY if there is more than one
        if (validOverlapingCards.Count == 1) {
            foreach (Card c in validOverlapingCards) {
                c.SetLabel ("1");
                SetPotentialParent (c);
            }
        }
        else if (validOverlapingCards.Count > 1) {
            SortValidOverlapCards ();
        }
    }

    public void OnSelectedTriggerEnter (Card otherCard) {
        if (ValidateOverlappingCard (otherCard)) {
            validOverlapingCards.Add (otherCard);
            //selection.SetLabel (validOverlapingCards.Count.ToString ());

            //otherCard.EnableOverlapTriggerHighlight ();
        }
    }

    public void OnSelectedTriggerExit (Card otherCard) {
        if (validOverlapingCards.Contains (otherCard)) {
            validOverlapingCards.Remove (otherCard);
            otherCard.SetLabel ("-");

            if (otherCard == potentialParent) PurgePotentialParent ();
            //otherCard.DisableOverlapTriggerHighlight ();
        }
    }

    private bool ValidateOverlappingCard (Card card) {
        if (card.IgnoreOverlap) return false;
        if (selection.ParentCard && card == selection.ParentCard) return false;
        if (selection.ChildCard && card == selection.ChildCard) return false;
        if (selection.RootCard && card.RootCard == selection.RootCard) return false;

        return true;
    }

    private void Awake () {
        SetSingletonInstance (this);
        helper = new GameObject ("Card Transform Helper").transform;
    }

    private float GetPickupRotation () {
        if (Random.Range (0, 1f) < .5f) return PICKUP_ROTATION;
        else return -PICKUP_ROTATION;
    }

    private void PurgeOverlapCards () {
        //disable the triggeroutline on all colliders and clear the list
        //foreach (Card c in validOverlapingCards) {
        //    c.DisableOverlapTriggerHighlight ();
        //    c.SetLabel ("-");
        //}

        validOverlapingCards.Clear ();
        PurgePotentialParent ();
    }

    private void SortValidOverlapCards () {
        //prepare the list
        sortedOverlapingCards.Clear ();
        sortedOverlapingCards.AddRange (validOverlapingCards);

        sortedOverlapingCards.Sort ((a, b) => {
            float distA = (selection.transform.position - a.transform.position).sqrMagnitude;
            float distB = (selection.transform.position - b.transform.position).sqrMagnitude;
            return distA.CompareTo (distB);
        });

        //go through the list and assign the lables to the distance
        for (int i = 0 ; i < sortedOverlapingCards.Count ; i++) {
            if (i == 0) SetPotentialParent (sortedOverlapingCards[i]);
            sortedOverlapingCards[i].SetLabel ((i + 1).ToString ());
        }
    }

    private void SetPotentialParent (Card card) {
        if (card == potentialParent) return;
        if (card.ChildCard != null) return;

        if (potentialParent) PurgePotentialParent ();
        card.EnableOverlapTriggerHighlight ();
        potentialParent = card;
    }

    private void PurgePotentialParent () {
        if (potentialParent == null) return;

        potentialParent.DisableOverlapTriggerHighlight ();
        potentialParent.SetLabel ("-");
        potentialParent = null;
    }

    private void ParentCard () {
        selection.SetParent (potentialParent);
        selection.SetRoot (potentialParent.RootCard);
        potentialParent.SetChild (selection);
    }
}