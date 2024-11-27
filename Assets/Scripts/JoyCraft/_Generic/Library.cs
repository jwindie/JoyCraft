using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace JoyCraft {
    public static class Library {

        public const float LERP_SPEED = .1f;

            private static readonly Color[] FrequencyColors = new Color[] {
                Color.white,
                new Color (0.3788f,0.6328495f, 0.8113208f, 1),      //blue
                new Color (0.8773585f, 0.415677f, 0.3931559f, 1),   //red
                new Color (0.4690144f, 0.8301887f, 0.4033464f, 1),  //green
                new Color (0.8203458f, 0.4660021f, 0.8301887f, 1),   //purple
                new Color (0.960784f, 0.8666667f, 0.258824f),   //yellow
                Color.black
            };

        public static float ManhattanDistance2D (Vector2 a, Vector2 b) {
            return Mathf.Abs (a.x - b.x) + Mathf.Abs (a.y - b.y);
        }

        public static Color GetFrequencyColor (int frequency) {
            return FrequencyColors[frequency];
        }

        public static Vector2 InverseLerp (Vector2 a, Vector2 b, Vector2 t) {
            if (a == b) return Vector2.zero;
            
            //do inverseLerp on each comp
            return new Vector2 (
                Mathf.InverseLerp (a.x, b.x, t.x),
                Mathf.InverseLerp (a.y, b.y, t.y)
            );
        }

        public static float SnapToClampedInterval(float value, float min, float max, float step)
        {
            float snappedValue = min + Mathf.Round((value - min) / step) * step;
            return Mathf.Clamp(snappedValue, min, max);
        }

        public static Vector3 SnapToGrid(Vector3 position, float gridSize) {
            float x = Mathf.Round(position.x / gridSize) * gridSize;
            float y = Mathf.Round(position.y / gridSize) * gridSize;
            float z = Mathf.Round(position.z / gridSize) * gridSize;
            return new Vector3(x, y, z);
        }

        public static float SnapToInterval (float value, float intervalSize) {
        return Mathf.Round(value / intervalSize) * intervalSize;
        }
    }
}