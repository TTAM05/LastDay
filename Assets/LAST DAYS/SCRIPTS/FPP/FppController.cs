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

    private CharacterController controller;
    private PlayerInputActions input;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation;
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
            currentLook, lookInput,
            ref lookVelocity, smoothTime
        );

        float mouseX = currentLook.x * mouseSensitivity;
        float mouseY = currentLook.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
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