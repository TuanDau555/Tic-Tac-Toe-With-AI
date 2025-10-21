using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// NOTE: Every move that need to check for win or for block...
/// ... always need to check all the EMPTY CELL 
/// </summary>
/// <remarks>
/// AIPlayer is responsible for determining the best move for the AI in a Tic-Tac-Toe-like game.
/// It uses heuristics and the minimax algorithm to evaluate possible moves and select the optimal one.
/// The AI prioritizes immediate wins, blocking the opponent, and then uses minimax for deeper strategy.
/// </remarks>
public class AIPlayer
{
    #region Variables
    // Heuristic and algorithm constants
    private const int k_winScore = 100000;
    private const int k_almostWinScore = 10101;
    private const int k_threeInRowScore = 10099;
    private const int k_TwoInRowScore = 301;
    private const int k_CenterBonus = 39;
    private const int k_positiveInfinity = 1000000;
    private const int k_negativeInfinity = -1000000;
    #endregion

    #region Finding Best Move
    // Main entry: returns the best move for the AI given the current board state
    public static Vector2Int GetBestMove(int[,] board, int boardSize)
    {
        int occupiedCells = (boardSize * boardSize) - BoardManager.Instance.GetEmptyCellCount();
        int depth = GetAdaptiveDepth(boardSize, occupiedCells);
        int pointToWin = boardSize >= 5 ? 5 : 3; // Win condition depends on board size

        // Try to win immediately
        Vector2Int win = FindImmediateMove(board, boardSize, 2, pointToWin);
        if (win.x != -1) return win;

        // Try to block opponent's win
        Vector2Int block = FindImmediateMove(board, boardSize, 1, pointToWin);
        if (block.x != -1) return block;

        // Otherwise, use minimax to find the best move
        return FindBestMoveUsingMinimax(board, boardSize, pointToWin, depth);
    }

    // Uses minimax with alpha-beta pruning to select the best move
    private static Vector2Int FindBestMoveUsingMinimax(int[,] board, int boardSize, int pointToWin, int maxDepth)
    {
        var moves = GetSmartPositions(board, boardSize);
        if (moves.Count == 0)
            return new Vector2Int(boardSize / 2, boardSize / 2); // Default to center if no moves
        int bestScore = k_negativeInfinity;
        Vector2Int bestMove = moves[0];
        foreach (var move in moves)
        {
            board[move.x, move.y] = 2;
            int score = Minimax(board, boardSize, maxDepth - 1, false, k_negativeInfinity, k_positiveInfinity, pointToWin);
            board[move.x, move.y] = 0;
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }
        return bestMove;
    }

    // Minimax algorithm with alpha-beta pruning
    private static int Minimax(int[,] board, int boardSize, int depth, bool isMaximizing,
                               int alpha, int beta, int pointToWin)
    {
        if (depth == 0)
            return EvaluateBoard(board, boardSize, pointToWin);
        var smartMoves = GetSmartPositions(board, boardSize);

        // Max for AI
        if (isMaximizing)
        {
            int maxEval = k_negativeInfinity;
            foreach (var move in smartMoves)
            {
                board[move.x, move.y] = 2;
                // Check for immediate win
                if (GameManager.Instance.CheckForWinnerPlayer(move.x, move.y, 2))
                {
                    // if win so undo move and return plus score no need to calculate anymore
                    board[move.x, move.y] = 0;
                    return k_winScore + depth;
                }
                int eval = Minimax(board, boardSize, depth - 1, false, alpha, beta, pointToWin);
                board[move.x, move.y] = 0; // undo and continue calculate
                maxEval = Mathf.Max(maxEval, eval);
                alpha = Mathf.Max(alpha, eval);
                if (beta <= alpha) break;
            }
            return maxEval;
        }
        // Min for player
        else
        {
            int minEval = k_positiveInfinity;
            foreach (var move in smartMoves)
            {
                board[move.x, move.y] = 1;
                // Check if opponent can win
                if (GameManager.Instance.CheckForWinnerPlayer(move.x, move.y, 1))
                {
                    board[move.x, move.y] = 0;
                    return -k_winScore - depth; // player win so minus AI score
                }
                int eval = Minimax(board, boardSize, depth - 1, true, alpha, beta, pointToWin);
                board[move.x, move.y] = 0;
                minEval = Mathf.Min(minEval, eval);
                beta = Mathf.Min(beta, eval);
                if (beta <= alpha) break;
            }
            return minEval;
        }
    }
    #endregion

