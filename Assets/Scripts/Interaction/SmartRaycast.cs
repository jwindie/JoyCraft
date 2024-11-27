using UnityEngine;

public static class SmartRaycast {

    private static Ray m_ray;
    private static float m_reach;
    private static LayerMask m_mask;
    private static readonly RaycastHit[] hits = new RaycastHit[1];
    private static bool lastResult;
    private static bool needsRecast;

    public static Collider Collider { get; private set; }
    public static Vector3 Point { get; private set; }

    public static Vector2 UV { get; private set; }
    public static Ray Ray {
        get {
            return m_ray;
        }
        set {
            if (value.origin != m_ray.origin ||
                value.direction != m_ray.direction) {
                m_ray = value;
                needsRecast = true;
            }
        }
    }
    public static float Reach {
        get {
            return m_reach;
        }
        set {
            if (value == m_reach) return;
            m_reach = value;
            needsRecast = true;
        }
    }
    public static LayerMask Mask {
        get {
            return m_mask;
        }
        set {
            if (value == m_mask) return;
            m_mask = value;
            needsRecast = true;
        }
    }
    public static bool Raycast () {
        if (!needsRecast) return lastResult;

        needsRecast = false;
        return InternalRaycast ();
    }
    public static bool Raycast (Ray ray, LayerMask mask) {
        Ray = ray;
        Mask = mask;

        if (!needsRecast) return lastResult;

        needsRecast = false;
        return InternalRaycast ();
    }
    public static bool ForceRaycast () {

        needsRecast = false;
        return InternalRaycast ();
    }
    public static bool ForceRaycast (Ray ray, LayerMask mask) {
        Ray = ray;
        Mask = mask;

        needsRecast = false;
        return InternalRaycast ();
    }

    private static bool InternalRaycast () {
        int i = Physics.RaycastNonAlloc (m_ray, hits, m_reach, m_mask);

        if (i > 0) {
            Collider = hits[0].collider;
            Point = hits[0].point;
            UV = hits[0].textureCoord;

            return lastResult = true;
        }
        return lastResult = false;
    }
}