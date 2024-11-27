using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropertyTag : MonoBehaviour {
    [SerializeField] private List<PropertyTagName> tagTypes;

    private HashSet<PropertyTagName> tags;
    private void Awake () {
        HashTags ();
    }
    public void HashTags () {
        //add the tags to the has set but only distinct ones
        //ensures no issues of duplication
        tags = new HashSet<PropertyTagName> (tagTypes.Distinct ());
#if !UNITY_EDITOR
        tagTypes = null;
#endif
    }

#if UNITY_EDITOR
    public PropertyTagName[] GetTags () {
        return tagTypes.ToArray ();
    }
#endif
    public bool HasTag (PropertyTagName tag) {
        return tags.Contains (tag);
    }
    public void AddTag (PropertyTagName tag) {
        if (!tags.Contains (tag)) tags.Add (tag);
    }
    public void RemoveTag (PropertyTagName tag) {
        if (tags.Contains (tag)) tags.Remove (tag);
    }
}
