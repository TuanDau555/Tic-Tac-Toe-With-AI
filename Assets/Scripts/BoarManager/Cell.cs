using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This script is attach to each Button
/// </summary> <summary>

public class Cell : MonoBehaviour
{
    public int row;
    public int column;
    private Button button;
    [SerializeField] private Sprite defaultImage;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }

    public void OnClick()
    {
        if (BoardManager.Instance != null && GameManager.Instance != null)
        {
            BoardManager.Instance.SetPlayerCell(row, column);
        }
    }

    /// <summary>
    /// Set the Sprite of the cell
    /// </summary>
    /// <param name="sprite">Appropriate Sprite of that Cell when click</param>
    public void SetSprite(Sprite sprite)
    {
        button.image.sprite = sprite;
        button.image.enabled = true;
    }

    /// <summary>
    /// Reset image and button can click again
    /// </summary>
    public void ResetCell()
    {
        button.interactable = true;
        button.image.sprite = defaultImage;
    }

}