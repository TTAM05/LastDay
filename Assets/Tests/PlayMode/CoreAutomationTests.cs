using System;
using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class CoreAutomationTests
{
    private GunData testGun;
    private GameObject shopObj;
    private ShopManager shopManager;
    private GameObject settingObj;
    private GameSetting gameSetting;

    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();

        testGun = ScriptableObject.CreateInstance<GunData>();
        testGun.gunName = "AK47_Test";
        testGun.damage = 20f;
        testGun.recoilX = 1.0f;
        testGun.recoilY = 1.0f;
        testGun.Price = 300;
        testGun.currencyType = CurrencyType.Money;

        testGun.upgradeLevels = new GunUpgradeLevel[4];
        for (int i = 0; i < 4; i++)
        {
            testGun.upgradeLevels[i] = new GunUpgradeLevel
            {
                damageBonus = (i + 1) * 5f,
                recoilBonus = (i + 1) * 0.2f
            };
        }

        shopObj = new GameObject("ShopManager");
        shopManager = shopObj.AddComponent<ShopManager>();
        shopManager.moneyText = new GameObject("MoneyText").AddComponent<TextMeshProUGUI>();
        shopManager.crystalText = new GameObject("CrystalText").AddComponent<TextMeshProUGUI>();

        settingObj = new GameObject("GameSetting");
        gameSetting = settingObj.AddComponent<GameSetting>();
        gameSetting.optionPanel = new GameObject("OptionPanel");
    }

    [TearDown]
    public void TearDown()
    {
        if (testGun != null) Object.Destroy(testGun);
        if (shopObj != null) Object.Destroy(shopObj);
        if (settingObj != null) Object.Destroy(settingObj);
        PlayerPrefs.DeleteAll();
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    [Test]
    public void AutomatedTest_GunUpgradeCalculator_CalculatesDamageAndRecoilCorrectly()
    {
        PlayerPrefs.SetInt(testGun.gunName + "_Level", 0);

        Assert.AreEqual(25f, GunUpgradeCalculator.GetDamage(testGun), "FAILED: Tính toán Damage cấp 0 bị sai!");
        Assert.AreEqual(0.8f, GunUpgradeCalculator.GetRecoilX(testGun), 0.001f, "FAILED: Tính toán RecoilX cấp 0 bị sai!");
    }

    [Test]
    public void AutomatedTest_GunUpgradeCalculator_LevelOverflow_ThrowsIndexException()
    {
        PlayerPrefs.SetInt(testGun.gunName + "_Level", 5);

        Assert.Throws<IndexOutOfRangeException>(() => GunUpgradeCalculator.GetDamage(testGun),
            "FAILED (BUG THẬT CONFIRMED): GunUpgradeCalculator không kiểm tra giới hạn mảng upgradeLevels khi Level bị vượt mốc!");
    }

    [Test]
    public void AutomatedTest_GameSetting_OpenAndCloseOption_TogglesTimeScaleAndAudio()
    {
        gameSetting.OpenOption();

        Assert.AreEqual(0f, Time.timeScale, "FAILED: OpenOption không đưa Time.timeScale về 0!");
        Assert.IsTrue(AudioListener.pause, "FAILED: OpenOption không tạm dừng AudioListener!");
        Assert.IsTrue(gameSetting.optionPanel.activeSelf, "FAILED: OptionPanel chưa được bật!");

        gameSetting.CloseOption();

        Assert.AreEqual(1f, Time.timeScale, "FAILED: CloseOption không khôi phục Time.timeScale về 1!");
        Assert.IsFalse(AudioListener.pause, "FAILED: CloseOption không khôi phục AudioListener!");
        Assert.IsFalse(gameSetting.optionPanel.activeSelf, "FAILED: OptionPanel chưa được tắt!");
    }

    [Test]
    public void AutomatedTest_ShopManager_BuyGun_SuccessfulPurchaseDeductsMoney()
    {
        shopManager.AddMoney(1000);

        Assert.IsTrue(shopManager.BuyGun(testGun), "FAILED: Đủ tiền nhưng giao dịch BuyGun trả về false!");
        Assert.AreEqual(700, shopManager.Money, "FAILED: Số dư tiền trong Ví sau khi mua không bị trừ đúng 300 xu!");
        Assert.AreEqual(700, PlayerPrefs.GetInt("Money"), "FAILED: Dữ liệu Tiền trong PlayerPrefs chưa được lưu đồng bộ!");
    }

    [Test]
    public void AutomatedTest_ShopManager_BuyGun_InsufficientFunds_RejectsPurchase()
    {
        shopManager.AddMoney(100);

        Assert.IsFalse(shopManager.BuyGun(testGun), "FAILED: Không đủ tiền nhưng giao dịch BuyGun vẫn báo thành công!");
        Assert.AreEqual(100, shopManager.Money, "FAILED: Giao dịch thất bại nhưng số dư tiền trong Ví vẫn bị thay đổi!");
    }
}