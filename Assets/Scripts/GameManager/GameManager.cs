using UnityEngine;


public enum TurnState
{
    XTurn,
    OTurn,
    GameOver
}
{
    public TurnState currentTurnState;

    void Start()
    {
        currentTurnState = TurnState.XTurn;
    }
}
// Giả sử file GameManager.cs của bạn có dạng như thế này
// Hãy đảm bảo bạn có biến isGameOver
using UnityEngine;

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