    #region Calculate Score
    // Evaluates the board by summing up the scores for both players
    private static int EvaluateBoard(int[,] board, int boardSize, int pointToWin)
    {
        int aiScore = 0, playerScore = 0;
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                if (board[row, col] == 2)
                    aiScore += EvaluatePosition(board, boardSize, row, col, 2, pointToWin);
                else if (board[row, col] == 1)
                    playerScore += EvaluatePosition(board, boardSize, row, col, 1, pointToWin);
            }
        }
        // Positive result is good for AI
        return aiScore - playerScore;
    }

    // Evaluates a single position for a player
    private static int EvaluatePosition(int[,] board, int boardSize, int row, int col, int player, int pointToWin)
    {
        int score = 0;
        int[] dRow = { 0, 1, 1, 1 }, dCol = { 1, 0, 1, -1 };
        for (int dir = 0; dir < 4; dir++)
        {
            score += EvaluateDirection(board, boardSize, row, col, dRow[dir], dCol[dir], player, pointToWin);
        }
        score += GetPositionBonus(row, col, boardSize);
        return score;
    }
    #endregion

    #region Smart Positioning
    // Checks for immediate win or block opportunities
    private static Vector2Int FindImmediateMove(int[,] board, int boardSize, int player, int pointToWin)
    {
        var moves = GetSmartPositions(board, boardSize);
        foreach (var m in moves)
        {
            board[m.x, m.y] = player;
            if (GameManager.Instance.CheckForWinnerPlayer(m.x, m.y, player))
            {
                board[m.x, m.y] = 0;
                return m;
            }
            board[m.x, m.y] = 0;
        }
        return new Vector2Int(-1, -1);
    }

    // Returns a list of smart candidate positions for the AI to consider for its next move.
    // It looks for empty cells within a 5x5 area (centered on each occupied cell) to focus the search on relevant spots.
    // If the board is empty (no moves yet), it returns the center position as the starting move.
    private static List<Vector2Int> GetSmartPositions(int[,] board, int boardSize)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        bool hasMoved = false;
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                // If the cell is occupied, look for nearby empty cells
                if (board[row, col] != 0)
                {
                    hasMoved = true;
                    for (int dRow = -2; dRow <= 2; dRow++)
                    {
                        for (int dCol = -2; dCol <= 2; dCol++)
                        {
                            // Check bounds and if the cell is empty
                            if (row + dRow >= 0
                                && row + dRow < boardSize
                                && col + dCol >= 0
                                && col + dCol < boardSize
                                && board[row + dRow, col + dCol] == 0)
                            {
                                // Avoid duplicates
                                if (!positions.Contains(new Vector2Int(row + dRow, col + dCol)))
                                    positions.Add(new Vector2Int(row + dRow, col + dCol));
                            }
                        }
                    }
                }
            }
        }
        // If no moves have been made, start at the center
        if (!hasMoved) positions.Add(new Vector2Int(boardSize / 2, boardSize / 2));
        return positions;
    }
    #endregion

    #region Heuristic Calculation
    // Heuristic evaluation for a direction from a given cell
    private static int EvaluateDirection(int[,] board, int boardSize, int row, int col,
                                       int dirRow, int dirCol, int player, int pointToWin)
    {
        int count = 1;
        bool leftOpen = false;
        bool rightOpen = false;
        // Count consecutive pieces to the right
        for (int i = 1; i < pointToWin; i++)
        {
            int newRow = row + dirRow * i;
            int newCol = col + dirCol * i;
            if (newRow < 0 || newRow >= boardSize || newCol < 0 || newCol >= boardSize)
                break;
            if (board[newRow, newCol] == player)
                count++;
            else if (board[newRow, newCol] == 0)
            {
                rightOpen = true;
                break;
            }
            else
                break;
        }
        // Count consecutive pieces to the left
        for (int i = 1; i < pointToWin; i++)
        {
            int newRow = row - dirRow * i;
            int newCol = col - dirCol * i;
            if (newRow < 0 || newRow >= boardSize || newCol < 0 || newCol >= boardSize)
                break;
            if (board[newRow, newCol] == player)
                count++;
            else if (board[newRow, newCol] == 0)
            {
                leftOpen = true;
                break;
            }
            else
                break;
        }
        return CalculateScore(count, leftOpen, rightOpen, pointToWin);
    }

    // Calculates the heuristic score for a line of pieces
    private static int CalculateScore(int count, bool leftOpen, bool rightOpen, int pointToWin)
    {
        bool openBoth = leftOpen && rightOpen;
        bool openOneSide = leftOpen || rightOpen;

        if (count >= pointToWin)
            return k_winScore;
        // Four in a row (almost win)*         
        if (count == pointToWin - 1)
        {
            if (openBoth)
                return k_almostWinScore * 2 + 1;
            if (openOneSide)
                return k_almostWinScore;
            return k_threeInRowScore;
        }
        // Three in a row*         
        if (count == pointToWin - 2)
        {
            if (openBoth)
                return k_threeInRowScore * 3 + 1;
            if (openOneSide)
                return k_TwoInRowScore * 2 + 1;
        }
        // Two in a row*         
        if (count >= 2 && (leftOpen || rightOpen))
            return k_TwoInRowScore;
        return count;
    }
        // Returns a bonus for being closer to the center of the board
    private static int GetPositionBonus(int row, int col, int boardSize)
    {
        int center = boardSize / 2;
        int distance = Mathf.Abs(row - center) + Mathf.Abs(col - center);
        return Mathf.Max(0, k_CenterBonus - distance);
    }
    #endregion

    #region Dynamic Depth

    /// <summary>
    /// Dynamically determines the minimax search depth based on board size and number of occupied cells.
    /// This helps balance AI strength and performance by limiting the number of nodes evaluated.
    /// </summary>
    /// <param name="boardSize">The size of the game board (e.g., 3 for 3x3, 5 for 5x5).</param>
    /// <param name="occupiedCells">The number of cells currently occupied on the board.</param>
    /// <returns>The adaptive search depth for the minimax algorithm (minimum 2).</returns>
    private static int GetAdaptiveDepth(int boardSize, int occupiedCells)
    {
        // Calculate the number of empty cells
        int emptyCells = (boardSize * boardSize) - occupiedCells;
        // Limit the number of candidate moves to a maximum of 25 for performance
        int estimatedCandidates = Mathf.Min(18, emptyCells);
        // Set a maximum number of nodes to avoid lag
        int maxNodes = 100000;
        int depth = 1;
        int nodes = estimatedCandidates;
        // Increase depth as long as the estimated number of nodes is within the limit and depth < 6
        // Or you can easily understand that this loop just for limit the depth and node check 
        while (nodes * estimatedCandidates < maxNodes && depth < 6)
        {
            depth++;
            nodes *= estimatedCandidates;
        }
        // Ensure a minimum depth of 2
        return Mathf.Max(2, depth - 1);
    }
    #endregion
}