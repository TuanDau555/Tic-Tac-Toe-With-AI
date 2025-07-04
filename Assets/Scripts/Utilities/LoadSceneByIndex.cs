using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneByIndex : MonoBehaviour
{
    [SerializeField] private int boardSize;
    [SerializeField] private GameMode gameMode;
    
    public void LoadScene(int sceneIndex)
    {
        // Validate board size
        if (!GameModeChose.IsValidBoardSize(boardSize))
        {
            // if not Error
            return;
        }

        // Set game configuration because it will be call in GameManager and BoardManager
        // without this function call it will not be able to send the data
        GameModeChose.SetGameConfiguration(boardSize, gameMode);
        
        // and load scene
        SceneManager.LoadScene(sceneIndex);
        
        Debug.Log($"Loading scene {sceneIndex} with board size: {boardSize}, mode: {gameMode}");
    }
}
