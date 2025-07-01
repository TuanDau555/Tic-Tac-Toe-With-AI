using System;
using System.Collections;
using UnityEngine;

public enum TurnState
{
    XTurn,
    OTurn,
    GameOver
}

public enum GameMode
{
    PVP,
    PVE
}

public class GameManager : Singleton<GameManager>
{
    #region Variable
    [Tooltip("Which Type of Player is playing")]
    public TurnState currentTurnState;
    [Tooltip("PVP: PlayerVsPlayer; PVE: PlayerVsAI")]
    public GameMode currentMode = GameMode.PVP;

    [SerializeField] private int pointToWin;
    [SerializeField] private int turnCount;
    [SerializeField] private float aiThinkingTime = 0.3f;
    private string resultText;
    #endregion

    #region Main Method
    void Start()
    {
        currentTurnState = TurnState.XTurn;
        pointToWin = BoardManager.Instance.boardSize >= 5 ? 5 : 3;
    }
    #endregion

    #region Turn Process
    public void ProcessTurn(int row, int column, int currentPlayer)
    {
        
        // Win
        if (CheckForWinnerPlayer(row, column, currentPlayer))
        {
            AnnounceWinner(currentPlayer);
            UpdateState(TurnState.GameOver);
            return;
        }
        // Draw
        else if (turnCount >= BoardManager.Instance.boardSize * BoardManager.Instance.boardSize)
        {
            AnnounceDraw();
            UpdateState(TurnState.GameOver);
            return;
        }
        // Not found Winner
        else
        {
            TurnState nextTurn = (currentPlayer == 1) ? TurnState.OTurn : TurnState.XTurn;
            UpdateState(nextTurn);
            turnCount++;
        }
    }

    private void UpdateState(TurnState newState)
    {
        currentTurnState = newState;
        if (currentMode == GameMode.PVE && newState == TurnState.OTurn)
        {
            StartCoroutine(PlayAITurn());
        }
    }
    #endregion

    #region Player With AI
    private IEnumerator PlayAITurn()
    {
        yield return new WaitForSeconds(aiThinkingTime);
        
        // Start calculate move here 
        Vector2Int aiMove = AIPlayer.GetBestMove(
            BoardManager.Instance.boardCells, 
            BoardManager.Instance.boardSize
        );

        // Validate move 
        if (aiMove.x >= 0 && aiMove.y >= 0)
        {
            if (BoardManager.Instance.boardCells[aiMove.x, aiMove.y] == 0)
            {
                Debug.Log($"[AI] Đánh tại vị trí ({aiMove.x}, {aiMove.y})");
                BoardManager.Instance.SetAICell(aiMove.x, aiMove.y);
            }
            else
            {
                Debug.LogError($"[AI] Lỗi: Vị trí ({aiMove.x}, {aiMove.y}) đã có quân!");
                FindAndPlayFallbackMove();
            }
        }
        else
        {
            Debug.LogError("[AI] Không tìm được nước đi hợp lệ!");
            FindAndPlayFallbackMove();
        }
    }


    /// <summary>
    /// Find and play alternative if AI didn't find a valid move
    /// </summary>
    private void FindAndPlayFallbackMove()
    {
        for (int row = 0; row < BoardManager.Instance.boardSize; row++)
        {
            for (int col = 0; col < BoardManager.Instance.boardSize; col++)
            {
                if (BoardManager.Instance.boardCells[row, col] == 0)
                {
                    BoardManager.Instance.SetAICell(row, col);
                    Debug.Log($"AI played fallback move at ({row}, {col})");
                    return;
                }
            }
        }
        Debug.LogError("No empty cells available for fallback move!");
    }
    #endregion

    #region Game Over Announce

    private void AnnounceWinner(int playerWin)
    {
        if (playerWin == 1)
        {
            resultText = "X Wins!";
            GameOverUI.Instance.ShowGameOver(resultText, Color.green);
        }
        else
        {
            resultText = "O Wins!";
            GameOverUI.Instance.ShowGameOver(resultText, Color.red);
        }
    }

    private void AnnounceDraw()
    {
        resultText = "It's a Tie!";
        GameOverUI.Instance.ShowGameOver(resultText, Color.yellow);
    }
    #endregion

    #region Rematch

    /// <summary>
    /// This function is called when Rematch button is clicked
    /// </summary>
    public void OnRematchClick()
    {
        StartRematch();
    }

    private void StartRematch()
    {
        // Reset the logic
        for (int row = 0; row < BoardManager.Instance.boardSize; row++)
        {
            for (int column = 0; column < BoardManager.Instance.boardSize; column++)
            {
                BoardManager.Instance.boardCells[row, column] = 0;
            }
        }

        // Reset player's last move tracking
        BoardManager.Instance.playerLastRow = -1;
        BoardManager.Instance.playerLastCol = -1;

        // Reset UI
        BoardManager.Instance.ResetBoard();

        // Update states
        UpdateState(TurnState.XTurn);
        turnCount = 0;

        // Hide game over UI
        GameOverUI.Instance.Hide();
    }

    #endregion

    #region Win Condition

    /// <summary>
    /// Logic to check if there is a winner.
    /// This method will check all rows, columns, and diagonals
    /// to determine if a player has won the game.
    /// </summary>
    /// <param name="row">Row of current cell just clicked (0-based index)</param>
    /// <param name="column">Column of current cell just clicked (0-based index)</param>
    /// <param name="player">1 for X and 2 for O</param>
    /// <returns>True if someone wins, else no winner</returns>
    public bool CheckForWinnerPlayer(int row, int column, int player)
    {
        Debug.Log("Checking for winner...");

        int[][] directions = new int[][]
        {
            new int[] {0, 1}, // Horizontal
            new int[] {1, 0}, // Vertical
            new int[] {1, 1}, // Main Diagonal \
            new int[] {1, -1} // Anti Diagonal /
        };

        // Check each direction
        foreach (var direction in directions)
        {
            int count = 1; // Count current cell

            // Count consecutive cells in positive direction
            count += CountDirection(row, column, direction[0], direction[1], player);
            // Count consecutive cells in negative direction
            count += CountDirection(row, column, -direction[0], -direction[1], player);

            // Check win condition
            if (count >= pointToWin) 
            {
                Debug.Log($"Winner found! Player {player} has {count} in a row");
                return true;
            }
        }

        // If no winner found, game continues
        return false;
    }

    /// <summary>
    /// Count the consecutive cells in specific direction from the position (row, column)
    /// </summary>
    /// <param name="row">Start Row</param>
    /// <param name="column">Start Column</param>
    /// <param name="directionRow">Row step (e.g., 1, -1, 0)</param>
    /// <param name="directionColumn">Column step (e.g., 1, -1, 0)</param>
    /// <param name="player">1 for X and 2 for O</param>
    /// <returns>Number of consecutive cells found</returns>
    private int CountDirection(int row, int column, int directionRow, int directionColumn, int player)
    {
        int count = 0;
        for (int i = 1; i < pointToWin; i++)
        {
            int newRow = row + directionRow * i;
            int newColumn = column + directionColumn * i;

            // Out of bounds check
            if (newRow < 0 || newRow >= BoardManager.Instance.boardSize || 
                newColumn < 0 || newColumn >= BoardManager.Instance.boardSize)
                break;

            // Check if the cell belongs to the current player
            if (BoardManager.Instance.boardCells[newRow, newColumn] == player)
                count++;
            else
                break; // Stop counting if we hit a different player or empty cell
        }

        return count;
    }
    #endregion
}