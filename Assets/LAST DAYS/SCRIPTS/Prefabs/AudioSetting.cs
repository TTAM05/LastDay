using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioUIManager : MonoBehaviour
{

    [Header("Fill Images")]
    public Image playerFill;
    public Image environmentFill;
    public Image zombieFill;
    public Image sfxFill;

    void Start()
    {
       RefreshUI();
    }

      public void PlayerMinus()
    {
        AudioManager.Instance.ChangePlayer(-0.1f);
        RefreshUI();
    }

    public void PlayerPlus()
    {
        AudioManager.Instance.ChangePlayer(0.1f);
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

    public void ZombieMinus()
    {
        AudioManager.Instance.ChangeZombie(-0.1f);
        RefreshUI();
    }

    public void ZombiePlus()
    {
        AudioManager.Instance.ChangeZombie(0.1f);
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
        playerFill.fillAmount = AudioManager.Instance.player;
        environmentFill.fillAmount = AudioManager.Instance.environment;
        zombieFill.fillAmount = AudioManager.Instance.zombie;
        sfxFill.fillAmount = AudioManager.Instance.sfx;
    }
}