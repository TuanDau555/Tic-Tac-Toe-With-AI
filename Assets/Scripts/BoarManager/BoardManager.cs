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

    public void SetPlayerSpace(Button placingButton)
    {
        // When player cick makde sure it disable 
        // so they can't click it multiple time
        if (GameManager.Instance.currentTurnState == TurnState.XTurn)
        {
            btnImage = placingButton.image;
            placingButton.interactable = false;
            btnImage.sprite = playerSpaceSprite;
            btnImage.enabled = true;
            Debug.Log("Player space set");
            GameManager.Instance.currentTurnState = TurnState.OTurn;
        }

        else
        {
            btnImage = placingButton.image;
            placingButton.interactable = false;
            btnImage.sprite = AISpaceSprite;
            btnImage.enabled = true;
            Debug.Log("Player space set");
            GameManager.Instance.currentTurnState = TurnState.XTurn;
        }
    }

    private void SetAISpace()
    {

    }
}
