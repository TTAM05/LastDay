using UnityEngine;
using UnityEngine.SceneManagement; // Bắt buộc phải có để dùng SceneManager

public class MenuController : MonoBehaviour
{
    public string mapName;
    public GameObject SettingPanel;
    public GameObject MenuPanel;

     void Start()
    {
        // Đảm bảo SettingPanel được ẩn khi bắt đầu
        if (SettingPanel != null)
        {
            SettingPanel.SetActive(false);
        }
    }
    // Hàm chuyển sang Scene Login
    public void GoMenu()
    {
        SceneManager.LoadScene("MapMenu"); 
    }

    //Play
    public void PlayGame()
    {
        SceneManager.LoadScene(mapName);
    }

    // Hàm chuyển sang Scene Setting
    public void GoToSettings()
    {
        if (SettingPanel != null)
        {
            SettingPanel.SetActive(true);
            MenuPanel.SetActive(false);
        }
    }

    // Hàm quay lại Menu chính từ Setting
    public void BackToMenu()
    {
        if (SettingPanel != null)
        {
            SettingPanel.SetActive(false);
            MenuPanel.SetActive(true);
        }
    }

    // Hàm chuyển sang Start
    public void GoToStart()
    {
        SceneManager.LoadScene("Start");
    }

    // Hàm thoát Game
    public void ExitGame()
    {
        Debug.Log("Game is exiting..."); // Hiển thị trong Console để kiểm tra
        Application.Quit(); // Lệnh này chỉ có tác dụng khi đã Build thành file .exe
    }
}