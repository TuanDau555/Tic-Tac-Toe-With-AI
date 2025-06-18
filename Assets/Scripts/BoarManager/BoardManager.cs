using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [Header("Game Sprites")]
    [SerializeField] private Sprite playerSpaceSprite; // Sprite cho người chơi X
    [SerializeField] private Sprite AISpaceSprite;     // Sprite cho người chơi O

    [Header("Board Settings")]
    [SerializeField] private int boardSize = 3; // Kích thước bàn cờ (ví dụ: 3 cho 3x3, 5 cho 5x5)
    [SerializeField] private GameObject buttonParent; // Kéo Panel/GameObject chứa các Button vào đây

    private Button[,] boardButtons; // Mảng 2 chiều để lưu trữ các button trên bàn cờ

    void Awake()
    {
        // Khởi tạo và sắp xếp các button vào mảng 2 chiều
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        boardButtons = new Button[boardSize, boardSize];
        Button[] allButtons = buttonParent.GetComponentsInChildren<Button>();

        if (allButtons.Length != boardSize * boardSize)
        {
            Debug.LogError("Số lượng Button không khớp với boardSize! Vui lòng kiểm tra lại.");
            return;
        }

        int index = 0;
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                boardButtons[i, j] = allButtons[index];
                index++;
            }
        }
    }

    /// <summary>
    /// Được gọi khi người chơi nhấn vào một ô (Button).
    /// </summary>
    public void SetPlayerSpace(Button placingButton)
    {
        // --- ĐIỀU KIỆN DỪNG GAME ---
        // Nếu game đã kết thúc, không cho phép đánh thêm
        if (GameManager.Instance.isGameOver)
        {
            return;
        }

        Image btnImage = placingButton.image;
        placingButton.interactable = false;

        // Xác định sprite dựa trên lượt đi hiện tại
        Sprite currentPlayerSprite;
        if (GameManager.Instance.currentTurnState == TurnState.XTurn)
        {
            currentPlayerSprite = playerSpaceSprite;
            btnImage.sprite = currentPlayerSprite;
            GameManager.Instance.currentTurnState = TurnState.OTurn; // Chuyển lượt
        }
        else
        {
            currentPlayerSprite = AISpaceSprite;
            btnImage.sprite = currentPlayerSprite;
            GameManager.Instance.currentTurnState = TurnState.XTurn; // Chuyển lượt
        }

        btnImage.enabled = true;
        Debug.Log("Một ô đã được đánh.");

        // --- KIỂM TRA THẮNG THUA SAU MỖI NƯỚC ĐI ---
        CheckForWinner(placingButton, currentPlayerSprite);
    }

    private void CheckForWinner(Button lastMoveButton, Sprite playerSprite)
    {
        // Tìm vị trí (hàng, cột) của nước đi vừa rồi
        int row = -1, col = -1;
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                if (boardButtons[i, j] == lastMoveButton)
                {
                    row = i;
                    col = j;
                    break;
                }
            }
            if (row != -1) break;
        }

        // Nếu tìm thấy vị trí, bắt đầu kiểm tra
        if (row != -1 && col != -1)
        {
            if (CheckHorizontal(row, playerSprite) ||
                CheckVertical(col, playerSprite) ||
                CheckDiagonal(playerSprite) ||
                CheckAntiDiagonal(playerSprite))
            {
                // Nếu có người thắng, kết thúc game
                EndGame(playerSprite);
            }
        }
    }

    private bool CheckHorizontal(int row, Sprite playerSprite)
    {
        for (int j = 0; j < boardSize; j++)
        {
            // Nếu có một ô không phải của người chơi hiện tại, hàng này không thắng
            if (boardButtons[row, j].image.sprite != playerSprite)
            {
                return false;
            }
        }
        // Nếu tất cả các ô trong hàng đều là của người chơi, trả về true
        return true;
    }

    private bool CheckVertical(int col, Sprite playerSprite)
    {
        for (int i = 0; i < boardSize; i++)
        {
            if (boardButtons[i, col].image.sprite != playerSprite)
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckDiagonal(Sprite playerSprite)
    {
        // Kiểm tra đường chéo chính (từ trái trên xuống phải dưới)
        for (int i = 0; i < boardSize; i++)
        {
            if (boardButtons[i, i].image.sprite != playerSprite)
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckAntiDiagonal(Sprite playerSprite)
    {
        // Kiểm tra đường chéo phụ (từ phải trên xuống trái dưới)
        for (int i = 0; i < boardSize; i++)
        {
            if (boardButtons[i, boardSize - 1 - i].image.sprite != playerSprite)
            {
                return false;
            }
        }
        return true;
    }

    private void EndGame(Sprite winnerSprite)
    {
        GameManager.Instance.isGameOver = true;

        // Thông báo người thắng
        string winner = (winnerSprite == playerSpaceSprite) ? "Player X" : "Player O";
        Debug.Log($"Trò chơi kết thúc! Người thắng là: {winner}");

        // Vô hiệu hóa tất cả các button còn lại trên bàn cờ
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                boardButtons[i, j].interactable = false;
            }
        }
    }

    // Bạn có thể giữ hoặc xóa hàm này tùy ý
    private void SetAISpace()
    {
        // Logic cho AI đi sẽ nằm ở đây
    }
}