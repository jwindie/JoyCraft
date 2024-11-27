using JoyCraft.Application;
using JoyCraft.Global;

namespace JoyCraft.Scene {
    /// <summary>
    /// Access pointto scene objects to access the gameManger and other scripts in the Global space.
    /// </summary>
    public class Context : SingletonComponent<Context> {

        public GameManager Game {get; private set;}
        public Workspace Workspace {get; private set;}
        public CameraController CameraController {get; private set;}

        public Context() {
            SetSingletonInstance (this);
        }

        public void SetGameManager(GameManager gameManager) {
            Game = gameManager;
        }

        private void Awake() {
            App.Current.LogHandler.Log ("Created new context");

            Workspace = FindObjectOfType<Workspace> ();
            CameraController = FindObjectOfType<CameraController>();
        }
    }
}