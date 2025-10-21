using UnityEngine;

[CreateAssetMenu(fileName = "Board ConFigure", menuName = "Game/Board")]
public class BoardConfigure : ScriptableObject
{
    [System.Serializable]
    public class BoardSetting
    {
        [Tooltip("Size of the table eg. 3x3, 5x5,...")]
        public int boardSize;
        [Tooltip("Prefabs of the table size")]
        public GameObject prefab;
    }
    // Array of board settings for different sizes
    [Tooltip("List of board settings for different sizes")]
    public BoardSetting[] boardSettings;
    
    // Method to get the prefab based on the board size
    public GameObject GetBoardPrefabs(int size)
    {
        // loop through the board settings to find the matching size
        foreach (var setting in boardSettings)
        {
            if (setting.boardSize == size)
            {
                // Return the prefab if the size matches
                return setting.prefab;
            }
        }

        return null;
    }
}
