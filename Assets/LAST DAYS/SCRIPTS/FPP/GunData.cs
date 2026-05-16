using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "Gun/Gun Data")]
public class GunData : ScriptableObject
{
    [Header("Info")]
    public string gunName;

    [Header("Damage")]
    public float damage = 20f;

    [Header("Fire")]
    public float fireRate = 0.1f;

    public bool isAutomatic = true;

    [Header("Range")]
    public float range = 100f;

    [Header("Reload")]
    // public float reloadSpeed = 2f;
    public float reloadTime = 2f;

    [Header("Stability")]
    [Range(0, 100)]
    public float stability = 80f;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Ammo")]
    public int maxAmmo = 30;
    public int maxReserveAmmo = 200;  // đạn dự trữ tối đa

    [Header("Bullet")]
    public float bulletSpeed = 100f;

    [Header("Audio")]
    public AudioClip singleShotClip;

    public AudioClip autoShotClip;
    public AudioClip reloadclip;
}