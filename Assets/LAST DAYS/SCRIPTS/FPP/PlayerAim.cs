using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerAim : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera normalCamera;
    [SerializeField] private CinemachineVirtualCamera aimCamera;

    [Header("UI")]
    [SerializeField] private GameObject crosshair;

    private PlayerInputActions inputActions;
    private Animator anim;
    private bool isAiming;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Aim.started += OnAimStarted;
        inputActions.Player.Aim.canceled += OnAimCanceled;
    }

    private void OnDisable()
    {
        inputActions.Player.Aim.started -= OnAimStarted;
        inputActions.Player.Aim.canceled -= OnAimCanceled;

        inputActions.Player.Disable();
    }

    private void OnAimStarted(InputAction.CallbackContext context)
    {
        isAiming = true;

        normalCamera.Priority = 10;
        aimCamera.Priority = 20;

        crosshair.SetActive(true);
        anim.SetBool("IsAim", true);
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        isAiming = false;

        normalCamera.Priority = 20;
        aimCamera.Priority = 10;

        crosshair.SetActive(false);
        anim.SetBool("IsAim", false);
    }
}