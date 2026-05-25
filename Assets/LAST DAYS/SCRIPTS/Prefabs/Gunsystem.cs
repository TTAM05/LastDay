// using System.Collections;
// using UnityEngine;
// using UnityEngine.InputSystem;

// /// <summary>
// /// Hệ thống súng: bắn thường / auto, aim, reload, muzzle flash, vết lõm, máu địch.
// /// Gắn script này vào GameObject súng của Player.
// /// </summary>
// public class GunSysstem : MonoBehaviour
// {
//     // =========================================================
//     // REFERENCES
//     // =========================================================
//     [Header("References")]
//     public Camera           cam;           // Camera FPS
//     public AimSystem        aimSystem;     // Tính hướng bắn (hip / ADS)
//     public Animator         animator;      // Animator của súng
//     public GunRecoil        gunRecoil;     // Script giật súng
//     public AudioSource      gunAudio;      // AudioSource trên súng
//     public AmmoInventory    inventory;     // Đạn dự trữ của Player

//     [Header("Transform")]
//     public Transform muzzle;              // Đầu nòng súng

//     // =========================================================
//     // EFFECTS
//     // =========================================================
//     [Header("Effects")]
//     public ParticleSystem   muzzleFlash;  // Hiệu ứng muzzle flash
//     public ParticleSystem   bloodPrefab;  // Hiệu ứng máu (enemy)
//     public GameObject       impactPrefab; // Vết lõm đạn (surface)
//     public float            impactLifetime = 3f;

//     // =========================================================
//     // GUN DATA
//     // =========================================================
//     [Header("Gun Data")]
//     public GunData gunData;

//     // =========================================================
//     // ANIMATION
//     // =========================================================
//     [Header("Animation")]
//     public AnimationClip reloadClip;      // Clip reload để tính tốc độ

//     // =========================================================
//     // RUNTIME STATE
//     // =========================================================
//     [HideInInspector] public int currentAmmo;

//     private bool  isFiring;
//     private bool  isReloading;
//     private float nextFireTime;

//     // Input
//     private PlayerInputActions input;

//     // =========================================================
//     // UNITY MESSAGES
//     // =========================================================
//     void Awake()
//     {
//         input = new PlayerInputActions();
//     }

//     void Start()
//     {
//         currentAmmo = gunData.maxAmmo;

//         if (gunAudio != null)
//             gunAudio.playOnAwake = false;
//     }

//     void OnEnable()
//     {
//         input.Enable();
//         input.Player.Fire.performed   += OnFirePressed;
//         input.Player.Fire.canceled    += OnFireReleased;
//         input.Player.Reload.performed += OnReloadPressed;
//     }

//     void OnDisable()
//     {
//         input.Player.Fire.performed   -= OnFirePressed;
//         input.Player.Fire.canceled    -= OnFireReleased;
//         input.Player.Reload.performed -= OnReloadPressed;
//         input.Disable();
//     }

//     void Update()
//     {
//         if (isReloading) return;

//         // Auto fire: giữ chuột
//         if (gunData.isAutomatic && isFiring && Time.time >= nextFireTime)
//         {
//             if (currentAmmo > 0)
//                 Shoot();
//             else
//                 StartCoroutine(AutoReloadIfEmpty());

//             nextFireTime = Time.time + gunData.fireRate;
//         }
//     }

//     // =========================================================
//     // INPUT CALLBACKS
//     // =========================================================
//     void OnFirePressed(InputAction.CallbackContext ctx)
//     {
//         if (isReloading) return;

//         isFiring = true;

//         // Single fire: bắn ngay khi nhấn
//         if (!gunData.isAutomatic)
//         {
//             if (currentAmmo > 0 && Time.time >= nextFireTime)
//             {
//                 Shoot();
//                 nextFireTime = Time.time + gunData.fireRate;
//             }
//             else if (currentAmmo <= 0)
//             {
//                 StartCoroutine(Reload());
//             }
//         }
//     }

//     void OnFireReleased(InputAction.CallbackContext ctx)
//     {
//         isFiring = false;

//         if (animator != null)
//             animator.SetBool("Auto", false);
//     }

//     void OnReloadPressed(InputAction.CallbackContext ctx)
//     {
//         if (!isReloading && currentAmmo < gunData.maxAmmo)
//             StartCoroutine(Reload());
//     }

//     // =========================================================
//     // SHOOT
//     // =========================================================
//     void Shoot()
//     {
//         currentAmmo--;

//         // --- Hướng bắn ---
//         Vector3 origin;
//         Vector3 direction;

//         if (aimSystem != null)
//         {
//             // AimSystem tính sẵn cho cả hip-fire và ADS
//             origin    = aimSystem.FirePoint;
//             direction = aimSystem.FireDirection;
//         }
//         else
//         {
//             // Fallback: bắn thẳng từ tâm camera
//             Ray camRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
//             Vector3 target = Physics.Raycast(camRay, out RaycastHit fallbackHit, gunData.range)
//                 ? fallbackHit.point
//                 : camRay.GetPoint(gunData.range);

//             origin    = muzzle.position;
//             direction = (target - muzzle.position).normalized;
//         }

//         // --- Raycast ---
//         if (Physics.Raycast(origin, direction, out RaycastHit hit, gunData.range))
//         {
//             ProcessHit(hit);
//         }

