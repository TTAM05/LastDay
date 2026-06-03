using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameSetting : MonoBehaviour
{
    [Header("UI")]
    public GameObject optionPanel;
    public GameObject loadingObj;

    [Header("Pause")]
    public bool pauseWhenOpen = true;

    private bool isOptionOpen;
    private float oldAudioVolume;

    void Start()
    {
        if (optionPanel != null)
            optionPanel.SetActive(false);

        oldAudioVolume = AudioListener.volume;

         // tự tìm Loading
        loadingObj = GameObject.FindGameObjectWithTag("Loading");
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (loadingObj != null && loadingObj.activeInHierarchy)
                return;

            HunterDialogueZone dialogue = FindObjectOfType<HunterDialogueZone>();

            if (dialogue != null && dialogue.IsDialogueOpen())
                return;

            Health playerHealth = FindObjectOfType<Health>();
            if (playerHealth != null && playerHealth.IsDead)
                return;

            ToggleOption();
        }
    }

    public void ToggleOption()
    {
        if (loadingObj != null && loadingObj.activeInHierarchy)
            return;

        isOptionOpen = !isOptionOpen;

        if (optionPanel != null)
            optionPanel.SetActive(isOptionOpen);

        ApplyPauseState();

        foreach (var gun in FindObjectsOfType<GunSystem>())
        {
            gun.CancelFire();
        }
    }

    public void CloseOption()
    {
        isOptionOpen = false;

        if (optionPanel != null)
            optionPanel.SetActive(false);

        ApplyPauseState();

        foreach (var gun in FindObjectsOfType<GunSystem>())
        {
            gun.CancelFire();
        }
    }

    public void BackToMenu()
    {   
        Time.timeScale = 1f;
        AudioListener.pause = false;
        //Load scene MapMenu
        SceneManager.LoadScene("MapMenu");
    }
    void ApplyPauseState()
    {
        if (pauseWhenOpen)
            Time.timeScale = isOptionOpen ? 0f : 1f;

        AudioListener.pause = isOptionOpen;

        Cursor.visible = isOptionOpen;
        Cursor.lockState = isOptionOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }
}