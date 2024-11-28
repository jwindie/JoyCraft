using JoyCraft.Application;
using JoyCraft.Scene;
using JoyCraft.Scene;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoyCraft.Global {
    public class GameManager {
        private const string RESOURCE_LIST_PATH = "Resources";

        public enum GameState {
            Menu,
            Workspace
        }

        private GameState currentState;
        private Context context;
        
        public EventHandler EventHandler {get; private set;}

        public GameManager (GameState initalState  = GameState.Menu) {
            App.Current.LogHandler.Log("Initialized Game Manager");
            EventHandler = new EventHandler();
            
            SetGameState (initalState);
        }

        private void SetGameState (GameState state) {
            currentState = state;
            App.Current.LogHandler.Log($"Game state set to: {state}");

            //depending on the game mode, create a context or delete it
            switch (state) {
                case GameState.Menu: 
                    DestroyContext();
                    break;

                case GameState.Workspace:
                    CreateContext();
                    break;
            }
        }

        private void CreateContext() {
            if (context) throw new System.Exception ("Attempted to make a context when one exists. This is not allowed");
            
            var _=  new GameObject ("Context", typeof (Context));
            _.transform.SetSiblingIndex (1);
            context  = _ .GetComponent<Context>();
            context.SetGameManager(this);
        }

        private void DestroyContext() {
            if (!context) return;

            GameObject.Destroy (context.gameObject);
        }

        //functions
        //has reference to the UI managers and other oftype managers
        //swap the gameState and reload/discard certain elements
    }
}