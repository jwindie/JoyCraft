using JoyCraft;
using JoyCraft.Scene;

using System.Collections.Generic;
using UnityEngine;

public static class SimulationManager {

    public static HashSet<Node> nodes;

    [RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init () {

    }

    public static ISerialize[] GetSerializables () {

        return null;
    }
}