using UnityEngine;

public class AmmoInventory : MonoBehaviour
{
    public AudioClip pickupSound;
    public AudioSource audioSource;

    [Header("GunData")]
    public GunData[] gunData;

    [Header("Ammo per weapon")]
    public int[] reserveAmmo;

    void Start()
    {
        reserveAmmo = new int[gunData.Length];
    }

    public void AddAmmo(int weaponIndex, int amount)
    {
        reserveAmmo[weaponIndex] += amount;

        reserveAmmo[weaponIndex] = Mathf.Min(
            reserveAmmo[weaponIndex],
            gunData[weaponIndex].maxReserveAmmo
        );

        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
    }

    public int GetAmmo(int weaponIndex)
    {
        return reserveAmmo[weaponIndex];
    }

    public void UseAmmo(int weaponIndex, int amount)
    {
        reserveAmmo[weaponIndex] -= amount;
        reserveAmmo[weaponIndex] = Mathf.Max(reserveAmmo[weaponIndex], 0);
    }
}