using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;
    public float terrainCount = 0f;
    public static event Action<GameState> OnGameStateChange; // Event for GameState change 

    public float playerSpeed = 0.5f;
    public float maxSlope = 20f;

    public float playerTrackLeftZ;
    public float playerTrackMiddleZ;
    public float playerTrackRightZ;



    private void Awake()
    {
        Instance = this; 
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

        OnGameStateChange?.Invoke(newState); 
    }
    void Start()
    {
        UpdateGameState(GameState.StartGame);
    }


}

public enum GameState
{
    StartScreen,
    StartGame,
    GameOverScreen

}