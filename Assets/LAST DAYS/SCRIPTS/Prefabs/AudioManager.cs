using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer mixer;

    public float player = 0.5f;
    public float environment = 0.5f;
    public float zombie = 0.5f;
    public float sfx = 0.5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
        ApplyAll();
    }

    public void ChangePlayer(float amount)
    {
        player = Mathf.Clamp01(player + amount);
        SaveAndApply("PlayerVolume", player);
    }

    public void ChangeEnvironment(float amount)
    {
        environment = Mathf.Clamp01(environment + amount);
        SaveAndApply("EnvironmentVolume", environment);
    }

    public void ChangeZombie(float amount)
    {
        zombie = Mathf.Clamp01(zombie + amount);
        SaveAndApply("ZombieVolume", zombie);
    }

    public void ChangeSFX(float amount)
    {
        sfx = Mathf.Clamp01(sfx + amount);
        SaveAndApply("SFXVolume", sfx);
    }

    void Load()
    {
        player = PlayerPrefs.GetFloat("PlayerVolume", 0.5f);
        environment = PlayerPrefs.GetFloat("EnvironmentVolume", 0.5f);
        zombie = PlayerPrefs.GetFloat("ZombieVolume", 0.5f);
        sfx = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
    }

    void ApplyAll()
    {
        SetMixer("PlayerVolume", player);
        SetMixer("EnvironmentVolume", environment);
        SetMixer("ZombieVolume", zombie);
        SetMixer("SFXVolume", sfx);
    }

    void SaveAndApply(string key, float value)
    {
        SetMixer(key, value);
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    void SetMixer(string key, float value)
    {
        mixer.SetFloat(key, value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f);
    }
}