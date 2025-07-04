using UnityEngine;

public static class GameModeChose
{
    // Change both of this is inspector 
    [Tooltip("Board Size (eg. 3: 3x3, 5: 5x5,...)")]
    public static int selectedBoardSize = 3;
    [Tooltip("Game Mode (PVP or PVE)")]
    public static GameMode selectedGameMode = GameMode.PVP; 
    
    // Method to set both board size and game mode
    public static void SetGameConfiguration(int boardSize, GameMode gameMode)
    {
        selectedBoardSize = boardSize;
        selectedGameMode = gameMode;
    }

    // Validation method
    // Just to make sure i don't enter invalid number
    public static bool IsValidBoardSize(int size)
    {
        return size >= 3 && size <= 15; // Reasonable limits
    }
}