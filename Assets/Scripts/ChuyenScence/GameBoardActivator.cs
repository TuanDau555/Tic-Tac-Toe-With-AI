using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBoardActivator : MonoBehaviour
{
    public GameObject board3x3;
    public GameObject board5x5;
    public GameObject board11x11;

    void Start()
    {
        // Đọc giá trị từ "bộ nhớ" có tên "BoardSize"
        // Nếu không tìm thấy, nó sẽ lấy giá trị mặc định là 3
        int boardSize = PlayerPrefs.GetInt("BoardSize", 3);

        // Tắt hết các bàn cờ đi trước cho chắc
        board3x3.SetActive(false);
        board5x5.SetActive(false);
        board11x11.SetActive(false);

        // Dựa vào giá trị đọc được, bật bàn cờ tương ứng
        if (boardSize == 3)
        {
            board3x3.SetActive(true);
        }
        else if (boardSize == 5)
        {
            board5x5.SetActive(true);
        }
        else if (boardSize == 11)
        {
            board11x11.SetActive(true);
        }
    }

    // Hàm cho nút "back"
    public void GoBackToMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Thay bằng tên scene menu của bạn
    }
}
