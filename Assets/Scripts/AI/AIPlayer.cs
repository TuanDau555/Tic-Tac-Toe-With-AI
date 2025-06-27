using System.Collections.Generic;
using UnityEngine;

public class AIPlayer
{
    #region AI Mode Win Check   
    /// <summary>
    /// Check if the AI player has won from a specific cell.
    /// </summary>
    /// <remarks>
    /// NOTE: I think this method is the same as the player's win check
    /// So we can optimize it later...
    /// ...by creating a common method for both AI and player win checks.
    /// </remarks>
    /// <param name="boardCells">Is current board</param>
    /// <param name="row">Row's position of the Button</param>
    /// <param name="column">Column's position of the Button</param>
    /// <param name="size">Size of the board (3x3, 5x5)</param>
    /// <param name="currentPlayer">Player to check (Ai or User) </param>
    /// <param name="target">Consecutive Line to win (Ex: 3x3 need 3 Line)</param>
    /// <returns>Win</returns> 
    /// <summary>
    public static bool CheckWinFromCellAI(int[,] boardCells, int row, int column, int size, int currentPlayer, int target)
    {
        Vector2Int[] direction = new Vector2Int[]
        {
            // Directions for checking win conditions
            // The same directions as in the player's win check
            new Vector2Int(0, 1), new Vector2Int(1, 0),
            new Vector2Int(1, 1), new Vector2Int(1, -1)
        };

        // Check each direction
        // NOTE: We will optimize the code if we have time later
        foreach (var dir in direction)
        {
            int count = 1; // Start with the current cell

            // Count in the positive direction
            for (int step = 1; step < target; step++)
            {
                // Calculate the new row index, and store it in newRow
                int newRow = row + dir.x * step;
                // Calculate the new column index, and store it in newColumn
                int newColumn = column + dir.y * step;

                // if not out of bound,  
                if (newRow >= 0 && newRow < size && newColumn >= 0 && newColumn < size && boardCells[newRow, newColumn] == currentPlayer)
                    count++;
                else
                    break; // Out of bounds check

            }

            // Count in the negative direction
            for (int step = 1; step < target; step++)
            {
                // Calculate the new row index, and store it in newRow
                int newRow = row - dir.x * step;
                // Calculate the new column index, and store it in newColumn
                int newColumn = column - dir.y * step;

                // if not out of bound,  
                if (newRow >= 0 && newRow < size && newColumn >= 0 && newColumn < size && boardCells[newRow, newColumn] == currentPlayer)
                    count++;
                else
                    break; // Out of bounds check
            }
            // If count is equal to target, AI wins
            if (count >= target) return true;
        }


        // else no win found
        return false;
    }
    #endregion

    #region AI Evaluation
    /// <summary>
    /// AI thinks if the board is in a good state or not.
    /// </summary>
    /// <param name="boardCells">Is current board</param>
    /// <param name="size">Size of board (3x3, 5x5)</param>
    /// <returns>Score for AI (0: Mid move; plus: AI good move; minus: Player good move)</returns>
    public static int EvaluateBoard(int[,] boardCells, int size, int lastRow, int lastColumn)
    {
        int currentPlayer = boardCells[lastRow, lastColumn];
        if (currentPlayer == 0) return 0;
        
        // Size of board more than 5x5 alway need 5 consecutive line to win
        if (CheckWinFromCellAI(boardCells, lastRow, lastColumn, size, currentPlayer, size >= 5 ? 5 : 3))
        {
            // if Ai win point will plus, else minus
            // If current player is AI (2), return a high positive score
            return currentPlayer == 2 ? +10 : -10;
        }

        return 0;
    }
    #endregion

