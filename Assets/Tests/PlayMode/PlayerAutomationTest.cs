using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerAutomationTests
{
    private GameObject playerObj;
    private Health playerHealth;
    private CharData dummyCharData;
    private GameObject dummyBloodScreen;

    [SetUp]
    public void SetUp()
    {
        dummyCharData = ScriptableObject.CreateInstance<CharData>();
        dummyCharData.maxHealth = 100f;

        playerObj = new GameObject("Player_Test");
        playerObj.SetActive(false);

        playerHealth = playerObj.AddComponent<Health>();
        playerHealth.charData = dummyCharData;

        GameObject particleObj = new GameObject("HealParticle");
        particleObj.transform.SetParent(playerObj.transform);
        playerHealth.HealEffect = particleObj.AddComponent<ParticleSystem>();

        dummyBloodScreen = new GameObject("DummyBloodScreen");
        playerHealth.BloodScreenObj = dummyBloodScreen;

        playerObj.SetActive(true);
    }

    [TearDown]
    public void TearDown()
    {
        if (playerObj != null) Object.Destroy(playerObj);
        if (dummyCharData != null) Object.Destroy(dummyCharData);
        if (dummyBloodScreen != null) Object.Destroy(dummyBloodScreen);
    }

    [UnityTest]
    public IEnumerator AutomatedTest_PlayerHealth_TakeDamage_ReducesHPAndTriggersCooldown()
    {
        playerHealth.TakeDamage(30f);

        Assert.AreEqual(70f, playerHealth.currentHealth, "FAILED: Máu hiện tại sau khi nhận 30 damage không bằng 70 HP!");
        Assert.AreEqual(30f, playerHealth.totalDamageTaken, "FAILED: Tổng sát thương totalDamageTaken ghi nhận sai!");

        playerHealth.TakeDamage(40f);

        Assert.AreEqual(70f, playerHealth.currentHealth, "FAILED: Player bị nhận sát thương trong thời gian miễn sát thương (Cooldown)!");
        yield return null;
    }

    [UnityTest]
    public IEnumerator AutomatedTest_PlayerHealth_Heal_ClampsToMaxHealth()
    {
        playerHealth.TakeDamage(50f);
        Assert.AreEqual(50f, playerHealth.currentHealth);

        yield return new WaitForSeconds(1.1f);
        playerHealth.Heal(80f);

        Assert.AreEqual(100f, playerHealth.currentHealth, "FAILED: Hàm Heal không khống chế mốc Máu tối đa (maxHealth = 100 HP)!");
    }

    [Test]
    public void AutomatedTest_PlayerHealth_Heal_MissingParticleEffect_ThrowsNullCrash()
    {
        playerHealth.HealEffect = null;

        Assert.Throws<System.NullReferenceException>(() => playerHealth.Heal(20f),
            "FAILED (BUG THẬT CONFIRMED): Hàm Heal() văng NullReferenceException do không check null HealEffect!");
    }

    [UnityTest]
    public IEnumerator AutomatedTest_PlayerInteract_NoMainCamera_CausesNullCrashInUpdate()
    {
        GameObject interactObj = new GameObject("InteractObj");
        PlayerInteract interact = interactObj.AddComponent<PlayerInteract>();
        interact.cam = null;

        LogAssert.Expect(LogType.Exception, new Regex(".*NullReferenceException.*"));

        yield return null;

        Object.Destroy(interactObj);
    }

    [UnityTest]
    public IEnumerator AutomatedTest_WeaponAnimationSwitcher_MissingAnimator_ThrowsNullCrash()
    {
        GameObject switcherObj = new GameObject("SwitcherObj");
        WeaponAnimationSwitcher switcher = switcherObj.AddComponent<WeaponAnimationSwitcher>();

        LogAssert.Expect(LogType.Exception, new Regex(".*NullReferenceException.*"));

        switcher.SendMessage("OnSwitchWeapon", default(UnityEngine.InputSystem.InputAction.CallbackContext), SendMessageOptions.DontRequireReceiver);
        yield return null;

        Object.Destroy(switcherObj);
    }
}