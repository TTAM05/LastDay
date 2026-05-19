using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    // ── THÊM MỚI ──
    [Header("Sprint")]
    public float sprintSpeed = 10f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.1f;
    public float smoothTime = 0.03f;

    [Header("Jump & Gravity")]
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("References")]
    public Camera playerCamera;

    [Header("FootStep")]
    public AudioClip[] footstepSound;
    public AudioSource rightfootstep;
    public AudioSource leftfootstep;
    public float footstepInterval = 0.5f;
    private float NextfootstepTime;
    private bool isLeftFoot = true;

    private CharacterController controller;
    private PlayerInputActions input;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation;
    private float yRotation;

    public float wantedCameraXRotation;
    public float wantedYRotation;
    private Vector2 currentLook;
    private Vector2 lookVelocity;

    // ── THÊM MỚI ──
    private bool isSprinting;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = new PlayerInputActions();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // khởi tạo góc nhìn
        wantedYRotation = transform.eulerAngles.y;
        wantedCameraXRotation = xRotation;
    }

    void OnEnable()
    {
        input.Enable();

        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMoveCanceled;
        input.Player.Look.performed += OnLook;
        input.Player.Look.canceled += OnLookCanceled;
        input.Player.Jump.performed += OnJump;

        // ── THÊM MỚI ──
        input.Player.Sprint.performed += OnSprintStarted;
        input.Player.Sprint.canceled += OnSprintCanceled;
    }

    void OnDisable()
    {
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMoveCanceled;
        input.Player.Look.performed -= OnLook;
        input.Player.Look.canceled -= OnLookCanceled;
        input.Player.Jump.performed -= OnJump;

        // ── THÊM MỚI ──
        input.Player.Sprint.performed -= OnSprintStarted;
        input.Player.Sprint.canceled -= OnSprintCanceled;

        input.Disable();
    }

    void Update()
    {
        GroundCheck();
        Move();
        ApplyGravity();

        //HandleFootsteps
        if(isGrounded && controller.velocity.magnitude > 2f && NextfootstepTime <= Time.time)
        {
            if(Time.time >= NextfootstepTime)
            {
                PlayerFootstepSound();
                NextfootstepTime = Time.time + footstepInterval;
            }
        }
    }

    void LateUpdate()
    {
        Look();
    }

    // =========================================================
    // MOVE — có sprint
    // =========================================================
    void Move()
    {
        Vector3 move =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        // ── THÊM MỚI: chọn tốc độ ──
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        Vector3 finalMove = move * currentSpeed;
        finalMove.y = velocity.y;

        controller.Move(finalMove * Time.deltaTime);
    }

    void Look()
    {
        currentLook = Vector2.SmoothDamp(
            currentLook,
            lookInput,
            ref lookVelocity,
            smoothTime
        );

        float mouseX = currentLook.x * mouseSensitivity;
        float mouseY = currentLook.y * mouseSensitivity;

        // INPUT
        wantedYRotation += mouseX;

        wantedCameraXRotation += -mouseY;

        wantedCameraXRotation =
            Mathf.Clamp(wantedCameraXRotation, -90f, 90f);

        // APPLY
        xRotation = Mathf.Lerp(
            xRotation,
            wantedCameraXRotation,
            20f * Time.deltaTime
        );

        yRotation = Mathf.Lerp(
            yRotation,
            wantedYRotation,
            20f * Time.deltaTime
        );

        playerCamera.transform.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);

        transform.rotation =
            Quaternion.Euler(0f, yRotation, 0f);
    }

    void Jump()
    {
        if (isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position, groundDistance, groundMask
        );
    }

    void PlayerFootstepSound()
    {

        AudioClip clip = footstepSound[Random.Range(0, footstepSound.Length)];
        
        if(isLeftFoot)
        {
            leftfootstep.PlayOneShot(clip);
        }
        else
        {
            rightfootstep.PlayOneShot(clip);
        }

        isLeftFoot = !isLeftFoot;
    }

    void OnMove(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    void OnMoveCanceled(InputAction.CallbackContext ctx) => moveInput = Vector2.zero;
    void OnLook(InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();
    void OnLookCanceled(InputAction.CallbackContext ctx) => lookInput = Vector2.zero;
    void OnJump(InputAction.CallbackContext ctx) => Jump();

    // ── THÊM MỚI ──
    void OnSprintStarted(InputAction.CallbackContext ctx) => isSprinting = true;
    void OnSprintCanceled(InputAction.CallbackContext ctx) => isSprinting = false;

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}