    /// <summary>
    /// Minimax algorithm with Alpha-Beta Pruning used by AI to determine the best move.
    /// It simulates all possible future moves, trying to maximize the AI's score
    /// and minimize the player's score.
    /// </summary>
    /// <param name="boardCells">Is current board</param>
    /// <param name="size">Size of the table (3x3, 5x5,...)</param>
    /// <param name="depth">Depth of the tree</param>
    /// <param name="isMaximizing">Which turn to check (AI or Player)</param>
    /// <param name="alpha">Best score maximizing Ai can have</param>
    /// <param name="beta">Best score minimizing player can have</param>
    /// <returns>The best score found positive for AI, negative for player, 0 is draw</returns>
    private static int Minimax(int[,] boardCells, int size, int depth, bool isMaximizing, int alpha, int beta, int lastRow, int lastColumn)
    {
        int score = EvaluateBoard(boardCells, size, lastRow, lastColumn);
        // if there already a winner or depth is 0, return the score
        if (score != 0 || depth == 0)
            return score * (depth + 1); // Prioritize the move that win faster (or lose slower)

        // List for empty cell
        // Loop all cells 1 time
        List<(int row, int column)> emptyCells = GetEmptyCells(boardCells, size);

        // If it's AI's turn (maximizing player)
        if (isMaximizing)
        {
            // Initialize best score to the lowest possible value that play can get
            int best = int.MinValue;
            foreach (var cell in emptyCells)
            {
                // Simulate the AI's move
                // AI will test all possible moved (prediction player's move)...
                boardCells[cell.row, cell.column] = 2;
                //...continue to simulate on the next depth
                // Evaluate the move what would happen if player countered 
                int eval = Minimax(boardCells, size, depth - 1, false, alpha, beta, cell.row, cell.column);
                // Undo the move
                // AI have test the cell, so we reset it
                boardCells[cell.row, cell.column] = 0;
                // Get the best score
                // If the AI's score is better than the best score, update it
                best = Mathf.Max(best, eval);
                // Update alpha (the best score for the maximizing player)
                alpha = Mathf.Max(alpha, eval);
                // prune the search tree
                if (beta <= alpha) break;
            }
           if (best == int.MinValue || best == int.MaxValue)
                return 0;

            return best;
        }
        // If it's player's turn (minimizing player)
        else
        {
            // Initialize best score to the highest possible value that AI can get
            int best = int.MaxValue;

            foreach (var cell in emptyCells)
            {
                // Simulate the Player's move
                // AI also test it own possible move
                boardCells[cell.row, cell.column] = 1;
                // Continue to simulate on the next depth..
                // Evaluate the move what would happen if it countered
                int eval = Minimax(boardCells, size, depth - 1, true, alpha, beta, cell.row, cell.column);
                boardCells[cell.row, cell.column] = 0; // Undo(Reset) the cell after test
                // Get the best score
                // If the player's move is less than the best score, update it 
                best = Mathf.Min(best, eval);
                // Update beta (the score for minimizing player)
                beta = Mathf.Min(beta, eval);
                // prune the search tree
                if (beta <= alpha) break;
            }

            if (best == int.MinValue || best == int.MaxValue)
                return 0;

            return best;
        }
    }

    public static Vector2Int GetBestMove(int[,] boardCells, int size, int depthLimit)
    {
        int bestScore = int.MinValue;
        Vector2Int bestMove = new Vector2Int(-1, -1);

        List<(int row, int column)> emptyCells = GetEmptyCells(boardCells, size);

        // Return (-1, -1) if no moves are available (board full)
        if (emptyCells.Count == 0)
        {
            Debug.Log("No moves available for AI (board full).");
            return bestMove;
        }

        foreach (var cell in emptyCells)
        {
            boardCells[cell.row, cell.column] = 2;
            int score = Minimax(boardCells, size, depthLimit - 1, false, int.MinValue, int.MaxValue, cell.row, cell.column);
            boardCells[cell.row, cell.column] = 0;

            Debug.Log($"[AI] Evaluate move at ({cell.row},{cell.column}) → score = {score}");
            
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = new Vector2Int(cell.row, cell.column);
            }
        }
        return bestMove;
    }
    
    public static List<(int row, int col)> GetEmptyCells(int[,] board, int size)
    {
        List<(int row, int col)> emptyCells = new List<(int, int)>();

        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                if (board[row, col] == 0)
                    emptyCells.Add((row, col));
            }
        }

        return emptyCells;
    }

    
}

