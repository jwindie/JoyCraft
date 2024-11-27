using JoyCraft.Global;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace JoyCraft.Application {

    public class App : JoyCraft.SingletonComponent<App> {        
        private const string DEFAULT_ROOT = "Application/Datapath/Logs";
        private const string LOG_ROOT = "Application/Datapath/Logs";

        private enum DatapathLocation { Persistent, Nonpersistent};

        //INSPECTOR
        [SerializeField] private DatapathLocation dataPathLocation;
        [SerializeField] private bool openDataPathOnValidate = false;

        private string dataPathStub;

        public LogHandler LogHandler {get; private set;}
        public FileHandler FileHandler {get; private set;}
        public InputHandler InputHandler {get; private set;}
        public GameManager Game {get; private set;}

        //Main Entry point into the game and progam here
        public void Awake() {
            SetSingletonInstance (this);
            LogHandler = new LogHandler($"{LOG_ROOT}/latest.log");

#if UNITY_EDITOR
            //LogHandler.LogToUnity (true);
#endif
            try {
                FileHandler = new FileHandler();
                InputHandler = new InputHandler();
                
                ValidateDataDirectory();
            }
            catch  (System.Exception e) {
                Debug.LogError ($"Issues on initialization: {e}");
                Debug.LogError ("CONTACT ALFRED ADMIN FOR WRITE PERMISSION ON DRIVE!!!");
            }
            //create bootstrap objects
            Game = new GameManager(GameManager.GameState.Workspace);
        }

        /// <summary>
        /// Create all the needed folders and subfolders that will be used for the app class and various handlers.
        /// </summary>
        private void ValidateDataDirectory() {
            LogHandler.Log("Validating app files");
            //root path
            if (!Directory.Exists ($"{dataPathStub}/{DEFAULT_ROOT}")) {
                Directory.CreateDirectory ($"{dataPathStub}/{DEFAULT_ROOT}");
            }
            //optionally open path
            if (openDataPathOnValidate) System.Diagnostics.Process.Start ($"{dataPathStub}/{DEFAULT_ROOT}");
        }

        private void ComposeDataPathStub() {
            if (dataPathLocation == DatapathLocation.Persistent) dataPathStub = $"{UnityEngine.Application.persistentDataPath}/";
            if (dataPathLocation == DatapathLocation.Nonpersistent) dataPathStub = $"{UnityEngine.Application.dataPath}/";
        }

        private void OnValidate() {
            name = "App";
            transform.SetSiblingIndex (0);
        }
    }
}