using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private Sprite playerSpaceSprite;
    [SerializeField] private Sprite AISpaceSprite;

    Image btnImage;
    void Start()
    {

    }

    /// <summary>
    /// When player clicks on a button
    /// this method is called to set the player's space.
    /// If it's the player's turn, it sets the player's sprite,
    /// otherwise it sets the AI's sprite.
    /// Disables the button to prevent multiple clicks.
    /// </summary>
    public void SetPlayerSpace(Button placingButton)
    {
        if (GameManager.Instance.currentTurnState == TurnState.XTurn)
        {
            btnImage = placingButton.image;
            placingButton.interactable = false;
            btnImage.sprite = playerSpaceSprite;
            btnImage.enabled = true;
            GameManager.Instance.currentTurnState = TurnState.OTurn;
            Debug.Log("Player space set");
        }

        else
        {
            btnImage = placingButton.image;
            placingButton.interactable = false;
            btnImage.sprite = AISpaceSprite;
            btnImage.enabled = true;
            GameManager.Instance.currentTurnState = TurnState.XTurn;
            Debug.Log("Player space set");
        }
    }

    private void SetAISpace()
    {

    }
}
