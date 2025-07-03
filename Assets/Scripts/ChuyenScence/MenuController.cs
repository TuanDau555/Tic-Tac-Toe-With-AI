using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Hàm này sẽ được gọi khi bấm nút 3x3
    public void Select3x3()
    {
        // 1. Lưu lựa chọn vào biến tĩnh
        GameSettings.SelectedBoardSize = GameSettings.BoardSize.Size3x3;

        // 2. Tải Scene Game
        LoadGameScene();
    }

    // Hàm này sẽ được gọi khi bấm nút 5x5
    public void Select5x5()
    {
        GameSettings.SelectedBoardSize = GameSettings.BoardSize.Size5x5;

        LoadGameScene();
    }

    // Hàm này sẽ được gọi khi bấm nút 11x11
    public void Select11x11()
    {
        GameSettings.SelectedBoardSize = GameSettings.BoardSize.Size11x11;
        LoadGameScene();
    }

    private void LoadGameScene()
    {
        // Thay "GameScene" bằng tên Scene Game của bạn
        SceneManager.LoadScene("GameScene AI");
    }
}
