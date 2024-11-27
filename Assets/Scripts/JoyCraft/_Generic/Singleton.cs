using UnityEngine;

namespace JoyCraft {

    public abstract class Singleton<T> : MonoBehaviour where T : class {
        protected static T instance;

        public static T Current {
            get {
                return instance;
            }
        }

        public static void SetSingletonInstance (T item) {
            instance = item;
        }
    }
}