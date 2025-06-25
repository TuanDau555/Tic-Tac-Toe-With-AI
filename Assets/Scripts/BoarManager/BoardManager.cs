using UnityEngine;
using UnityEngine.UI;

public class BoardManager : Singleton<BoardManager>
{
    #region Parameters
    [Header("Board Settings")]
    [Tooltip("The size of the board, e.g., 3 for a 3x3 grid.")]
    public int boardSize = 3; // We can change this to support larger boards in the future.
    [HideInInspector] public int[,] boardCells; // 2D array to represent the board spaces.

    [Space(10)]
    [Header("UI Elements")]
    [SerializeField] private Sprite playerCellSprite;
    [SerializeField] private Sprite AICellSprite;

    #endregion

    #region Main Methods
    void Start()
    {
        // Initialize the board spaces array based on the board size
        boardCells = new int[boardSize, boardSize];
    }

    #endregion

    #region Placing X and O 
    /// <summary>
    /// /// When player clicks on a button
    /// this method is called to set the player's space.
    /// If it's the player's turn, it sets the player's sprite,
    /// otherwise it sets the AI's sprite.
    /// Disables the button to prevent multiple clicks.
    /// </summary>
    /// <param name="placingButton">Clicked Button to placing in Cell</param>
    /// <param name="row">Row's position of button</param>
    /// <param name="column">Column's position of button</param>
    public void SetPlayerCell(int row, int column)
    {
        // 1 represents player space, 2 represents AI space
        // this parameter used to track easier which space is occupied by whom
        int currentPlayer = GameManager.Instance.currentTurnState == TurnState.XTurn ? 1 : 2;

        PlacingCell(row, column, currentPlayer);
    }

    private void SetAICell()
    {

    }

    /// <summary>
    /// Placing Cell on the board
    /// </summary>
    /// <param name="row">Row position of the button</param>
    /// <param name="column">Column position of the button</param>
    /// <param name="currentPlayer">The player which is playing</param>
    private void PlacingCell(int row, int column, int currentPlayer)
    {
        // Don't override available cell (can't be clicked again)
        if (boardCells[row, column] != 0)
        {
            return;
        }

        boardCells[row, column] = currentPlayer; // Update the board State
        PlacingSprite(row, column, currentPlayer);

        GameManager.Instance.ProcessTurn(row, column, currentPlayer);
    }


    /// <summary>
    /// Set the Sprite when clicked on the board
    /// </summary>
    /// <param name="row">Row position of the button</param>
    /// <param name="column">Column position of the button</param>
    /// <param name="currentPlayer">The player which is playing</param>
    private void PlacingSprite(int row, int column, int currentPlayer)
    {
        Sprite sprite = (currentPlayer == 1) ? playerCellSprite : AICellSprite;

        foreach (Cell cell in FindObjectsOfType<Cell>())
        {
            if (cell.row == row && cell.column == column)
            {
                cell.SetSprite(sprite); // Set the sprite when clicked
                break; // exit loop once correct is click
            }
        }
    }
    #endregion

    #region Reset Board

    public void ResetBoard()
    {
        foreach (var cell in FindObjectsOfType<Cell>())
        {
            cell.ResetCell();
        }
    }
    
    #endregion
}
