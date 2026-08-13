using System.Collections;
using NUnit.Framework;
using UnityEngine;
using TMPro;
using UnityEngine.TestTools;

namespace LastDay.Tests.Integration
{
    public class GunSystemAmmoInventoryIntegrationTests
    {
        private GameObject parentObj;
        private GameObject gunObj;
        private GunSystem gunSystem;
        private AmmoInventory ammoInventory;
        private WeaponManager weaponManager;
        private GunData mockGunData;
        private InventoryData mockInventoryData;

        [SetUp]
        public void SetUp()
        {
            // 0. Xóa PlayerPrefs cũ để tránh dính level
            PlayerPrefs.DeleteAll();

            // 1. Tạo GameObject cha và TẠM TẮT
            parentObj = new GameObject("PlayerParent");
            parentObj.SetActive(false);

            // 2. Tạo GameObject con đại diện cho súng
            gunObj = new GameObject("AK47_Gun");
            gunObj.transform.SetParent(parentObj.transform);

            // 3. Khởi tạo InventoryData mẫu (Cấp đạn dự trữ tối đa = 90)
            mockInventoryData = ScriptableObject.CreateInstance<InventoryData>();
            mockInventoryData.maxReserveAmmo = 90;

            // 4. Khởi tạo GunData mẫu
            mockGunData = ScriptableObject.CreateInstance<GunData>();
            mockGunData.gunName = "TestGun";
            mockGunData.maxAmmo = 30;
            mockGunData.reloadTime = 0.1f;
            mockGunData.inventoryData = mockInventoryData; // Gán để tránh NullReferenceException ở dòng 31

            // Khởi tạo mảng cấp độ nâng cấp súng
            mockGunData.upgradeLevels = new GunUpgradeLevel[1];
            mockGunData.upgradeLevels[0] = new GunUpgradeLevel();

            // 5. Gắn GunSystem & Cấu hình dữ liệu
            gunSystem = gunObj.AddComponent<GunSystem>();
            gunSystem.gunData = mockGunData;

            // Gắn UI Text
            GameObject ammoTextObj = new GameObject("AmmoText");
            GameObject reserveTextObj = new GameObject("ReserveAmmoText");
            gunSystem.ammoText = ammoTextObj.AddComponent<TextMeshProUGUI>();
            gunSystem.reserveAmmoText = reserveTextObj.AddComponent<TextMeshProUGUI>();
            gunSystem.SetWeaponIndex(0);

            // 6. Gắn WeaponManager vào parentObj
            weaponManager = parentObj.AddComponent<WeaponManager>();
            weaponManager.weapons = new GameObject[] { gunObj };

            // 7. Gắn AmmoInventory vào parentObj
            ammoInventory = parentObj.AddComponent<AmmoInventory>();

            // 8. BẬT LẠI GameObject cha để Awake() và Start() chạy chuẩn thứ tự
            parentObj.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(parentObj);
            Object.Destroy(mockGunData);
            if (mockInventoryData != null) Object.Destroy(mockInventoryData);
            PlayerPrefs.DeleteAll();
        }

        // =========================================================================
        // CA TEST 1: Tích hợp Cập nhật UI Đạn
        // =========================================================================
        [UnityTest]
        public IEnumerator INT_01_UpdateAmmoUI_ReadsReserveAmmoFromInventory_UpdatesText()
        {
            gunSystem.currentAmmo = 15;

            gunSystem.UpdateAmmoUI();
            yield return null;

            Assert.AreEqual("15 / 30", gunSystem.ammoText.text,
                "LỖI TÍCH HỢP: GunSystem.UpdateAmmoUI() không hiển thị đúng chuỗi ammoText!");
        }

        // =========================================================================
        // CA TEST 2: Tích hợp Bắn súng & Trừ đạn
        // =========================================================================
        [UnityTest]
        public IEnumerator INT_02_Shoot_DecreasesCurrentAmmo_And_UpdatesAmmoUI()
        {
            gunSystem.currentAmmo = 30;
            gunSystem.UpdateAmmoUI();
            Assert.AreEqual("30 / 30", gunSystem.ammoText.text);

            // Giả lập 1 phát bắn
            gunSystem.currentAmmo--; 
            gunSystem.UpdateAmmoUI();
            yield return null;

            Assert.AreEqual(29, gunSystem.currentAmmo, "Súng bắn nhưng currentAmmo không giảm!");
            Assert.AreEqual("29 / 30", gunSystem.ammoText.text, "Súng bắn nhưng ammoText trên UI không cập nhật!");
        }

        // =========================================================================
        // CA TEST 3: BẮT BUG TÍCH HỢP (NẠP ĐẠN ẢO KHÔNG TRỪ KHO)
        // =========================================================================
        [UnityTest]
        public IEnumerator INT_03_Reload_WhenInventoryEmpty_ShouldNotRefillAmmo()
        {
            // Trừ sạch đạn trong kho về 0
            int currentReserve = ammoInventory.GetAmmo(0);
            ammoInventory.UseAmmo(0, currentReserve);

            // Súng cũng hết đạn (0/30)
            gunSystem.currentAmmo = 0;

            // Chạy Coroutine Reload
            yield return gunSystem.StartCoroutine("Reload");

            // Kiểm tra: Kho bằng 0 thì súng PHẢI giữ nguyên 0 viên
            Assert.AreEqual(0, gunSystem.currentAmmo, 
                "LỖI BUG TÍCH HỢP TÌM THẤY: Kho đạn dự trữ bằng 0 nhưng GunSystem vẫn tự nạp đầy 30 viên đạn ảo!");
        }
    }
}