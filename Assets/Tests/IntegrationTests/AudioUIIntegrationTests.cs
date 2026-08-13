using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

namespace LastDay.Tests.Integration
{
    public class AudioUIIntegrationTests
    {
        private GameObject audioManagerObj;
        private GameObject uiManagerObj;
        private AudioManager audioManager;
        private AudioUIManager audioUIManager;

        private Image masterFill;
        private Image sfxFill;
        private Image envFill;

        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteAll();

            // 1. Khởi tạo AudioManager
            audioManagerObj = new GameObject("AudioManager");
            audioManager = audioManagerObj.AddComponent<AudioManager>();

            // Gán giá trị mặc định thử nghiệm
            audioManager.master = 0.5f;
            audioManager.sfx = 0.5f;
            audioManager.environment = 0.5f;

            // 2. Khởi tạo AudioUIManager và các Image Fill
            uiManagerObj = new GameObject("AudioUIManager");
            audioUIManager = uiManagerObj.AddComponent<AudioUIManager>();

            GameObject mObj = new GameObject("MasterFill");
            masterFill = mObj.AddComponent<Image>();

            GameObject sObj = new GameObject("SFXFill");
            sfxFill = sObj.AddComponent<Image>();

            GameObject eObj = new GameObject("EnvFill");
            envFill = eObj.AddComponent<Image>();

            // Gán tham chiếu UI
            audioUIManager.masterFill = masterFill;
            audioUIManager.sfxFill = sfxFill;
            audioUIManager.environmentFill = envFill;
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(audioManagerObj);
            Object.Destroy(uiManagerObj);
            PlayerPrefs.DeleteAll();
        }

        // =========================================================================
        // TC 01: Nhấn MasterPlus -> AudioManager tăng giá trị & UI FillAmount đồng bộ
        // =========================================================================
        [UnityTest]
        public IEnumerator INT_AUDIO_01_MasterPlus_UpdatesAudioManagerAndUIFillAmount()
        {
            audioManager.master = 0.5f;

            audioUIManager.MasterPlus();
            yield return null;

            Assert.AreEqual(0.6f, audioManager.master, 0.001f, 
                "LỖI TÍCH HỢP: AudioManager.master không tăng lên 0.6!");
            Assert.AreEqual(0.6f, masterFill.fillAmount, 0.001f, 
                "LỖI TÍCH HỢP UI: masterFill.fillAmount không đồng bộ về 0.6!");
        }

        // =========================================================================
        // TC 02: Nhấn MasterPlus vượt ngưỡng 1.0 -> Khống chế Clamp01 tại 1.0 trên UI
        // =========================================================================
        [UnityTest]
        public IEnumerator INT_AUDIO_02_VolumeExceedingMax_ClampsToOneAndSyncsUI()
        {
            audioManager.master = 0.95f;

            audioUIManager.MasterPlus();
            yield return null;

            Assert.AreEqual(1.0f, audioManager.master, 0.001f, 
                "LỖI TÍCH HỢP: AudioManager.master bị vượt quá ngưỡng 1.0!");
            Assert.AreEqual(1.0f, masterFill.fillAmount, 0.001f, 
                "LỖI TÍCH HỢP UI: masterFill.fillAmount không giữ nguyên tại 1.0!");
        }

        // =========================================================================
        // TC 03: Nhấn SFXMinus -> Cập nhật UI & Lưu dữ liệu vào PlayerPrefs
        // =========================================================================
        [UnityTest]
        public IEnumerator INT_AUDIO_03_SFXMinus_SavesUpdatedVolumeToPlayerPrefs()
        {
            audioManager.sfx = 0.5f;

            audioUIManager.SFXMinus();
            yield return null;

            float savedValue = PlayerPrefs.GetFloat("SFXVolume", -1f);
            Assert.AreEqual(0.4f, savedValue, 0.001f, 
                "LỖI TÍCH HỢP LƯU TRỮ: PlayerPrefs không lưu đúng SFXVolume = 0.4!");
            Assert.AreEqual(0.4f, sfxFill.fillAmount, 0.001f, 
                "LỖI TÍCH HỢP UI: sfxFill.fillAmount không cập nhật về 0.4!");
        }
    }
}