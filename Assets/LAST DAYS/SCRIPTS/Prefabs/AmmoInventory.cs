using UnityEngine;

public class AmmoInventory : MonoBehaviour
{
    public AudioClip pickupSound;
    public AudioSource audioSource;

    // Không cần set tay nữa — tự lấy từ WeaponManager
    [HideInInspector] public GunData[] gunData;
    [HideInInspector] public int[] reserveAmmo;

    void Start()
    {
        WeaponManager wm = GetComponentInChildren<WeaponManager>();

        if (wm == null)
        {
            Debug.LogError("[AmmoInventory] Không tìm thấy WeaponManager!");
            return;
        }

        gunData     = new GunData[wm.weapons.Length];
        reserveAmmo = new int[wm.weapons.Length];

        for (int i = 0; i < wm.weapons.Length; i++)
        {
            GunSystem gs = wm.weapons[i].GetComponent<GunSystem>();

            if (gs == null)
            {
                Debug.LogError($"[AmmoInventory] Weapon[{i}] không có GunSystem!");
                continue;
            }

            gunData[i]     = gs.gunData;
            reserveAmmo[i] = gs.gunData.maxReserveAmmo; // bắt đầu với đạn đầy
        }
    }

    // Nhặt đạn
    public void AddAmmo(int weaponIndex, int amount)
    {
        if (!IsValid(weaponIndex)) return;

        reserveAmmo[weaponIndex] += amount;
        reserveAmmo[weaponIndex]  = Mathf.Min(reserveAmmo[weaponIndex], gunData[weaponIndex].maxReserveAmmo);

        if (pickupSound != null && audioSource != null)
            audioSource.PlayOneShot(pickupSound);
    }

    public int GetAmmo(int weaponIndex)
    {
        if (!IsValid(weaponIndex)) return 0;
        return reserveAmmo[weaponIndex];
    }

    public void UseAmmo(int weaponIndex, int amount)
    {
        if (!IsValid(weaponIndex)) return;

        reserveAmmo[weaponIndex] -= amount;
        reserveAmmo[weaponIndex]  = Mathf.Max(reserveAmmo[weaponIndex], 0);
    }

    private bool IsValid(int index)
    {
        if (reserveAmmo == null || index < 0 || index >= reserveAmmo.Length)
        {
            Debug.LogError($"[AmmoInventory] weaponIndex không hợp lệ: {index}");
            return false;
        }
        return true;
    }
}