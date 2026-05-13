using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform cameraTarget;

    [SerializeField] private float mouseSensitivity = 100f;

    private PlayerInputActions inputActions;

    private Vector2 lookInput;

    private float xRotation;

    private void Awake()
    {
        // Tạo Input Actions
        inputActions = new PlayerInputActions();

        // Khóa chuột giữa màn hình
        Cursor.lockState = CursorLockMode.Locked;

        // Ẩn chuột
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        // Bật input
        inputActions.Enable();

        // Đọc input chuột
        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        Look();
    }

    private void Look()
    {
        // Mouse X
        float mouseX =
            lookInput.x *
            mouseSensitivity *
            Time.deltaTime;

        // Mouse Y
        float mouseY =
            lookInput.y *
            mouseSensitivity *
            Time.deltaTime;

        // Xoay ngang player
        transform.Rotate(Vector3.up * mouseX);

        // Xoay dọc camera
        xRotation -= mouseY;

        // Giới hạn góc nhìn lên xuống
        xRotation = Mathf.Clamp(xRotation, -40f, 70f);

        // Xoay camera target
        cameraTarget.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        // Lấy giá trị chuột
        lookInput = context.ReadValue<Vector2>();
    }
}