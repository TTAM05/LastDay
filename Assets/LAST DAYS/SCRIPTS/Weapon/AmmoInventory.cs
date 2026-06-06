using UnityEngine;

public class AmmoInventory : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioSource audioSource;

    [Header("Inventory")]
    [SerializeField] private InventoryData inventoryData;

    private int[] reserveAmmoByWeapon;

    void Start()
    {
        WeaponManager weaponManager = GetComponentInChildren<WeaponManager>();

        if (weaponManager == null)
        {
            Debug.LogError("Không tìm thấy WeaponManager");
            return;
        }

        reserveAmmoByWeapon = new int[weaponManager.weapons.Length];

        for (int i = 0; i < reserveAmmoByWeapon.Length; i++)
        {
            reserveAmmoByWeapon[i] = inventoryData.maxReserveAmmo;
        }
    }

    public void AddAmmo(int weaponIndex, int amount)
    {
        if (!IsValidWeaponIndex(weaponIndex))
            return;

        reserveAmmoByWeapon[weaponIndex] += amount;

        reserveAmmoByWeapon[weaponIndex] =
            Mathf.Min(reserveAmmoByWeapon[weaponIndex], inventoryData.maxReserveAmmo);

        if (pickupSound != null && audioSource != null)
            audioSource.PlayOneShot(pickupSound);
    }

    public int GetAmmo(int weaponIndex)
    {
        if (!IsValidWeaponIndex(weaponIndex))
            return 0;

        return reserveAmmoByWeapon[weaponIndex];
    }

    public void UseAmmo(int weaponIndex, int amount)
    {
        if (!IsValidWeaponIndex(weaponIndex))
            return;

        reserveAmmoByWeapon[weaponIndex] -= amount;

        reserveAmmoByWeapon[weaponIndex] =
            Mathf.Max(reserveAmmoByWeapon[weaponIndex], 0);
    }

    private bool IsValidWeaponIndex(int index)
    {
        return reserveAmmoByWeapon != null &&
               index >= 0 &&
               index < reserveAmmoByWeapon.Length;
    }
}