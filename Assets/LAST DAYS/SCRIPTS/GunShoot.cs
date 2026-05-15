using UnityEngine;
using UnityEngine.InputSystem;

public class GunSystem : MonoBehaviour
{
    [Header("References")]

    // camera FPS
    public Camera cam;

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
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // âm thanh bắn
        if (gunAudio != null)
        {
            if (isAutomatic)
            {
                gunAudio.PlayOneShot(gunAutoClip);
            }
            else
            {
                gunAudio.PlayOneShot(gunSingleClip);
            }
        }

        // animation
        if (animator != null)
        {
            // auto fire
            if (isAutomatic)
            {
                animator.SetBool("Auto", true);
            }

            // single fire
            else
            {
                animator.SetTrigger("Fire");
            }
        }

        // debug
        Debug.Log("Bang!");

        // ray từ giữa màn hình
        Ray ray = cam.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        Vector3 targetPoint;

        // nếu ray trúng vật
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000f);
        }

        // hướng bắn
        Vector3 shootDirection =
            (targetPoint - muzzle.position).normalized;

        // rotation đạn
        Quaternion bulletRotation =
            Quaternion.LookRotation(shootDirection);

        // tạo đạn
        GameObject bullet = Instantiate(
            bulletPrefab,
            muzzle.position,
            bulletRotation
        );

        // rigidbody
        Rigidbody rb =
            bullet.GetComponent<Rigidbody>();

        // velocity đạn
        rb.linearVelocity =
            shootDirection * bulletSpeed;
     }
}