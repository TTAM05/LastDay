using UnityEngine;
using UnityEngine.SceneManagement; // Bắt buộc phải có để dùng SceneManager

public class MenuController : MonoBehaviour
{
    // Hàm chuyển sang Scene Login
    public void GoToLogin()
    {
        SceneManager.LoadScene("Login"); 
    }

    // Hàm chuyển sang Scene Setting
    public void GoToSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    // Hàm thoát Game
    public void ExitGame()
    {
        Debug.Log("Game is exiting..."); // Hiển thị trong Console để kiểm tra
        Application.Quit(); // Lệnh này chỉ có tác dụng khi đã Build thành file .exe
    }
}