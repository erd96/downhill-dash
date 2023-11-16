using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;
    public static event Action<GameState> OnGameStateChange; // Event for GameState change 

    public float playerSpeed = 4f;
    public float maxSlope = 30f;
    private void Awake()
    {
        Instance = this; // Singleton
    }


    public void UpdateGameState(GameState newState)
    {
        State = newState; 

        switch (State)

        {
            case GameState.StartScreen:
                break;
            case GameState.StartGame:
                break;
            case GameState.GameOverScreen:
                break;
            default:
                break;
        }

        OnGameStateChange?.Invoke(newState); //
    }
    void Start()
    {
        UpdateGameState(GameState.StartGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum GameState
{
    StartScreen,
    StartGame,
    GameOverScreen

}