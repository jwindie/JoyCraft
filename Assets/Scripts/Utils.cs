using UnityEngine;

namespace IdleHands {
    public static class Utils {
        public static bool LayerIsIncludedInLayerMask (LayerMask layerMask, int layer) {
            //returns if a bit is set
            return (layerMask & (1 << layer)) != 0;
        }
        public static float Remap (float input, Vector4 mapValues) {
            return Remap (input, mapValues[0], mapValues[1], mapValues[2], mapValues[3]);
        }
        public static float Remap (float input, float inputMin, float inputMax, float min, float max) {
            return min + (input - inputMin) * (max - min) / (inputMax - inputMin);
        }
        public static float Frac (float value) {
            return (float) System.Math.Truncate (value);
        }
        public static Vector2 Wobble (float time, float sinScale, float cosScale) {
            return new Vector2 (Mathf.Sin (time * sinScale), Mathf.Cos (time * cosScale));
        }

        /// <summary>
        /// Clamps a value between 0 and 1
        /// </summary>
        public static float Clamp01 (float value) {
            return Mathf.Clamp (value, 0, 1);
        }
        /// <summary>
        /// Clamps a value between 0 and 1
        /// </summary>
        public static int Clamp01 (int value) {
            return Mathf.Clamp (value, 0, 1);
        }
        public static void SetLayerOnObjects (int layer, GameObject[] objects) {
            for (int i = 0 ; i < objects.Length ; i++) {
                objects[i].layer = layer;
            }
        }
        /// <summary>
        /// Clamps a value by wrapping it around 
        /// </summary>
        public static float Wrap (float value, float min, float max) {
            if (value < min) value += min;
            if (value > max) value -= max;
            return value;
        }
        /// <summary>
        /// Clamps a value by wrapping it around 
        /// </summary>
        public static int Wrap (int value, int min, int max) {
            if (value < min) value += min;
            if (value > max) value -= max;
            return value;
        }
        public static Quaternion QuaternionSmoothDamp (Quaternion rot, Quaternion target, ref Quaternion deriv, float time) {
            if (Time.deltaTime < Mathf.Epsilon) return rot;
            // account for double-cover
            var Dot = Quaternion.Dot (rot, target);
            var Multi = Dot > 0f ? 1f : -1f;
            target.x *= Multi;
            target.y *= Multi;
            target.z *= Multi;
            target.w *= Multi;
            // smooth damp (nlerp approx)
            var Result = new Vector4 (
                Mathf.SmoothDamp (rot.x, target.x, ref deriv.x, time),
                Mathf.SmoothDamp (rot.y, target.y, ref deriv.y, time),
                Mathf.SmoothDamp (rot.z, target.z, ref deriv.z, time),
                Mathf.SmoothDamp (rot.w, target.w, ref deriv.w, time)
            ).normalized;

            // ensure deriv is tangent
            var derivError = Vector4.Project (new Vector4 (deriv.x, deriv.y, deriv.z, deriv.w), Result);
            deriv.x -= derivError.x;
            deriv.y -= derivError.y;
            deriv.z -= derivError.z;
            deriv.w -= derivError.w;

            return new Quaternion (Result.x, Result.y, Result.z, Result.w);
        }
        /// <summary>
        /// Shorthand for typing transform.parent.GetComponentInChildren()
        /// </summary>
        public static T GetComponentInSiblings<T> (this Transform t) {
            if (t.root == t) return default;
            return t.parent.GetComponentInChildren<T> ();
        }
        /// <summary>
        /// Shorthand for typing transform.parent.GetComponentsInChildren()
        /// </summary>
        public static T[] GetComponentsInSiblings<T> (this Transform t) {
            if (t.root == t) return default;
            return t.parent.GetComponentsInChildren<T> ();
        }
        public static float FrameAwareDamp (float a, float b, float lambda, float dt) {
            return Mathf.Lerp (a, b, 1 - Mathf.Exp (-lambda * dt));
        }

        /// <summary>
        /// Lerps between two input colors by t and returns the new color.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Color MixColor (Color a, Color b, float t) {
            return new Color (
                Mathf.Lerp (a.r, b.r, t),
                Mathf.Lerp (a.g, b.g, t),
                Mathf.Lerp (a.b, b.b, t),
                Mathf.Lerp (a.a, b.a, t)
                );
        }

        /// <summary>
        /// Swaps the alpha value of a color and returns te new color.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color SwapAlpha (this Color c, float alpha) {
            return new Color (c.r, c.g, c.b, alpha);
        }
    }

    public class Formatting {
        /// <summary>
        /// Formats an integer value. 
        /// <para>For example, 11,520 ==> 11K and 148,000,000 ==> 148M</para>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatIntValueKMB (int value) {
            int sign = value > 0 ? 1 : -1;
            int abs = Mathf.Abs (value);
            if (abs < 1000) {
                return (sign * abs).ToString ();
            }
            else if (abs < 1000000) {
                return (sign * (abs / 1000)).ToString () + "K";
            }
            else if (abs < 1000000000) {
                return (sign * (abs / 1000000)).ToString () + "M";
            }
            else return (sign * (abs / 1000000000)).ToString () + "B";
        }
    }
}