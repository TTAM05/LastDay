using UnityEngine;

public class AmmoInventory : MonoBehaviour
{
    public int reserveAmmo = 0;

    public int maxReserveAmmo = 300;

    public void AddAmmo(int amount)
    {
        reserveAmmo += amount;

        reserveAmmo = Mathf.Min(
            reserveAmmo,
            maxReserveAmmo
        );
    }
}