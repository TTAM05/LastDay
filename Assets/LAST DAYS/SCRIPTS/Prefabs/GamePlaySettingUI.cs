using UnityEngine;
using UnityEngine.UI;

public class GameplaySettingUI : MonoBehaviour
{
    public Toggle minimapToggle;
    public Toggle healthBarToggle;

    GameObject minimapObj;
    GameObject healthBarObj;

    void Start()
    {
        minimapObj = FindObjectByTagIncludeInactive("Minimap");
        healthBarObj = FindObjectByTagIncludeInactive("HealthBar");

        bool minimap = PlayerPrefs.GetInt("Minimap", 1) == 1;
        bool healthbar = PlayerPrefs.GetInt("HealthBar", 1) == 1;

        minimapToggle.onValueChanged.RemoveAllListeners();
        healthBarToggle.onValueChanged.RemoveAllListeners();

        minimapToggle.isOn = minimap;
        healthBarToggle.isOn = healthbar;

        ApplyMinimap(minimap);
        ApplyHealthBar(healthbar);

        minimapToggle.onValueChanged.AddListener(ApplyMinimap);
        healthBarToggle.onValueChanged.AddListener(ApplyHealthBar);
    }

    void ApplyMinimap(bool isOn)
    {
        if (minimapObj == null)
            minimapObj = FindObjectByTagIncludeInactive("Minimap");

        if (minimapObj != null)
            minimapObj.SetActive(isOn);

        PlayerPrefs.SetInt("Minimap", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    void ApplyHealthBar(bool isOn)
    {
        if (healthBarObj == null)
            healthBarObj = FindObjectByTagIncludeInactive("HealthBar");

        if (healthBarObj != null)
            healthBarObj.SetActive(isOn);

        PlayerPrefs.SetInt("HealthBar", isOn ? 1 : 0);
        PlayerPrefs.Save();
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