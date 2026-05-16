using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AimSystem : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public Animator animator;
    public Transform muzzle;           // đầu nòng súng
    public Image crosshairImage;       // UI crosshair

    [Header("Settings")]
    public float range = 1000f;

    private PlayerInputActions input;
    private bool isAiming;

    // điểm bắn — các script khác lấy từ đây
    public Vector3 FirePoint { get; private set; }
    public Vector3 FireDirection { get; private set; }

    // =====================================================
    // AWAKE
    // =====================================================
    void Awake()
    {
        input = new PlayerInputActions();
    }

    // =====================================================
    // ENABLE / DISABLE
    // =====================================================
    void OnEnable()
    {
        input.Enable();
        input.Player.Aim.performed += StartAim;
        input.Player.Aim.canceled  += StopAim;
    }

    void OnDisable()
    {
        input.Player.Aim.performed -= StartAim;
        input.Player.Aim.canceled  -= StopAim;
        input.Disable();
    }

    // =====================================================
    // UPDATE
    // =====================================================
    void Update()
    {
        UpdateFireData();
    }

    // =====================================================
    // FIRE DATA
    // tính điểm bắn theo chế độ hip / aim
    // =====================================================
    void UpdateFireData()
    {
        if (isAiming)
        {
            // ADS — bắn theo hướng nòng súng
            FirePoint     = muzzle.position;
            FireDirection = muzzle.forward;
        }
        else
        {
            // Hip fire — bắn theo hướng camera
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, range)
                ? hit.point
                : ray.GetPoint(range);

            FirePoint     = muzzle.position;
            FireDirection = (targetPoint - muzzle.position).normalized;
        }
        // ← thêm dòng này để thấy hướng bắn trong Scene view
        Debug.DrawRay(FirePoint, FireDirection * 10f, Color.red);
    }

    // =====================================================
    // START AIM
    // =====================================================
    void StartAim(InputAction.CallbackContext ctx)
    {
        isAiming = true;

        // ẩn crosshair khi ADS
        if (crosshairImage != null)
            crosshairImage.enabled = false;

        if (animator != null)
            animator.SetBool("Aim", true);
    }

    // =====================================================
    // STOP AIM
    // =====================================================
    void StopAim(InputAction.CallbackContext ctx)
    {
        isAiming = false;

        // hiện crosshair khi thôi ADS
        if (crosshairImage != null)
            crosshairImage.enabled = true;

        if (animator != null)
            animator.SetBool("Aim", false);
    }


}