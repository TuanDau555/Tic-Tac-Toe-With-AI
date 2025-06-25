using TMPro;
using UnityEngine;

public class GameOverUI : Singleton<GameOverUI>
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private GameObject gameNotify;

    void Start()
    {
        Hide();
    }
    
    /// <summary>
    /// Show when Game Over(Win, lose, Draw)
    /// </summary>
    /// <param name="gameOverText">Show text who win</param>
    /// <param name="colorText">Show color of text</param> 
    /// <summary>
    public void ShowGameOver(string gameOverText, Color colorText)
    {
        resultText.text = gameOverText;
        resultText.color = colorText;
        Show();
    }

    private void Show()
    {
        gameNotify.SetActive(true);
    }

    public void Hide()
    {
        gameNotify.SetActive(false);    
    }
}
