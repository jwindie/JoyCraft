using JoyCraft.Application;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace JoyCraft.Global {

    /// <summary>
    /// Events throughout the game. Local to the namespace.
    /// </summary>
    public class EventHandler {

        public EventHandler () {
            App.Current.LogHandler.Log("Initialized Event Handler");
        }

        public readonly Event OnClearEntities = new Event ();
        public readonly Event OnPauseWireSimulation = new Event();
        public readonly Event OnResumeWireSimulation = new Event();

        public readonly Event StartLoadProcess = new Event ();
        public readonly Event EndLoadProcess = new Event ();

        public readonly Event<float> SetLoadProgress = new Event<float> ();
        public readonly Event<string> SetLoadMessage = new Event<string> ();
        public readonly Event<bool> ShowLoadMessage = new Event<bool> ();

        public readonly Event<int> LoadScene = new Event<int> ();
        public readonly Event<UnityEngine.SceneManagement.Scene, LoadSceneMode> SceneLoaded = new Event<UnityEngine.SceneManagement.Scene, LoadSceneMode> ();
        public readonly Event<UnityEngine.SceneManagement.Scene> UnloadScene = new Event<UnityEngine.SceneManagement.Scene> ();        
        public readonly Event<string> LogToConsole = new Event<string> ();
    
        public readonly Event Quit = new Event ();
        public readonly Event<object> OnFailedToQuit = new Event<object> ();
    }
}