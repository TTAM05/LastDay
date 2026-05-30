using UnityEngine;
using UnityEngine.InputSystem;

public class MacheteSystem : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    private PlayerInputActions inputActions;
    private AudioSource audioSource;
    public AudioClip slideClip;

    void Reset()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();

        inputActions.Player.Fire.performed += OnAttack;
        inputActions.Player.Enable();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>() ?? GetComponentInParent<AudioSource>();
        if (audioSource != null)
            audioSource.playOnAwake = false;
    }

    void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Fire.performed -= OnAttack;
            inputActions.Player.Disable();
        }
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (animator != null)
            animator.SetTrigger("Slide");
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>() ?? GetComponentInParent<AudioSource>();

        if (audioSource != null && slideClip != null)
            audioSource.PlayOneShot(slideClip);
        else if (audioSource == null)
            Debug.LogWarning("MacheteSystem: AudioSource not found for slideClip playback.");
    }
}
