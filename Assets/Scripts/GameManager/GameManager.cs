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
    public TurnState currentTurnState;

    void Start()
    {
        currentTurnState = TurnState.XTurn;
    }
}
