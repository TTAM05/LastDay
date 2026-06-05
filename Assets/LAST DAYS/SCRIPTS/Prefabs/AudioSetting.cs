using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioUIManager : MonoBehaviour
{

    [Header("Fill Images")]

    public Image environmentFill;
    public Image masterFill;
    public Image sfxFill;

    void Start()
    {
       RefreshUI();
    }

    public void EnvironmentMinus()
    {
        AudioManager.Instance.ChangeEnvironment(-0.1f);
        RefreshUI();
    }

    public void EnvironmentPlus()
    {
        AudioManager.Instance.ChangeEnvironment(0.1f);
        RefreshUI();
    }

    public void MasterMinus()
    {
        AudioManager.Instance.ChangeMaster(-0.1f);
        RefreshUI();
    }

    public void MasterPlus()
    {
        AudioManager.Instance.ChangeMaster(0.1f);
        RefreshUI();
    }

    public void SFXMinus()
    {
        AudioManager.Instance.ChangeSFX(-0.1f);
        RefreshUI();
    }

    public void SFXPlus()
    {
        AudioManager.Instance.ChangeSFX(0.1f);
        RefreshUI();
    }

    void RefreshUI()
    {
    
        environmentFill.fillAmount = AudioManager.Instance.environment;
        masterFill.fillAmount = AudioManager.Instance.master;
        sfxFill.fillAmount = AudioManager.Instance.sfx;
    }
}