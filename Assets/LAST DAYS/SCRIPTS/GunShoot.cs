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

    [Header("Gun Settings")]

    // tốc độ bắn
    public float fireRate = 0.1f;

    // true = auto
    // false = single
    public bool isAutomatic = true;

    // input system
    private PlayerInputActions input;

    // đang giữ chuột
    private bool isFiring;

    // cooldown bắn
    private float nextFireTime;
    public AudioSource gunAudio;
    public AudioClip gunSingleClip;
    public AudioClip gunAutoClip;

    [Header("Bullet")]

    public GameObject bulletPrefab;

    public Transform muzzle;

    public float bulletSpeed = 100f;
    void Start()
    {
        if (gunAudio != null)
        {
            gunAudio.playOnAwake = false;
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
    // ENABLE
    // =========================================================
    void OnEnable()
    {
        input.Enable();

        // nhấn chuột
        input.Player.Fire.performed += OnFirePressed;

        // thả chuột
        input.Player.Fire.canceled += OnFireReleased;
    }

    // =========================================================
    // DISABLE
    // =========================================================
    void OnDisable()
    {
        input.Player.Fire.performed -= OnFirePressed;

        input.Player.Fire.canceled -= OnFireReleased;

        input.Disable();
    }

    // =========================================================
    // UPDATE
    // =========================================================
    void Update()
    {
        // bắn liên thanh
        if (isAutomatic && isFiring)
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();

                nextFireTime =
                    Time.time + fireRate;
            }
        }
    }

    // =========================================================
    // NHẤN CHUỘT
    // =========================================================
    void OnFirePressed(InputAction.CallbackContext ctx)
    {
        isFiring = true;

        // single fire
        if (!isAutomatic)
        {
            Shoot();
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
        // muzzle flash
        if (muzzleFlash != null) muzzleFlash.Play();

        // âm thanh
        if (gunAudio != null)
            gunAudio.PlayOneShot(isAutomatic ? gunAutoClip : gunSingleClip);

        // animation
        if (animator != null)
        {
            if (isAutomatic) animator.SetBool("Auto", true);
            else             animator.SetTrigger("Fire");
        }

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
        rb.linearVelocity = fireDirection * bulletSpeed;
    }
}