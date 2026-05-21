using UnityEngine;

public class AmmoInventory : MonoBehaviour
{
    public AudioClip pickupSound;
    public AudioSource audioSource;

    // Không cần set tay nữa — tự lấy từ WeaponManager
    [HideInInspector] public GunData[] gunData;
    public InventoryData inventoryData;
    [HideInInspector] public int reserveAmmo;

    void Start()
    {
        WeaponManager wm = GetComponentInChildren<WeaponManager>();

        if (wm == null)
        {
            Debug.LogError("[AmmoInventory] Không tìm thấy WeaponManager!");
            return;
        }

        gunData  = new GunData[wm.weapons.Length]; 

        for (int i = 0; i < wm.weapons.Length; i++)
        {
            GunSystem gs = wm.weapons[i].GetComponent<GunSystem>();

            if (gs == null)
            {
                Debug.LogError($"[AmmoInventory] Weapon[{i}] không có GunSystem!");
                continue;
            }

            gunData[i]     = gs.gunData;
            reserveAmmo = inventoryData.maxReserveAmmo; // bắt đầu với đạn đầy
        }
    }

    // Nhặt đạn
    public void AddAmmo(int weaponIndex, int amount)
    {
       

        reserveAmmo += amount;
        reserveAmmo  = Mathf.Min(reserveAmmo, inventoryData.maxReserveAmmo);

        if (pickupSound != null && audioSource != null)
            audioSource.PlayOneShot(pickupSound);
    }

    public int GetAmmo(int weaponIndex)
    {
        
        return reserveAmmo;
    }

    public void UseAmmo(int weaponIndex, int amount)
    {
        
        reserveAmmo -= amount;
        reserveAmmo  = Mathf.Max(reserveAmmo, 0);
    }

  
}