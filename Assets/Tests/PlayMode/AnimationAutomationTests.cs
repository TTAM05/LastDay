using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class AnimationAutomationTests
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
    public IEnumerator AutomatedTest_Gun_FreeReloadBug_WhenReserveAmmoIsEmpty()
    {
        SceneManager.LoadScene("Map1");
        yield return null;
        yield return new WaitForSeconds(0.5f);

        GunSystem gun = Object.FindFirstObjectByType<GunSystem>();
        Assert.IsNotNull(gun, "LỖI AUTOMATION: Không tìm thấy GunSystem!");

        AmmoInventory inventory = gun.GetComponentInParent<AmmoInventory>();
        Assert.IsNotNull(inventory, "LỖI AUTOMATION: Không tìm thấy AmmoInventory!");

        gun.currentAmmo = 1;
        for (int i = 0; i < 5; i++)
        {
            int reserve = inventory.GetAmmo(i);
            if (reserve > 0) inventory.UseAmmo(i, reserve);
        }
        Assert.AreEqual(0, inventory.GetAmmo(0), "LỖI SETUP: Kho đạn dự trữ chưa về 0!");

        MethodInfo onReloadMethod = typeof(GunSystem).GetMethod("OnReloadPressed", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(onReloadMethod, "LỖI AUTOMATION: Không tìm thấy hàm OnReloadPressed!");

        onReloadMethod.Invoke(gun, new object[] { default(InputAction.CallbackContext) });
        yield return new WaitForSeconds(0.1f);

        Assert.IsFalse(gun.IsReloading(), "FAILED: Kho đạn dự trữ bằng 0 nhưng súng vẫn cho phép nạp đạn ảo!");
    }

    [UnityTest]
    public IEnumerator AutomatedTest_Weapon_EquipGrenade_InterruptsActiveReload()
    {
        SceneManager.LoadScene("Map1");
        yield return null;
        yield return new WaitForSeconds(0.5f);

        GunSystem gun = Object.FindFirstObjectByType<GunSystem>();
        WeaponManager weaponManager = Object.FindFirstObjectByType<WeaponManager>();

        Assert.IsNotNull(gun, "LỖI AUTOMATION: Không tìm thấy GunSystem!");
        Assert.IsNotNull(weaponManager, "LỖI AUTOMATION: Không tìm thấy WeaponManager!");

        gun.currentAmmo = 1;
        MethodInfo onReloadMethod = typeof(GunSystem).GetMethod("OnReloadPressed", BindingFlags.NonPublic | BindingFlags.Instance);
        onReloadMethod.Invoke(gun, new object[] { default(InputAction.CallbackContext) });
        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(gun.IsReloading(), "LỖI SETUP: Súng chưa vào trạng thái Reloading!");

        PlayerPrefs.SetInt("Grenade", 3);
        MethodInfo equipGrenadeMethod = typeof(WeaponManager).GetMethod("EquipGrenade", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(equipGrenadeMethod, "LỖI AUTOMATION: Không tìm thấy hàm EquipGrenade!");

        equipGrenadeMethod.Invoke(weaponManager, null);
        yield return null;

        Assert.AreNotEqual(-1, weaponManager.currentWeapon, 
            "FAILED: Đang nạp đạn nhưng vẫn cho phép rút lựu đạn làm ngắt Coroutine Reload dở dang!");
    }

    [UnityTest]
    public IEnumerator AutomatedTest_Weapon_SelectWeapon_ShouldBeDisabled_WhenPlayerIsDead()
    {
        SceneManager.LoadScene("Map1");
        yield return null;
        yield return new WaitForSeconds(0.5f);

        FPSController fps = Object.FindFirstObjectByType<FPSController>();
        WeaponManager weaponManager = Object.FindFirstObjectByType<WeaponManager>();

        Assert.IsNotNull(fps, "LỖI AUTOMATION: Không tìm thấy FPSController!");
        Assert.IsNotNull(weaponManager, "LỖI AUTOMATION: Không tìm thấy WeaponManager!");

        int initialWeaponIndex = weaponManager.currentWeapon;

        fps.Die();
        yield return new WaitForSeconds(0.2f);

        weaponManager.SelectWeapon(1);
        yield return null;

        Assert.AreEqual(initialWeaponIndex, weaponManager.currentWeapon, 
            $"FAILED: Player đã chết nhưng vẫn cho phép chuyển từ súng {initialWeaponIndex} sang súng {weaponManager.currentWeapon}!");
    }
}