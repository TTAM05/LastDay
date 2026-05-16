using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [Header("Cấu hình Mixer")]
    public AudioMixer mainMixer; // Kéo file AudioMixer vào đây

    [Header("Cấu hình UI Sliders")]
    public Slider musicSlider;
    public Slider soundSlider;

    void Start()
    {
        // 1. Khởi tạo giá trị ban đầu cho Sliders từ Mixer
        float musicValue, sfxValue;
        
        // Lấy giá trị dB hiện tại từ Mixer
        mainMixer.GetFloat("MusicVol", out musicValue);
        mainMixer.GetFloat("SFXVol", out sfxValue);
        
        // Chuyển đổi dB (-80 đến 20) sang giá trị Slider (0 đến 1)
        if (musicSlider != null)
            musicSlider.value = Mathf.Pow(10, musicValue / 20);
        
        if (soundSlider != null)
            soundSlider.value = Mathf.Pow(10, sfxValue / 20);

        // 2. Đăng ký sự kiện lắng nghe thay đổi của thanh trượt
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            
        if (soundSlider != null)
            soundSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float value)
    {
        // Chuyển giá trị Slider sang Decibel (dB)
        // Dùng 0.0001f để tránh lỗi log10(0) khi kéo về hết bên trái
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat("MusicVol", dB);
    }

    public void SetSFXVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat("SFXVol", dB);
        
        // Phát thử âm thanh ngắn để người chơi nghe mức độ to/nhỏ
        AudioSource testSound = GetComponent<AudioSource>();
        if (testSound != null && !testSound.isPlaying) 
        {
            testSound.Play();
        }
    }

    // Hàm cho nút Close (X)
    public void BackToStartScene()
    {
        // Đảm bảo tên Scene "Start" chính xác 100% với file Scene của bạn
        SceneManager.LoadScene("Start");
    }
}