using UnityEngine;
using UnityEngine.UI;


public class BoardManager : Singleton<BoardManager>
{
    #region Parameters
    [Header("Board Settings")]
    [Tooltip("The size of the board, e.g., 3 for a 3x3 grid.")]
    public int boardSize = 3;
    [HideInInspector] public int[,] boardCells;
    [HideInInspector] public int playerLastRow = -1;
    [HideInInspector] public int playerLastCol = -1;

    [Space(10)]
    [Header("Board Configuration")]
    [SerializeField] private BoardConfigure boardConfigure;

    [Space(10)]
    [Header("UI Elements")]
    [SerializeField] private Sprite playerCellSprite;
    [SerializeField] private Sprite AICellSprite;

    // Cache for better performance
    private Cell[] allCells;
    #endregion

    #region Main Methods
    void Start()
    {
        GeneratedTable();
     
        // Cache all cells for better performance
        allCells = FindObjectsOfType<Cell>();

    }
    #endregion

    #region Generated table
    private void GeneratedTable()
    {
        boardSize = GameModeChose.selectedBoardSize; // Data send from Main Menu
        // Initialize the board spaces array based on the board size
        boardCells = new int[boardSize, boardSize];

        // Get the prefab for the board size
        GameObject prefab = boardConfigure.GetBoardPrefabs(boardSize);
        if (prefab != null)
        {
            // after get prefab, instantiate it
            // Set the parent to this transform to keep hierarchy organized
            Instantiate(prefab, transform);
        }
        else
        {
            Debug.LogError("Not found Table");
        }
        Debug.Log($"Board initialized with size {boardSize}x{boardSize}");
    }
    #endregion

    #region Placing X and O 
    /// <summary>
    /// When player clicks on a button, this method is called to set the player's move.
    /// Validates the move and processes it if valid.
    /// </summary>
    /// <param name="row">Row position of clicked cell</param>
    /// <param name="column">Column position of clicked cell</param>
    public void SetPlayerCell(int row, int column)
    {
        // Validate move
        if (!IsValidMove(row, column))
        {
            Debug.LogWarning($"Invalid player move at ({row},{column})");
            return;
        }

        // Only allow player moves during player's turn
        if (GameManager.Instance.currentTurnState != TurnState.XTurn && GameManager.Instance.currentMode == GameMode.PVE)
        {
            Debug.LogWarning("Not player's turn");
            return;
        }

        // 1 represents player, 2 represents AI
        int currentPlayer = (GameManager.Instance.currentTurnState == TurnState.XTurn) ? 1 : 2;

        // Update player's last move (for PVE Mode)
        if (currentPlayer == 1)
        {
            playerLastRow = row;
            playerLastCol = column;
        }
        PlacingCell(row, column, currentPlayer);
    }

    /// <summary>
    /// AI makes a move at specified position
    /// </summary>
    /// <param name="row">Row position</param>
    /// <param name="column">Column position</param>
    public void SetAICell(int row, int column)
    {
        if (!IsValidMove(row, column))
        {
            Debug.LogError($"Invalid AI move attempted at ({row},{column})");
            return;
        }

        int currentPlayer = 2; // AI is always player 2
        PlacingCell(row, column, currentPlayer);
    }

    /// <summary>
    /// Places a piece on the board and updates game state
    /// </summary>
    /// <param name="row">Row position</param>
    /// <param name="column">Column position</param>
    /// <param name="currentPlayer">The player making the move (1 for X, 2 for O)</param>
    private void PlacingCell(int row, int column, int currentPlayer)
    {
        // Update board state
        boardCells[row, column] = currentPlayer;

        // Update visual representation
        PlacingSprite(row, column, currentPlayer);

        // Store player's last move for AI reference
        if (currentPlayer == 1)
        {
            playerLastRow = row;
            playerLastCol = column;
        }

        Debug.Log($"Player {currentPlayer} placed at ({row},{column})");

        // Process the turn in GameManager
        GameManager.Instance.ProcessTurn(row, column, currentPlayer);
    }

    /// <summary>
    /// Updates the visual sprite for a cell
    /// </summary>
    /// <param name="row">Row position</param>
    /// <param name="column">Column position</param>
    /// <param name="currentPlayer">The player (1 for X, 2 for O)</param>
    private void PlacingSprite(int row, int column, int currentPlayer)
    {
        Sprite sprite = (currentPlayer == 1) ? playerCellSprite : AICellSprite;

        // Use cached cells for better performance
        foreach (Cell cell in allCells)
        {
            if (cell.row == row && cell.column == column)
            {
                cell.SetSprite(sprite);
                Debug.Log($"Sprite set for cell ({row},{column})");
                return; // Exit once we find the correct cell
            }
        }

        Debug.LogError($"Could not find cell at position ({row},{column})");
    }
    #endregion

    #region Board State

    /// <summary>
    /// Gets the number of empty cells on the board
    /// </summary>
    /// <returns>Number of empty cells</returns>
    public int GetEmptyCellCount()
    {
        int count = 0;
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                if (boardCells[row, col] == 0)
                    count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Validates if a move is legal
    /// </summary>
    /// <param name="row">Row position</param>
    /// <param name="column">Column position</param>
    /// <returns>True if move is valid</returns>
    private bool IsValidMove(int row, int column)
    {
        // Check bounds
        if (row < 0 || row >= boardSize || column < 0 || column >= boardSize)
            return false;

        // Check if cell is empty
        if (boardCells[row, column] != 0)
            return false;

        // Check if game is not over
        if (GameManager.Instance.currentTurnState == TurnState.GameOver)
            return false;

        return true;
    }

    #endregion

    #region Reset Board

    /// <summary>
    /// Resets the entire board to initial state
    /// </summary>
    public void ResetBoard()
    {
        // Reset logical board
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                boardCells[row, col] = 0;
            }
        }

        // Reset player tracking
        playerLastRow = -1;
        playerLastCol = -1;

        // Reset visual board
        if (allCells == null)
            allCells = FindObjectsOfType<Cell>();

        foreach (var cell in allCells)
        {
            cell.ResetCell();
        }

        Debug.Log("Board reset completed");
    }

    #endregion
}