//         // --- Hiệu ứng ---
//         PlayMuzzleFlash();
//         PlayFireSound();
//         PlayFireAnimation();

//         if (gunRecoil != null)
//             gunRecoil.Fire(gunData);
//     }

//     // =========================================================
//     // XỬ LÝ KHI TRÚNG ĐẠN
//     // =========================================================
//     void ProcessHit(RaycastHit hit)
//     {
//         bool isEnemyHead = hit.collider.CompareTag("EnemyHead");
//         bool isEnemyBody = hit.collider.CompareTag("EnemyBody");
//         bool isEnemy     = isEnemyHead || isEnemyBody ||
//                            hit.collider.CompareTag("Enemy");

//         // --- Damage ---
//         if (isEnemyHead)
//         {
//             SpawnBlood(hit);
//             EnemyHealth eh = hit.collider.GetComponentInParent<EnemyHealth>();
//             if (eh != null) eh.TakeDamage(gunData.damage * 2, true);
//         }
//         else if (isEnemyBody)
//         {
//             SpawnBlood(hit);
//             EnemyHealth eh = hit.collider.GetComponentInParent<EnemyHealth>();
//             if (eh != null) eh.TakeDamage(gunData.damage, false);
//         }
//         else if (hit.collider.CompareTag("Enemy"))
//         {
//             EnemyHealth eh = hit.collider.GetComponent<EnemyHealth>();
//             if (eh != null) eh.TakeDamage(gunData.damage, false);
//         }

//         // --- Vết lõm: chỉ spawn trên surface, không phải enemy ---
//         if (!isEnemy)
//             SpawnImpact(hit);
//     }

//     // =========================================================
//     // SPAWN HELPERS
//     // =========================================================
//     void SpawnBlood(RaycastHit hit)
//     {
//         if (bloodPrefab == null) return;

//         ParticleSystem blood = Instantiate(
//             bloodPrefab,
//             hit.point + hit.normal * 0.01f,
//             Quaternion.LookRotation(hit.normal)
//         );
//         blood.transform.SetParent(hit.collider.transform);
//         Destroy(blood.gameObject, 2f);
//     }

//     void SpawnImpact(RaycastHit hit)
//     {
//         if (impactPrefab == null) return;

//         GameObject impact = Instantiate(
//             impactPrefab,
//             hit.point + hit.normal * 0.01f,
//             Quaternion.LookRotation(hit.normal)
//         );
//         impact.transform.SetParent(hit.collider.transform);
//         Destroy(impact, impactLifetime);
//     }

//     // =========================================================
//     // EFFECT HELPERS
//     // =========================================================
//     void PlayMuzzleFlash()
//     {
//         if (muzzleFlash != null)
//             muzzleFlash.Play();
//     }

//     void PlayFireSound()
//     {
//         if (gunAudio == null) return;

//         AudioClip clip = gunData.isAutomatic
//             ? gunData.autoShotClip
//             : gunData.singleShotClip;

//         if (clip != null)
//             gunAudio.PlayOneShot(clip);
//     }

//     void PlayFireAnimation()
//     {
//         if (animator == null) return;

//         if (gunData.isAutomatic)
//             animator.SetBool("Auto", true);
//         else
//             animator.SetTrigger("Fire");
//     }

//     // =========================================================
//     // RELOAD
//     // =========================================================
//     IEnumerator Reload()
//     {
//         if (isReloading)              yield break;
//         if (currentAmmo >= gunData.maxAmmo) yield break;
//         if (inventory == null || inventory.reserveAmmo <= 0) yield break;

//         isReloading = true;

//         // Tính tốc độ animation khớp với reloadTime
//         if (animator != null && reloadClip != null)
//         {
//             float speed = reloadClip.length / gunData.reloadTime;
//             animator.SetFloat("ReloadSpeed", speed);
//             animator.SetTrigger("Reload");
//         }

//         // Âm thanh reload
//         if (gunAudio != null && gunData.reloadclip != null)
//             gunAudio.PlayOneShot(gunData.reloadclip);

//         yield return new WaitForSeconds(gunData.reloadTime);

//         // Nạp đạn vào băng
//         int need      = gunData.maxAmmo - currentAmmo;
//         int available = Mathf.Min(need, inventory.reserveAmmo);

//         currentAmmo          += available;
//         inventory.reserveAmmo -= available;

//         isReloading = false;

//         Debug.Log($"[GunSystem] Reload complete | Mag: {currentAmmo}/{gunData.maxAmmo} | Reserve: {inventory.reserveAmmo}");
//     }

//     // Tự động reload khi hết đạn trong auto mode
//     IEnumerator AutoReloadIfEmpty()
//     {
//         isFiring = false;

//         if (animator != null)
//             animator.SetBool("Auto", false);

//         yield return Reload();
//     }

//     // =========================================================
//     // PUBLIC UTILS
//     // =========================================================

//     /// <summary>Thêm đạn dự trữ từ pickup.</summary>
//     public void AddReserveAmmo(int amount)
//     {
//         if (inventory == null) return;
//         inventory.reserveAmmo = Mathf.Min(
//             inventory.reserveAmmo + amount,
//             gunData.maxReserveAmmo
//         );
//     }

//     /// <summary>Đọc HUD: đạn trong băng / dự trữ.</summary>
//     public (int mag, int reserve) GetAmmoState() =>
//         (currentAmmo, inventory != null ? inventory.reserveAmmo : 0);
// }