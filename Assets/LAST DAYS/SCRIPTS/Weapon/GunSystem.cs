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
    private WeaponManager weaponManager;
    private int weaponIndex;
    private AmmoInventory inventory;
    [Header("Animation")]
    public AnimationClip reloadClip;

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
    private Vector3 startLocalPosition;
    private Quaternion startLocalRotation;
    private Vector3 startLocalScale;


    void Awake()
    {
        input = new PlayerInputActions();

        startLocalPosition = transform.localPosition;
        startLocalRotation = transform.localRotation;
        startLocalScale = transform.localScale;
    }

    void Start()
    {
        if (gunAudio != null)
        {
            gunAudio.playOnAwake = false;
        }

        currentAmmo = gunData.maxAmmo;
   
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

        if (enabled)
            SetUIActive(true);
    }

    void OnEnable()
    {
        EnsureUIReferences();
        SetUIActive(true);

        isReloading = false;
        isFiring = false;

        input.Enable();

        input.Player.Fire.performed += OnFirePressed;
        input.Player.Reload.performed += OnReloadPressed;
        input.Player.Fire.canceled += OnFireReleased;
    }

    void OnDisable()
    {
        input.Player.Fire.performed -= OnFirePressed;
        input.Player.Reload.performed -= OnReloadPressed;
        input.Player.Fire.canceled -= OnFireReleased;

        input.Disable();

        CancelFire();
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

    public void SetUIActive(bool active)
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
    // UPDATE
    // =========================================================
    void Update()
    {   
        if (Time.timeScale == 0f) return;

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

    // =========================================================
    // NHẤN CHUỘT
    // =========================================================
    void OnFirePressed(InputAction.CallbackContext ctx)
    {   
        if(Time.timeScale == 0f) return;
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
        if(Time.timeScale == 0f) return;
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
        if (Time.timeScale == 0f) return;
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
        if (!CanShoot())
            return;

        ConsumeAmmo();

        PlayShootEffect();

        ApplyRecoil();

        HandleHit();

        UpdateAmmoUI();
    }

    public void SetWeaponIndex(int index)
    {
        weaponIndex = index;
    }

    bool CanShoot()
    {
        return Time.timeScale > 0f &&
            !isReloading &&
            currentAmmo > 0 &&
            gunData != null;
    }

    void ConsumeAmmo()
    {
        currentAmmo--;
    }

    void PlayShootEffect()
    {
        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (gunAudio != null)
        {
            AudioClip clip = gunData.isAutomatic
                ? gunData.autoShotClip
                : gunData.singleShotClip;

            gunAudio.PlayOneShot(clip);
        }

        if (animator != null)
        {
            if (gunData.isAutomatic)
                animator.SetBool("Auto", true);
            else
                animator.SetTrigger("Fire");
        }
    }

    void ApplyRecoil()
    {
        if (gunRecoil != null)
            gunRecoil.Fire(gunData);
    }

    void HandleHit()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (!Physics.Raycast(ray, out RaycastHit hit, gunData.range))
            return;

        bool isHeadshot =
            hit.collider.CompareTag("EnemyHead") ||
            hit.collider.CompareTag("MutantHead");

        bool isBody =
            hit.collider.CompareTag("EnemyBody") ||
            hit.collider.CompareTag("MutantBody");

        if (!isHeadshot && !isBody)
            return;

        float finalDamage = isHeadshot
            ? gunData.damage * 3f
            : gunData.damage;

        EnemyHealth enemy =
            hit.collider.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(finalDamage, isHeadshot);
            SpawnBlood(hit);
            ShowHitMarker();
            return;
        }

        MutantHealth mutant =
            hit.collider.GetComponentInParent<MutantHealth>();

        if (mutant != null)
        {
            mutant.TakeDamage(finalDamage, isHeadshot);
            SpawnBlood(hit);
            ShowHitMarker();
        }
    }

    void SpawnBlood(RaycastHit hit)
    {
        if (bloodPrefab == null)
            return;

        ParticleSystem blood = Instantiate(
            bloodPrefab,
            hit.point + hit.normal * 0.01f,
            Quaternion.LookRotation(hit.normal)
        );

        blood.transform.SetParent(hit.collider.transform);

        Destroy(blood.gameObject, 2f);
    }

    void ShowHitMarker()
    {
        if (aimSystem != null && aimSystem.isAiming)
            return;

        if (hitRoutine != null)
            StopCoroutine(hitRoutine);

        hitRoutine = StartCoroutine(ShowHitCrosshair());
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

        if (currentAmmo >= gunData.maxAmmo)
            yield break;

        if (inventory == null)
            yield break;

        if (inventory.GetAmmo(weaponIndex) <= 0)
            yield break;

        isReloading = true;
        isFiring = false;

        if (reloadTimeText != null)
            reloadTimeText.gameObject.SetActive(true);

        if (animator != null && reloadClip != null)
        {
            float reloadSpeed = reloadClip.length / gunData.reloadTime;
            animator.SetFloat("ReloadSpeed", reloadSpeed);
            animator.SetTrigger("Reload");
        }

        float timer = gunData.reloadTime;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            if (reloadTimeText != null)
                reloadTimeText.text = $"Reloading... {timer:F1}s";

            yield return null;
        }

        int needAmmo = gunData.maxAmmo - currentAmmo;
        int reserve = inventory.GetAmmo(weaponIndex);
        int ammoToLoad = Mathf.Min(needAmmo, reserve);

        currentAmmo += ammoToLoad;
        inventory.UseAmmo(weaponIndex, ammoToLoad);

        if (reloadTimeText != null)
            reloadTimeText.gameObject.SetActive(false);

        isReloading = false;

        UpdateAmmoUI();
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    

    public void CancelFire()
    {
        isFiring = false;

        if (animator != null)
            animator.SetBool("Auto", false);
    }

    public void PlayReloadSound()
    {
        if (gunAudio != null && gunData.reloadclip != null)
        {
            gunAudio.PlayOneShot(gunData.reloadclip);
        }
    }

    public void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo} / {gunData.maxAmmo}";

        if (reserveAmmoText != null)
            reserveAmmoText.text = inventory.GetAmmo(weaponIndex).ToString();

    }
}