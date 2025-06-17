using System;
using UnityEngine;


public enum TurnState
{
    XTurn,
    OTurn,
    GameOver
}

public class GameManager : Singleton<GameManager>
{
    public TurnState currentTurnState;

    internal void ShowResult(string v)
    {
        throw new NotImplementedException();
    }

    void Start()
    {
        currentTurnState = TurnState.XTurn;
    }
}
