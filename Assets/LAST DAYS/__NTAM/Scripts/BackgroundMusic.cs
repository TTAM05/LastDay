using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic instance;

    void Awake()
    {
        // Kiểm tra nếu đã có một bản sao của nhạc nền đang chạy chưa
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Nếu đã có rồi thì xóa cái mới tạo đi để tránh trùng nhạc
        }
    }
}