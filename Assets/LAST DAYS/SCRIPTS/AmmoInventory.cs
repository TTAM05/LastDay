using UnityEngine;

public class AmmoInventory : MonoBehaviour
{
    public int reserveAmmo = 0;

    public int maxReserveAmmo = 300;
    public AudioClip pickupSound;
    public AudioSource audioSource;

    public void AddAmmo(int amount)
    {
        reserveAmmo += amount;

        reserveAmmo = Mathf.Min(
            reserveAmmo,
            maxReserveAmmo
        );

        //sound effect
        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
    }
}