// File: BoardManager.cs
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [Header("Các đối tượng bàn cờ trong Scene")]
    // Kéo GameObject "Board_3x3" từ Hierarchy vào đây
    public GameObject board3x3_Object;

    // Kéo GameObject "Board_11x11" từ Hierarchy vào đây
    public GameObject board11x11_Object;

    void Awake()
    {
        // 1. LUÔN LUÔN tắt hết tất cả các bàn cờ lúc đầu
        // để đảm bảo không có cái nào vô tình bị bật sẵn.
        if (board3x3_Object != null) board3x3_Object.SetActive(false);
        if (board11x11_Object != null) board11x11_Object.SetActive(false);

        // 2. Đọc lựa chọn từ Scene Menu
        switch (GameSettings.SelectedBoardSize)
        {
            // Nếu người dùng chọn 3x3
            case GameSettings.BoardSize.Size3x3:
                Debug.Log("Lựa chọn là 3x3. Đang kích hoạt Board_3x3...");
                // Bật GameObject "Board_3x3" lên (tương đương với việc bạn tích vào ô đó)
                if (board3x3_Object != null)
                {
                    board3x3_Object.SetActive(true);
                }
                break;

            // Nếu người dùng chọn 11x11
            case GameSettings.BoardSize.Size11x11:
                Debug.Log("Lựa chọn là 11x11. Đang kích hoạt Board_11x11...");
                // Bật GameObject "Board_11x11" lên (tương đương với việc bạn tích vào ô đó)
                if (board11x11_Object != null)
                {
                    board11x11_Object.SetActive(true);
                }
                break;

            default:
                Debug.LogError("Chưa chọn kích thước bàn cờ!");
                break;
        }
    }
}