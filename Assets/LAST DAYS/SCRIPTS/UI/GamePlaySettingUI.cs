using UnityEngine;
using UnityEngine.UI;

public class GameplaySettingUI : MonoBehaviour
{
    public Button minimapOnButton;
    public Button minimapOffButton;
    public Button healthBarOnButton;
    public Button healthBarOffButton;

    public Button gameplayTabButton;
    public Button uiTabButton;
    public Button screenModeTabButton;

    public Color activeColor = new Color32(0xC9, 0xFF, 0x2D, 0xFF);
    public Color inactiveColor = Color.white;

    GameObject minimapObj;
    GameObject healthBarObj;

    public GameObject GameplayContent;
    public GameObject UIContent;
    public GameObject ScreenModeContent;

    void Start()
    {
        minimapObj = FindObjectByTagIncludeInactive("Minimap");
        healthBarObj = FindObjectByTagIncludeInactive("HealthBar");

        bool minimap = PlayerPrefs.GetInt("Minimap", 1) == 1;
        bool healthbar = PlayerPrefs.GetInt("HealthBar", 1) == 1;

        SetMinimap(minimap, true);
        SetHealthBar(healthbar, true);
        UpdateTabButtons();
    }

    public void OnMinimapOn()
    {
        SetMinimap(true);
    }

    public void OnMinimapOff()
    {
        SetMinimap(false);
    }

    public void OnHealthBarOn()
    {
        SetHealthBar(true);
    }

    public void OnHealthBarOff()
    {
        SetHealthBar(false);
    }

    public void ShowGameplayContent()
    {
        if (GameplayContent != null)
            GameplayContent.SetActive(true);

        if (UIContent != null)
            UIContent.SetActive(false);

        if (ScreenModeContent != null)
            ScreenModeContent.SetActive(false);

        SetButtonColor(gameplayTabButton, activeColor);
        SetButtonColor(uiTabButton, inactiveColor);
        SetButtonColor(screenModeTabButton, inactiveColor);
    }

    public void ShowUIContent()
    {
        if (GameplayContent != null)
            GameplayContent.SetActive(false);

        if (UIContent != null)
            UIContent.SetActive(true);

        if (ScreenModeContent != null)
            ScreenModeContent.SetActive(false);

        SetButtonColor(gameplayTabButton, inactiveColor);
        SetButtonColor(uiTabButton, activeColor);
        SetButtonColor(screenModeTabButton, inactiveColor);
    }

    void SetTabButtonColors(bool gameplayActive)
    {
        SetButtonColor(gameplayTabButton, gameplayActive ? activeColor : inactiveColor);
        SetButtonColor(uiTabButton, gameplayActive ? inactiveColor : activeColor);
    }

    void UpdateTabButtons()
    {
        bool gameplayActive = GameplayContent != null && GameplayContent.activeSelf;
        SetTabButtonColors(gameplayActive);
    }

    void SetMinimap(bool isOn, bool initializing = false)
    {
        if (minimapObj == null)
            minimapObj = FindObjectByTagIncludeInactive("Minimap");

        if (minimapObj != null)
            minimapObj.SetActive(isOn);

        if (!initializing)
        {
            PlayerPrefs.SetInt("Minimap", isOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        UpdateButtonColors(minimapOnButton, minimapOffButton, isOn);
    }

    void SetHealthBar(bool isOn, bool initializing = false)
    {
        if (healthBarObj == null)
            healthBarObj = FindObjectByTagIncludeInactive("HealthBar");

        if (healthBarObj != null)
            healthBarObj.SetActive(isOn);

        if (!initializing)
        {
            PlayerPrefs.SetInt("HealthBar", isOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        UpdateButtonColors(healthBarOnButton, healthBarOffButton, isOn);
    }

    void UpdateButtonColors(Button onButton, Button offButton, bool isOn)
    {
        SetButtonColor(onButton, isOn ? activeColor : inactiveColor);
        SetButtonColor(offButton, isOn ? inactiveColor : activeColor);
    }

    void SetButtonColor(Button button, Color color)
    {
        if (button == null)
            return;

        if (button.targetGraphic != null)
            button.targetGraphic.color = color;
    }

    public void ShowScreenModeContent()
    {
        if (GameplayContent != null)
            GameplayContent.SetActive(false);

        if (UIContent != null)
            UIContent.SetActive(false);

        if (ScreenModeContent != null)
            ScreenModeContent.SetActive(true);

        SetButtonColor(gameplayTabButton, inactiveColor);
        SetButtonColor(uiTabButton, inactiveColor);
        SetButtonColor(screenModeTabButton, activeColor);
    }

    GameObject FindObjectByTagIncludeInactive(string tag)
    {
        Transform[] allObjects = Resources.FindObjectsOfTypeAll<Transform>();

        foreach (Transform t in allObjects)
        {
            if (t.CompareTag(tag) && t.gameObject.scene.IsValid())
                return t.gameObject;
        }

        Debug.LogWarning("Không tìm thấy object tag: " + tag);
        return null;
    }
}