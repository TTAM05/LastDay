using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunSystem : MonoBehaviour
{
    [Header("References")]

    // camera FPS
    public Camera cam;
    public AimSystem aimSystem;

    // animator súng
    public Animator animator;

    // muzzle flash
    public ParticleSystem muzzleFlash;

   
    // input system
    private PlayerInputActions input;

    // đang giữ chuột
    private bool isFiring;

    //đang thay đạn
    private bool isReloading;

    // cooldown bắn
    private float nextFireTime;
    public AudioSource gunAudio;
    public GunRecoil gunRecoil;

    [Header("Bullet")]

    public GameObject bulletPrefab;

    public Transform muzzle;

    [Header("Gun Data")]
    public GunData gunData;
    public int currentAmmo;
    private int reserveAmmo;
    [Header("Animation")]
    public AnimationClip reloadClip;

    [Header("Impact")]
    public GameObject impactPrefab;   // kéo Prefab Quad vào đây
    public float impactLifetime = 2f; // biến mất sau bao giây


    void Start()
    {
        if (gunAudio != null)
        {
            gunAudio.playOnAwake = false;
        }

        currentAmmo = gunData.maxAmmo;
        // ví dụ spawn full ammo
        reserveAmmo = gunData.maxReserveAmmo;
    }


    // =========================================================
    // AWAKE
    // =========================================================
    void Awake()
    {
        input = new PlayerInputActions();
    }

    // =========================================================
    // ENABLE
    // =========================================================
    void OnEnable()
    {
        input.Enable();

        // nhấn chuột
        input.Player.Fire.performed += OnFirePressed;
        input.Player.Reload.performed += OnReloadPressed;

        // thả chuột
        input.Player.Fire.canceled += OnFireReleased;


    }

    // =========================================================
    // DISABLE
    // =========================================================
    void OnDisable()
    {
        input.Player.Fire.performed -= OnFirePressed;
        input.Player.Reload.performed -= OnReloadPressed;
        input.Player.Fire.canceled -= OnFireReleased;
        input.Disable();
    }

    // =========================================================
    // UPDATE
    // =========================================================
    void Update()
    {
        if(isReloading)
        {
            return; // không làm gì khi đang nạp đạn
        }
        
        // bắn liên thanh
        if (gunData.isAutomatic && isFiring)
        {
            if (Time.time >= nextFireTime)
            {
                if (currentAmmo > 0)
                {
                    Shoot();
                }

                nextFireTime = Time.time + gunData.fireRate;
            }
        }

        //đạn chỉ đc giới hạn trong range của gundata

    }

    // =========================================================
    // NHẤN CHUỘT
    // =========================================================
    void OnFirePressed(InputAction.CallbackContext ctx)
    {
        if (isReloading) return;

        isFiring = true;

        // single fire
        if (!gunData.isAutomatic && currentAmmo > 0)
        {
            Shoot();
        }
    }

    void OnReloadPressed(InputAction.CallbackContext ctx)
    {
        if (currentAmmo < gunData.maxAmmo && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    // =========================================================
    // THẢ CHUỘT
    // =========================================================
    void OnFireReleased(InputAction.CallbackContext ctx)
    {
        isFiring = false;

        // tắt anim auto
        if (animator != null)
        {
            animator.SetBool("Auto", false);
        }
    }



    // =========================================================
    // SHOOT
    // =========================================================
    void Shoot()
    {
        //hiện vêt đạn bắn trúng
        if (aimSystem != null)
        {
            Ray ray = new Ray(aimSystem.FirePoint, aimSystem.FireDirection);
            if (Physics.Raycast(ray, out RaycastHit hit, gunData.range))
            {
                // tạo impact tại điểm trúng
                GameObject impact = Instantiate(
                    impactPrefab,
                    hit.point + hit.normal * 0.01f, // đẩy ra một chút để tránh z-fighting
                    Quaternion.LookRotation(hit.normal) // quay theo mặt phẳng va chạm
                );

                // hủy impact sau một thời gian
                Destroy(impact, impactLifetime);
            }
        }

        //- arrmor
        currentAmmo--;
        // muzzle flash
        if (muzzleFlash != null) muzzleFlash.Play();

        // âm thanh
        if (gunAudio != null)
            gunAudio.PlayOneShot(gunData.isAutomatic ? gunData.autoShotClip : gunData.singleShotClip);

        // animation
        if (animator != null)
        {
            if (gunData.isAutomatic) 
            {
                animator.SetBool("Auto", true);
            }
            else
            {
                animator.SetTrigger("Fire");
            }
        }
        // recoil
        gunRecoil.Fire(gunData);

        // =====================================================
        // HƯỚNG BẮN — lấy từ AimSystem
        // =====================================================
        Vector3 fireOrigin;
        Vector3 fireDirection;

        if (aimSystem != null)
        {
            // AimSystem đã tính sẵn cho cả hip và ADS
            fireOrigin    = aimSystem.FirePoint;
            fireDirection = aimSystem.FireDirection;
        }
        else
        {
            // fallback nếu không có AimSystem
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 target = Physics.Raycast(ray, out RaycastHit hit, 1000f)
                ? hit.point
                : ray.GetPoint(1000f);

            fireOrigin    = muzzle.position;
            fireDirection = (target - muzzle.position).normalized;
        }
        // =====================================================

        // tạo đạn
        GameObject bullet = Instantiate(
            bulletPrefab,
            fireOrigin,
            Quaternion.LookRotation(fireDirection)
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = fireDirection * gunData.bulletSpeed;
    }

    // =========================================================
    //Nạp đạn
    IEnumerator Reload()
    {
        if (isReloading)
            yield break;

        // đầy băng
        if (currentAmmo >= gunData.maxAmmo)
            yield break;

        // hết đạn dự trữ
        if (reserveAmmo <= 0)
            yield break;

        isReloading = true;

        // =================================================
        // TÍNH TỐC ĐỘ ANIMATION
        // =================================================

        float animLength = reloadClip.length;

        // tốc độ anim
        float reloadSpeed = animLength / gunData.reloadTime;

        // set speed vào animator
        animator.SetFloat("ReloadSpeed", reloadSpeed);

        // play anim
        animator.SetTrigger("Reload");

        // =================================================

        // chờ đúng reloadTime
        yield return new WaitForSeconds(gunData.reloadTime);

        // số đạn cần nạp
        int needAmmo = gunData.maxAmmo - currentAmmo;

        // số đạn thực sự có thể nạp
        int ammoToLoad = Mathf.Min(needAmmo, reserveAmmo);

        currentAmmo += ammoToLoad;

        reserveAmmo -= ammoToLoad;

        isReloading = false;
    }

    public void PlayReloadSound()
    {
        if (gunAudio != null && gunData.reloadclip != null)
        {
            gunAudio.PlayOneShot(gunData.reloadclip);
        }
    }
}