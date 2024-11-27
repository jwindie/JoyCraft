using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkChain : MonoBehaviour
{
    [Range (5, 20)] public int links;
    public bool enableCollision;
    public float colliderSize;
}
