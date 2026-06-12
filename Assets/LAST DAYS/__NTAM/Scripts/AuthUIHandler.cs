using UnityEngine;
using TMPro;
using UnityEngine.UI; // Cần thư viện này để dùng Toggle (Ô check)
using System.Data.SqlClient;

public class AuthUIHandler : MonoBehaviour
{
    [Header("--- PANEL LOGIN ---")]
    public TMP_InputField loginUsername;
    public TMP_InputField loginPassword;
    public Toggle rememberMeToggle; // Ô tích "Ghi nhớ đăng nhập"

    [Header("--- PANEL REGISTER ---")]
    public TMP_InputField regUsername;
    public TMP_InputField regPassword;
    public TMP_InputField regConfirmPassword;

    [Header("--- UI CHUNG ---")]
    public TMP_Text feedbackText; 
    public GameObject loginPanel;
    public GameObject registerPanel;

    // Thay chuỗi kết nối của bạn vào đây
    private string connectionString = "Server=TEN_SERVER;Database=TEN_DATABASE;User Id=TEN_USER;Password=MAT_KHAU;";

    void Start()
    {
        ShowLogin();
        LoadSavedLogin(); // Gọi hàm kiểm tra xem trước đó có lưu mật khẩu không
    }

    // ==========================================
    // 1. TÍNH NĂNG GHI NHỚ ĐĂNG NHẬP (REMEMBER ME)
    // ==========================================
    private void LoadSavedLogin()
    {
        // Kiểm tra xem máy tính có lưu thông tin cũ không
        if (PlayerPrefs.HasKey("SavedUser"))
        {
            loginUsername.text = PlayerPrefs.GetString("SavedUser");
            loginPassword.text = PlayerPrefs.GetString("SavedPass");
            rememberMeToggle.isOn = true; // Tự động tích vào ô Ghi nhớ
        }
    }

    // ==========================================
    // 2. XỬ LÝ ĐĂNG NHẬP (LOGIN)
    // ==========================================
    public void OnLoginButtonClicked()
    {
        string user = loginUsername.text.Trim();
        string pass = loginPassword.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            ShowFeedback("Vui lòng nhập đầy đủ thông tin!", Color.red);
            return;
        }

        // KẾT NỐI SQL ĐỂ KIỂM TRA TÀI KHOẢN
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Accounts WHERE Username = @user AND Password = @pass";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user", user);
                    cmd.Parameters.AddWithValue("@pass", pass);
                    
                    int result = (int)cmd.ExecuteScalar();

                    if (result > 0)
                    {
                        ShowFeedback("Đăng nhập thành công!", Color.green);
                        
                        // XỬ LÝ GHI NHỚ ĐĂNG NHẬP SAU KHI THÀNH CÔNG
                        if (rememberMeToggle.isOn)
                        {
                            PlayerPrefs.SetString("SavedUser", user);
                            PlayerPrefs.SetString("SavedPass", pass);
                            PlayerPrefs.Save();
                        }
                        else
                        {
                            // Nếu không tích, xóa thông tin cũ đi
                            PlayerPrefs.DeleteKey("SavedUser");
                            PlayerPrefs.DeleteKey("SavedPass");
                        }

                        // Code chuyển Scene sang Map1 ở đây
                        // UnityEngine.SceneManagement.SceneManager.LoadScene("Map1");
                    }
                    else
                    {
                        ShowFeedback("Sai tên đăng nhập hoặc mật khẩu!", Color.red);
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            ShowFeedback("Lỗi kết nối máy chủ!", Color.red);
            Debug.LogError(ex.Message);
        }
    }

    // ==========================================
    // 3. XỬ LÝ ĐĂNG KÝ VÀ XÁC THỰC 2 MẬT KHẨU
    // ==========================================
    public void OnRegisterButtonClicked()
    {
        string user = regUsername.text.Trim();
        string pass = regPassword.text;
        string confirmPass = regConfirmPassword.text;

        // Xác thực trùng mật khẩu
        if (pass != confirmPass)
        {
            ShowFeedback("Mật khẩu xác nhận không khớp!", Color.red);
            return; // Dừng lại ngay lập tức
        }

        if (pass.Length < 6)
        {
            ShowFeedback("Mật khẩu phải từ 6 ký tự trở lên!", Color.red);
            return;
        }

        // LƯU THÔNG TIN LÊN SQL SERVER
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // BƯỚC A: Kiểm tra trùng tên
                string checkQuery = "SELECT COUNT(*) FROM Accounts WHERE Username = @user";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@user", user);
                    if ((int)checkCmd.ExecuteScalar() > 0)
                    {
                        ShowFeedback("Tên đăng nhập đã tồn tại!", Color.red);
                        return;
                    }
                }

                // BƯỚC B: Tạo tài khoản
                string insertQuery = "INSERT INTO Accounts (Username, Password, Diamonds) VALUES (@user, @pass, 500)";
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@user", user);
                    insertCmd.Parameters.AddWithValue("@pass", pass);
                    insertCmd.ExecuteNonQuery();

                    ShowFeedback("Đăng ký thành công!", Color.green);
                    Invoke("ShowLogin", 1.5f); // Chuyển về màn hình đăng nhập
                }
            }
        }
        catch (System.Exception ex)
        {
            ShowFeedback("Lỗi kết nối máy chủ!", Color.red);
            Debug.LogError(ex.Message);
        }
    }

    // Các hàm phụ trợ
    public void ShowRegister() { loginPanel.SetActive(false); registerPanel.SetActive(true); feedbackText.text = ""; }
    public void ShowLogin() { loginPanel.SetActive(true); registerPanel.SetActive(false); feedbackText.text = ""; }
    private void ShowFeedback(string message, Color color) { if (feedbackText != null) { feedbackText.text = message; feedbackText.color = color; } }
}