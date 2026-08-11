using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

public class AudioAutomationTests
{
    private GameObject uiGameObject;
    private AudioUIManager audioUIManager;
    private GameObject audioManagerObject;

    [SetUp]
    public void SetUp()
    {
        audioManagerObject = new GameObject("AudioManager");
        if (AudioManager.Instance == null) audioManagerObject.AddComponent<AudioManager>();

        uiGameObject = new GameObject("AudioUIManager");
        audioUIManager = uiGameObject.AddComponent<AudioUIManager>();

        audioUIManager.environmentFill = new GameObject("EnvFill").AddComponent<Image>();
        audioUIManager.masterFill = new GameObject("MasterFill").AddComponent<Image>();
        audioUIManager.sfxFill = new GameObject("SFXFill").AddComponent<Image>();
        audioUIManager.sfxFill.type = Image.Type.Filled;

        if (AudioManager.Instance != null) AudioManager.Instance.sfx = 0.5f;
        audioUIManager.sfxFill.fillAmount = 0.5f;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(uiGameObject);
        Object.DestroyImmediate(audioManagerObject);
    }

    private void InvokePrivateRefreshUI(AudioUIManager manager)
    {
        MethodInfo method = typeof(AudioUIManager).GetMethod("RefreshUI", BindingFlags.NonPublic | BindingFlags.Instance);
        method?.Invoke(manager, null);
    }

    [UnityTest]
    public IEnumerator TC_AUTO_AUD_01_SFXPlus_IncreasesVolumeAndFillAmount()
    {
        float initialSfx = AudioManager.Instance.sfx;

        audioUIManager.SFXPlus();
        yield return null;

        Assert.AreEqual(initialSfx + 0.1f, AudioManager.Instance.sfx, 0.001f, 
            "[FAILED] Logic sai: Giá trị SFX trong AudioManager không tăng!");
        Assert.AreEqual(AudioManager.Instance.sfx, audioUIManager.sfxFill.fillAmount, 0.001f, 
            "[FAILED] Lỗi Freeze UI: Thanh sfxFill.fillAmount không cập nhật theo AudioManager!");
    }

    [UnityTest]
    public IEnumerator TC_AUTO_AUD_02_SFXMinus_DecreasesVolumeAndFillAmount()
    {
        float initialSfx = AudioManager.Instance.sfx;

        audioUIManager.SFXMinus();
        yield return null;

        Assert.AreEqual(initialSfx - 0.1f, AudioManager.Instance.sfx, 0.001f, 
            "[FAILED] Logic sai: Giá trị SFX trong AudioManager không giảm!");
        Assert.AreEqual(AudioManager.Instance.sfx, audioUIManager.sfxFill.fillAmount, 0.001f, 
            "[FAILED] Lỗi Freeze UI: Thanh sfxFill.fillAmount không cập nhật theo AudioManager!");
    }
}