using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour {
    public static GameStateManager instance;
    public GameObject[] gameStates;  

    public GameObject activeState;


    void Start() {
        if (instance == null) instance = this;

        foreach (GameObject gameState in gameStates) {
           if(gameState != activeState) gameState.SetActive(false);
        }
    }


    public void SwitchToGameState(string stateName) {
        foreach (GameObject gameState in gameStates) {
            if (gameState.name == stateName) {
                activeState.SetActive(false);
                activeState = gameState;
                activeState.SetActive(true);
                return;
            }
        }

        Debug.LogWarning("GameStateManager: Game state with name '" + stateName + "' not found!");
    }

    public void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}