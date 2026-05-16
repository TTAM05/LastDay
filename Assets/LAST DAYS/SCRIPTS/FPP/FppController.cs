using UnityEngine;
using UnityEngine.InputSystem;



[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.1f;

    // thời gian smooth camera
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

    // component CharacterController
    private CharacterController controller;

    // file input actions
    private PlayerInputActions input;

    // input di chuyển
    private Vector2 moveInput;

    // input nhìn chuột
    private Vector2 lookInput;

    // velocity gravity
    private Vector3 velocity;

    // grounded
    private bool isGrounded;

    // rotation camera dọc
    private float xRotation;

    // smooth mouse
    private Vector2 currentLook;
    private Vector2 lookVelocity;

    // =========================================================
    // AWAKE
    // =========================================================
    void Awake()
    {
        // lấy CharacterController
        controller = GetComponent<CharacterController>();

        // tạo input system
        input = new PlayerInputActions();
    }

    // =========================================================
    // START
    // =========================================================
    void Start()
    {
        // khóa chuột vào giữa màn hình
        Cursor.lockState = CursorLockMode.Locked;

        // ẩn chuột
        Cursor.visible = false;
    }

    // =========================================================
    // ENABLE INPUT
    // =========================================================
    void OnEnable()
    {
        input.Enable();

        // MOVE
        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMoveCanceled;

        // LOOK
        input.Player.Look.performed += OnLook;
        input.Player.Look.canceled += OnLookCanceled;

        // JUMP
        input.Player.Jump.performed += OnJump;
    }

    // =========================================================
    // DISABLE INPUT
    // =========================================================
    void OnDisable()
    {
        // MOVE
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMoveCanceled;

        // LOOK
        input.Player.Look.performed -= OnLook;
        input.Player.Look.canceled -= OnLookCanceled;

        // JUMP
        input.Player.Jump.performed -= OnJump;

        input.Disable();
    }

    // =========================================================
    // UPDATE
    // =========================================================
    void Update()
    {
        GroundCheck();

        Move();

        ApplyGravity();
    }

    // =========================================================
    // LATE UPDATE
    // camera chạy sau movement để mượt hơn
    // =========================================================
    void LateUpdate()
    {
        Look();
    }

    // =========================================================
    // MOVE
    // =========================================================
    void Move()
    {
        // hướng di chuyển theo player
        Vector3 move =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        // move ngang
        Vector3 finalMove =
            move * moveSpeed;

        // cộng gravity
        finalMove.y = velocity.y;

        // move player
        controller.Move(finalMove * Time.deltaTime);
    }

    // =========================================================
    // LOOK
    // =========================================================
    void Look()
    {
        // smooth chuột
        currentLook = Vector2.SmoothDamp(
            currentLook,
            lookInput,
            ref lookVelocity,
            smoothTime
        );

        // mouse X/Y
        float mouseX = currentLook.x * mouseSensitivity;
        float mouseY = currentLook.y * mouseSensitivity;

        // xoay camera lên xuống
        xRotation -= mouseY;

        // giới hạn góc nhìn
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // xoay camera dọc
        playerCamera.transform.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);

        // xoay player ngang
        transform.Rotate(Vector3.up * mouseX);
    }

    // =========================================================
    // JUMP
    // =========================================================
    void Jump()
    {
        // chỉ nhảy khi đứng đất
        if (isGrounded)
        {
            // công thức nhảy
            velocity.y =
                Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // =========================================================
    // GRAVITY
    // =========================================================
    void ApplyGravity()
    {
        // nếu đứng đất và đang rơi
        if (isGrounded && velocity.y < 0)
        {
            // giữ player dính đất
            velocity.y = -2f;
        }

        // gravity
        velocity.y += gravity * Time.deltaTime;
    }

    // =========================================================
    // GROUND CHECK
    // =========================================================
    void GroundCheck()
    {
        // check sphere dưới chân
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );
    }

    // =========================================================
    // INPUT EVENTS
    // =========================================================

    void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    void OnLookCanceled(InputAction.CallbackContext ctx)
    {
        lookInput = Vector2.zero;
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        Jump();
    }

    // =========================================================
    // GIZMOS
    // =========================================================
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundDistance
        );
    }
}