using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHelper : MonoBehaviour
{
    public void SelectBoardAndStart(int boardSize)
    {
        // Lưu kích thước người chơi chọn vào "bộ nhớ" PlayerPrefs
        // Chúng ta đặt tên cho bộ nhớ này là "BoardSize"
        PlayerPrefs.SetInt("BoardSize", boardSize);

        // Lưu lại để chắc chắn dữ liệu được ghi
        PlayerPrefs.Save();

        // Chuyển sang GameScene
        SceneManager.LoadScene("GameScene"); // Nhớ thay "GameScene" bằng tên scene game của bạn
    }
}
