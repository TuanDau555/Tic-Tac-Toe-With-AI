using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    // Dùng enum để lựa chọn rõ ràng và tránh lỗi gõ sai chữ
    public enum BoardSize
    {
        Size3x3,
        Size5x5,
        Size11x11
    }

    // Đây là biến tĩnh sẽ lưu lựa chọn của người chơi
    // Nó sẽ tồn tại ngay cả khi chuyển scene
    public static BoardSize SelectedBoardSize;
}
