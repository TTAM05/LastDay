using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

namespace LastDay.Tests.Integration
{
    public class PlayerHealthUIIntegrationTests
    {
        private GameObject playerObj;
        private Health health;
        private CharData mockCharData;
        private Image healthBarFill;
        private GameObject gameOverPanel;
        private ParticleSystem healEffect;
        private GameObject bloodScreenObj;

        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteAll();
            Time.timeScale = 1f;
            AudioListener.pause = false;

            // 1. Khởi tạo Player root và TẠM TẮT để nạp reference an toàn
            playerObj = new GameObject("PlayerRoot");
            playerObj.SetActive(false);

            // 2. Khởi tạo Mock CharData
            mockCharData = ScriptableObject.CreateInstance<CharData>();
            mockCharData.maxHealth = 100f;

            // 3. Khởi tạo UI Image (healthBarFill)
            GameObject canvasObj = new GameObject("Canvas");
            GameObject fillObj = new GameObject("HealthBarFill");
            fillObj.transform.SetParent(canvasObj.transform);
            healthBarFill = fillObj.AddComponent<Image>();

            // 4. Khởi tạo UI Panel GameOver
            gameOverPanel = new GameObject("GameOverPanel");
            gameOverPanel.SetActive(false);

            // 5. Khởi tạo ParticleSystem HealEffect
            GameObject particleObj = new GameObject("HealEffect");
            particleObj.transform.SetParent(playerObj.transform);
            healEffect = particleObj.AddComponent<ParticleSystem>();

            // 6. Khởi tạo Prefab BloodScreenObj
            bloodScreenObj = new GameObject("BloodScreen_Mock");

            // 7. Gắn script Health và gán dữ liệu
            health = playerObj.AddComponent<Health>();
            health.charData = mockCharData;
            health.healthBarFill = healthBarFill;
            health.GameOverPanel = gameOverPanel;
            health.HealEffect = healEffect;
            health.BloodScreenObj = bloodScreenObj;

            // 8. Bật lại GameObject để Awake() khởi chạy
            playerObj.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(playerObj);
            Object.Destroy(mockCharData);
            Object.Destroy(bloodScreenObj);
            
            Time.timeScale = 1f;
            AudioListener.pause = false;
            PlayerPrefs.DeleteAll();
        }

        // =========================================================================
        // TC 01: Gameplay (TakeDamage) -> UI (healthBarFill.fillAmount)
        // =========================================================================
        [UnityTest]
        public IEnumerator INT_UI_01_TakeDamage_DecreasesHealthBarFillAmount()
        {
            Assert.AreEqual(1.0f, healthBarFill.fillAmount);

            health.TakeDamage(30f);
            yield return null;

            Assert.AreEqual(0.7f, healthBarFill.fillAmount, 0.01f,
                "LỖI TÍCH HỢP UI: Player nhận 30 damage nhưng healthBarFill.fillAmount không cập nhật về 0.7!");
        }

        // =========================================================================
        // TC 02: Gameplay (Heal) -> UI (healthBarFill.fillAmount)
        // =========================================================================
        [UnityTest]
        public IEnumerator INT_UI_02_Heal_IncreasesHealthBarFillAmountToMax()
        {
            health.TakeDamage(50f);
            yield return null;

            health.Heal(50f);
            yield return null;

            Assert.AreEqual(1.0f, healthBarFill.fillAmount, 0.01f,
                "LỖI TÍCH HỢP UI: Player hồi máu nhưng healthBarFill.fillAmount không đồng bộ về 1.0!");
        }

        // =========================================================================
        // TC 03: Gameplay (Die) -> Disable Gameplay & Enable GameOverPanel UI
        // =========================================================================
        [UnityTest]
        public IEnumerator INT_UI_03_PlayerDie_DisablesGunSystemAndActivatesGameOverPanel()
        {
            GameObject gunObj = new GameObject("AK47");
            gunObj.transform.SetParent(playerObj.transform);
            GunSystem gunSystem = gunObj.AddComponent<GunSystem>();

            health.TakeDamage(150f);

            // Chờ 2.1s cho Coroutine FreezeAfterDelay(2f) chạy xong
            yield return new WaitForSecondsRealtime(2.1f);

            Assert.IsTrue(health.IsDead, "Player nhận sát thương chí mạng nhưng IsDead vẫn bằng false!");
            Assert.IsFalse(gunSystem.enabled, "Player chết nhưng GunSystem chưa bị vô hiệu hóa!");
            Assert.IsTrue(gameOverPanel.activeSelf, "Player chết nhưng GameOverPanel UI không hiển thị!");
        }
    }
}