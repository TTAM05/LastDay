using UnityEngine;
using UnityEngine.InputSystem;

public class AimSystem : MonoBehaviour
{
    [Header("References")]

    public Camera cam;

    public Animator animator;

    [Header("FOV")]

    public float normalFOV = 60f;

    public float aimFOV = 40f;

    public float smoothSpeed = 10f;

    private PlayerInputActions input;

    private bool isAiming;

    // =====================================================
    // AWAKE
    // =====================================================
    void Awake()
    {
        input = new PlayerInputActions();
    }

    // =====================================================
    // ENABLE
    // =====================================================
    void OnEnable()
    {
        input.Enable();

        input.Player.Aim.performed += StartAim;

        input.Player.Aim.canceled += StopAim;
    }

    // =====================================================
    // DISABLE
    // =====================================================
    void OnDisable()
    {
        input.Player.Aim.performed -= StartAim;

        input.Player.Aim.canceled -= StopAim;

        input.Disable();
    }

    // =====================================================
    // UPDATE
    // =====================================================
    void Update()
    {
        // UpdateFOV();
    }

    // =====================================================
    // START AIM
    // =====================================================
    void StartAim(InputAction.CallbackContext ctx)
    {
        isAiming = true;

        if (animator != null)
        {
            animator.SetBool("Aim", true);
        }
    }

    // =====================================================
    // STOP AIM
    // =====================================================
    void StopAim(InputAction.CallbackContext ctx)
    {
        isAiming = false;

        if (animator != null)
        {
            animator.SetBool("Aim", false);
        }
    }

    // // =====================================================
    // // UPDATE FOV
    // // =====================================================
    // void UpdateFOV()
    // {
    //     float targetFOV =
    //         isAiming ? aimFOV : normalFOV;

    //     cam.fieldOfView = Mathf.Lerp(
    //         cam.fieldOfView,
    //         targetFOV,
    //         Time.deltaTime * smoothSpeed
    //     );
    // }
}