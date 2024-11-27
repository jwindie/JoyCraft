using System.Collections.Generic;

namespace JoyCraft.Global {
    public class Event {
        private List<System.Action> listeners = new List<System.Action> ();

        public void Invoke () {
            for (int i = listeners.Count - 1 ; i >= 0 ; i--)
                listeners[i]?.Invoke ();
        }
        public void RegisterListener (System.Action action) { listeners.Add (action); }
        public void UnregisterListener (System.Action action) { listeners.Remove (action); }
        public void UnregisterAllListeners () {
            listeners.Clear ();
        }
    }

    public class Event<T> {
        private List<System.Action<T>> listeners = new List<System.Action<T>> ();

        public void Invoke (T value) {
            for (int i = listeners.Count - 1 ; i >= 0 ; i--)
                listeners[i]?.Invoke (value);
        }
        public void RegisterListener (System.Action<T> action) { listeners.Add (action); }
        public void UnregisterListener (System.Action<T> action) { listeners.Remove (action); }
        public void UnregisterAllListeners () {
            listeners.Clear ();
        }
    }
    public class Event<T1, T2> {
        private List<System.Action<T1, T2>> listeners = new List<System.Action<T1, T2>> ();

        public void Invoke (T1 a, T2 b) {
            for (int i = listeners.Count - 1 ; i >= 0 ; i--)
                listeners[i]?.Invoke (a, b);
        }
        public void RegisterListener (System.Action<T1, T2> action) { listeners.Add (action); }
        public void UnregisterListener (System.Action<T1, T2> action) { listeners.Remove (action); }
        public void UnregisterAllListeners () {
            listeners.Clear ();
        }
    }
}