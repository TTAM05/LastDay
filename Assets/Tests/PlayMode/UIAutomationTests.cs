using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIAutomationTests
{
    private GameObject FindObjectEvenIfInactive(string name)
    {
        foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (t.hideFlags == HideFlags.None && t.name == name) return t.gameObject;
        }
        return null;
    }

    [UnityTest]
    public IEnumerator AutomatedTest_Lobby_OptionsPanel_Navigation()
    {
        SceneManager.LoadScene("Start");
        yield return null;

        GameObject optionPanel = FindObjectEvenIfInactive("OptionPanel 1") ?? FindObjectEvenIfInactive("OptionPanel");
        GameObject settingBtn = FindObjectEvenIfInactive("SETTING") ?? FindObjectEvenIfInactive("Setting");

        Assert.IsNotNull(settingBtn, "LỖI: Không tìm thấy nút SETTING trong Scene Start!");
        Assert.IsNotNull(optionPanel, "LỖI: Không tìm thấy OptionPanel trong Scene Start!");

        Button btnSetting = settingBtn.GetComponent<Button>();
        Assert.IsNotNull(btnSetting, "LỖI: GameObject SETTING thiếu Component UI.Button!");

        btnSetting.onClick.Invoke();

        float timeout = 1.0f;
        while (!optionPanel.activeInHierarchy && timeout > 0)
        {
            timeout -= Time.unscaledDeltaTime;
            yield return null;
        }

        Assert.IsTrue(optionPanel.activeInHierarchy, "FAILED: Đã click SETTING nhưng OptionPanel vẫn không hiển thị (Inactive)!");

        GameObject backBtn = FindObjectEvenIfInactive("BACK") ?? FindObjectEvenIfInactive("BackButton") ?? FindObjectEvenIfInactive("<- BACK");
        Assert.IsNotNull(backBtn, "LỖI: Không tìm thấy nút BACK!");

        Button btnBack = backBtn.GetComponent<Button>();
        Assert.IsNotNull(btnBack, "LỖI: GameObject nút BACK thiếu Component UI.Button!");

        btnBack.onClick.Invoke();

        timeout = 1.5f;
        while (optionPanel.activeInHierarchy && timeout > 0)
        {
            timeout -= Time.unscaledDeltaTime;
            yield return null;
        }

        Assert.IsFalse(optionPanel.activeInHierarchy, "FAILED: Đã click BACK nhưng OptionPanel vẫn chưa ẩn (Active)!");
    }

    [UnityTest]
    public IEnumerator AutomatedTest_Healthbar_ShouldBeVisible_OnGameStart()
    {
        SceneManager.LoadScene("Map1");
        yield return null;
        yield return new WaitForSeconds(0.5f);

        GameObject healthBar = GameObject.FindWithTag("HealthBar") ?? FindObjectEvenIfInactive("HealthBar");

        Assert.IsNotNull(healthBar, "FAILED: GameObject HealthBar không tồn tại trong Scene Map1!");
        Assert.IsTrue(healthBar.activeInHierarchy, "FAILED (BUG THẬT): HealthBar bị ẩn (Inactive) khi vừa vào trận! Cần thao tác Pause/Resume mới hiển thị.");
    }

    [UnityTest]
    public IEnumerator AutomatedTest_SpamClick_SettingButton_FrameByFrame_Stability()
    {
        LogAssert.ignoreFailingMessages = true;

        SceneManager.LoadScene("Start");
        yield return null;

        GameObject settingBtnObj = FindObjectEvenIfInactive("SETTING");
        Assert.IsNotNull(settingBtnObj, "LỖI AUTOMATION: Không tìm thấy nút SETTING!");

        Button settingBtn = settingBtnObj.GetComponent<Button>();

        for (int i = 0; i < 5; i++)
        {
            settingBtn.onClick.Invoke();
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);

        GameObject optionPanel = FindObjectEvenIfInactive("OptionPanel 1") ?? FindObjectEvenIfInactive("OptionPanel");
        Assert.IsNotNull(optionPanel, "FAILED: OptionPanel bị crash/Destroy khỏi bộ nhớ sau khi dồn dập click!");
    }

    [UnityTest]
    public IEnumerator AutomatedTest_HealthBar_FillAmount_SyncOnDamage()
    {
        SceneManager.LoadScene("Map1");
        yield return new WaitForSeconds(0.5f);

        Health playerHealth = Object.FindFirstObjectByType<Health>() ?? FindObjectEvenIfInactive("Player")?.GetComponent<Health>();

        Assert.IsNotNull(playerHealth, "LỖI AUTOMATION: Không tìm thấy Component Health!");
        Assert.IsNotNull(playerHealth.healthBarFill, "LỖI AUTOMATION: Chưa gán healthBarFill!");

        float maxHP = playerHealth.charData.maxHealth;
        Assert.AreEqual(1f, playerHealth.healthBarFill.fillAmount, 0.01f, "LỖI: Thanh máu ban đầu không đầy 100%!");

        float damageToApply = 30f;
        playerHealth.TakeDamage(damageToApply);
        yield return null;

        float expectedFillAmount = (maxHP - damageToApply) / maxHP;
        Assert.AreEqual(expectedFillAmount, playerHealth.healthBarFill.fillAmount, 0.001f,
            $"FAILED: Thanh máu UI không đồng bộ! Kỳ vọng = {expectedFillAmount}, Thực tế = {playerHealth.healthBarFill.fillAmount}");
    }

    [UnityTest]
    public IEnumerator AutomatedTest_LuckyWheel_BackButton_MustBeDisabled_DuringSpin()
    {
        SceneManager.LoadScene("Lucky");
        yield return null;
        yield return new WaitForSeconds(0.5f);

        GameObject spinBtnObj = FindObjectEvenIfInactive("Spin");
        GameObject backBtnObj = FindObjectEvenIfInactive("Back");

        Assert.IsNotNull(spinBtnObj, "LỖI AUTOMATION: Không tìm thấy GameObject 'Spin' trong Scene Lucky!");
        Assert.IsNotNull(backBtnObj, "LỖI AUTOMATION: Không tìm thấy GameObject 'Back' trong Scene Lucky!");

        Button spinBtn = spinBtnObj.GetComponent<Button>();
        Button backBtn = backBtnObj.GetComponent<Button>();

        Assert.IsNotNull(spinBtn, "LỖI AUTOMATION: GameObject 'Spin' thiếu Component UI.Button!");
        Assert.IsNotNull(backBtn, "LỖI AUTOMATION: GameObject 'Back' thiếu Component UI.Button!");

        spinBtn.onClick.Invoke();
        yield return null;

        Assert.IsFalse(backBtn.interactable, 
            "FAILED (BUG UI LOGIC): Đã kích hoạt LuckyWheel.Spin() nhưng nút BACK vẫn mở (interactable = true)! Người chơi bấm nút BACK sẽ gọi MenuController.GoMenu() làm chuyển Scene và mất trắng vé quay.");
    }
}