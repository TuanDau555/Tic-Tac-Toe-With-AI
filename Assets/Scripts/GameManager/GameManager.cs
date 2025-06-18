
using System;
using UnityEngine;
using UnityEngine.UI;

public enum TurnState
{
    XTurn,
    OTurn,
    GameOver
}


public class GameManager : Singleton<GameManager>
{
    // --- Singleton Pattern ---
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager is NULL!");
            }
            return _instance;
        }
    }

    public TurnState currentTurnState;

    
    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        currentTurnState = TurnState.XTurn; // Bắt đầu với lượt của X
    }
}