using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializedPrefabData : MonoBehaviour {

    public enum Group {
        House,
        Tree
    }

    public Group group;
    public int id;
    public new Transform transform;
}
