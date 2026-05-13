using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpForce = 10f;

    private CharacterController controller;

    private PlayerInputActions inputActions;

    private Vector2 moveInput;

    private Vector3 velocity;

    private bool isSprinting;

    private void Awake()
    {
        // Lấy CharacterController trên player
        controller = GetComponent<CharacterController>();

        // Tạo Input Actions
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        // Bật input
        inputActions.Enable();

        // Move
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        // Sprint
        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;

        // Jump 
        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Jump.canceled += OnJump;

    }

    private void OnDisable()
    {
        // Tắt input
        inputActions.Disable();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        // Lấy hướng camera
        Vector3 forward = Camera.main.transform.forward;

        // Lấy hướng phải của camera
        Vector3 right = Camera.main.transform.right;

        // Bỏ trục Y để tránh chúi xuống đất
        forward.y = 0f;
        right.y = 0f;

        // Normalize để tốc độ ổn định
        forward.Normalize();
        right.Normalize();

        // Tạo hướng di chuyển theo camera
        Vector3 move =
            forward * moveInput.y +
            right * moveInput.x;

        // Chọn tốc độ
        float currentSpeed =
            isSprinting ? sprintSpeed : walkSpeed;

        // Di chuyển
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // Đọc input WASD
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        // true khi giữ Shift
        // false khi thả Shift
        isSprinting = context.ReadValueAsButton();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }
}