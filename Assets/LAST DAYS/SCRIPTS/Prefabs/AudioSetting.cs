using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer mixer;

    [Header("Sliders")]
    public Slider environmentSlider;
    public Slider playerSlider;
    public Slider sfxSlider;
    public Slider zombieSlider;

    void Start()
    {
        // Load saved values
        environmentSlider.value = PlayerPrefs.GetFloat("EnvironmentVolume", 1f);
        playerSlider.value = PlayerPrefs.GetFloat("PlayerVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        zombieSlider.value = PlayerPrefs.GetFloat("ZombieVolume", 1f);

        // Add listeners
        environmentSlider.onValueChanged.AddListener(SetEnvironmentVolume);
        playerSlider.onValueChanged.AddListener(SetPlayerVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        zombieSlider.onValueChanged.AddListener(SetZombieVolume);

        // Apply loaded values
        SetEnvironmentVolume(environmentSlider.value);
        SetPlayerVolume(playerSlider.value);
        SetSFXVolume(sfxSlider.value);
        SetZombieVolume(zombieSlider.value);
    }

    public void SetEnvironmentVolume(float value)
    {
        SetVolume("EnvironmentVolume", value);
    }

    public void SetPlayerVolume(float value)
    {
        SetVolume("PlayerVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        SetVolume("SFXVolume", value);
    }

    public void SetZombieVolume(float value)
    {
        SetVolume("ZombieVolume", value);
    }

    void SetVolume(string parameter, float value)
    {
        // Save
        PlayerPrefs.SetFloat(parameter, value);

        // Apply volume
        if (value <= 0.0001f)
            mixer.SetFloat(parameter, -80f);
        else
            mixer.SetFloat(parameter, Mathf.Log10(value) * 20);
    }
}