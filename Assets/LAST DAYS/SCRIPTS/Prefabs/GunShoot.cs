using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

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

    [Header("Blood")]
    public ParticleSystem bloodPrefab;

    public Transform muzzle;

    [Header("Gun Data")]
    public GunData gunData;
    public int currentAmmo;
    // private int reserveAmmo;
    private WeaponManager weaponManager;
    private int weaponIndex;
    private AmmoInventory inventory;
    [Header("Animation")]
    public AnimationClip reloadClip;

    [Header("Impact")]
    public GameObject impactPrefab;   // kéo Prefab Quad vào đây
    public float impactLifetime = 2f; // biến mất sau bao giây

    [Header("UI")]
    public TMP_Text ammoText;
    public TMP_Text reserveAmmoText;
    public TMP_Text reloadTimeText;
    public GameObject bulletUI;
    public GameObject crosshairUI;

    [Header("Crosshair")]
    public GameObject normalCrosshair;
    public GameObject hitCrosshair;

    public float hitCrosshairTime = 0.1f;

    private Coroutine hitRoutine;


    void Start()
    {
        if (gunAudio != null)
        {
            gunAudio.playOnAwake = false;
        }

        currentAmmo = gunData.maxAmmo;
        // // ví dụ spawn full ammo
        // reserveAmmo = gunData.maxReserveAmmo;

        inventory = GetComponentInParent<AmmoInventory>();
        weaponManager = GetComponentInParent<WeaponManager>();

        //tự tìm UI theo tên nếu chưa gán
        if (ammoText == null)
            ammoText = GameObject.Find("AmmoText").GetComponent<TMP_Text>();    

        if (reserveAmmoText == null)
            reserveAmmoText = GameObject.Find("ReserveAmmoText").GetComponent<TMP_Text>();    
       
        // tìm trước
        if (reloadTimeText == null)
        {
            TMP_Text[] all = Resources.FindObjectsOfTypeAll<TMP_Text>();
            foreach (TMP_Text t in all)
            {
                if (t.name == "ReloadTimeText")
                {
                    reloadTimeText = t;
                    break;
                }
            }
        }

        // ẩn sau
        if (reloadTimeText != null)
            reloadTimeText.gameObject.SetActive(false);

        EnsureUIReferences();
        if (enabled)
            SetUIActive(true);
    }

    void OnEnable()
    {
        EnsureUIReferences();
        SetUIActive(true);

        input.Enable();

        // nhấn chuột
        input.Player.Fire.performed += OnFirePressed;
        input.Player.Reload.performed += OnReloadPressed;

        // thả chuột
        input.Player.Fire.canceled += OnFireReleased;
    }

    void OnDisable()
    {
        input.Player.Fire.performed -= OnFirePressed;
        input.Player.Reload.performed -= OnReloadPressed;
        input.Player.Fire.canceled -= OnFireReleased;
        input.Disable();

        SetUIActive(false);
    }

    void EnsureUIReferences()
    {
        if (bulletUI == null)
        {
            GameObject obj = GameObject.Find("BulletUI");
            if (obj != null) bulletUI = obj;
        }

        if (crosshairUI == null)
        {
            GameObject obj = GameObject.Find("Crosshair");
            if (obj == null)
                obj = GameObject.Find("CrossHair");
            if (obj != null) crosshairUI = obj;
        }
    }

    void SetUIActive(bool active)
    {
        if (bulletUI != null)
            bulletUI.SetActive(active);

        if (crosshairUI != null)
            crosshairUI.SetActive(active);
        else
        {
            if (normalCrosshair != null)
                normalCrosshair.SetActive(active);
            if (!active && hitCrosshair != null)
                hitCrosshair.SetActive(false);
        }
    }

    // =========================================================
    // AWAKE
    // =========================================================
    void Awake()
    {
        input = new PlayerInputActions();
    }

    // =========================================================
    // UPDATE
    // =========================================================
    void Update()
    {
        if (isReloading) return;

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

        UpdateAmmoUI();
    }

    void LateUpdate()
    {
        if (weaponManager == null) return;

        weaponIndex = weaponManager.currentWeapon;
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

        if (currentAmmo <= 0)
            return;

            
        //hiện vêt đạn bắn trúng
        if (aimSystem != null)
        {
            // Ray ray = new Ray(
            //     aimSystem.FirePoint,
            //     aimSystem.FireDirection
            // );
            Ray ray = cam.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0f)
            );

            
            if (Physics.Raycast(ray, out RaycastHit hit, gunData.range))
            {
                bool hitenemy = false;

                 // hiện hit crosshair
                // DAMAGE
                if (hit.collider.CompareTag("EnemyHead"))
                {
                    
                    hitenemy = true;

                     // hiện hit crosshair
                    //hiện máu khi headshot
                    ParticleSystem blood = Instantiate(
                        bloodPrefab,
                        hit.point + hit.normal * 0.01f,
                        Quaternion.LookRotation(hit.normal)
                    );
                    blood.transform.SetParent(hit.collider.transform);
                    Destroy(blood, 2f);

                    EnemyHealth enemy =
                        hit.collider.GetComponentInParent<EnemyHealth>();

                    enemy.TakeDamage(gunData.damage * 3, true);

                }
                else if (hit.collider.CompareTag("EnemyBody"))
                {
                    hitenemy = true;

                     // hiện hit crosshair
                    //hiện máu khi bắn trúng body
                    ParticleSystem blood = Instantiate(
                        bloodPrefab,
                        hit.point + hit.normal * 0.01f,
                        Quaternion.LookRotation(hit.normal)
                    );
                    blood.transform.SetParent(hit.collider.transform);
                    Destroy(blood, 2f);


                    EnemyHealth enemy =
                        hit.collider.GetComponentInParent<EnemyHealth>();

                    enemy.TakeDamage(gunData.damage, false);
                   
                }
             
                // IMPACT CHỈ HIỆN KHI KHÔNG PHẢI ENEMY
                bool isEnemy =
                hit.collider.CompareTag("Enemy") ||
                hit.collider.CompareTag("EnemyBody") ||
                hit.collider.CompareTag("EnemyHead");

                if (!isEnemy)
                {
                    GameObject impact = Instantiate(
                        impactPrefab,
                        hit.point + hit.normal * 0.01f,
                        Quaternion.LookRotation(hit.normal)
                    );

                    Destroy(impact, impactLifetime);
                }

                if (hitenemy && !aimSystem.isAiming)
                {
                    if (hitRoutine != null)
                        StopCoroutine(hitRoutine);

                    hitRoutine = StartCoroutine(ShowHitCrosshair());
                }
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

        //Gán damage cho viên đạn ngay khi bắn
                 Bullet bulletScript = bullet.GetComponent<Bullet>();
                 if (bulletScript != null)                 {
                     bulletScript.damage = gunData.damage;
                 }
    }

    IEnumerator ShowHitCrosshair()
    {
        normalCrosshair.SetActive(false);
        hitCrosshair.SetActive(true);

        yield return new WaitForSeconds(hitCrosshairTime);

        hitCrosshair.SetActive(false);
        normalCrosshair.SetActive(true);
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
        if (inventory.GetAmmo(weaponIndex) <= 0)
            yield break;

        isReloading = true;

          // ✅ Hiện reloadTimeText
        if (reloadTimeText != null)
            reloadTimeText.gameObject.SetActive(true);

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

         // ✅ Đếm ngược thời gian reload
        float timer = gunData.reloadTime;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (reloadTimeText != null)
                reloadTimeText.text = $"Reloading... {timer:F1}s";
            yield return null;
        }

        // =================================================

        // // chờ đúng reloadTime
        // yield return new WaitForSeconds(gunData.reloadTime);
        
        isReloading = false;

        // số đạn cần nạp
        int needAmmo = gunData.maxAmmo - currentAmmo;
        int reserve = inventory.GetAmmo(weaponIndex);

        // số đạn thực sự có thể nạp
        int ammoToLoad = Mathf.Min(needAmmo, reserve);

        currentAmmo += ammoToLoad;

        inventory.UseAmmo(weaponIndex, ammoToLoad);

        Debug.Log("Reserve ammo: " +  inventory.GetAmmo(weaponIndex));

        // ✅ Ẩn reloadTimeText khi xong
        if (reloadTimeText != null)
        reloadTimeText.gameObject.SetActive(false);

        
    }

    public void PlayReloadSound()
    {
        if (gunAudio != null && gunData.reloadclip != null)
        {
            gunAudio.PlayOneShot(gunData.reloadclip);
        }
    }

    // public void AddAmmo(int amount)
    // {
    //     reserveAmmo += amount;

    //     reserveAmmo = Mathf.Min(
    //         reserveAmmo,
    //         gunData.maxReserveAmmo
    //     );

    //     Debug.Log("Reserve Ammo: " + reserveAmmo);
    // }

    public void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo} / {gunData.maxAmmo}";

        if (reserveAmmoText != null)
            reserveAmmoText.text = inventory.GetAmmo(weaponIndex).ToString();

    }
}