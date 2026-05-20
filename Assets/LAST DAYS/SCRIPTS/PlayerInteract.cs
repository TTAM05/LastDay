using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public Camera cam;
    public float distance = 3f;
    public TMP_Text interactUI;
    private PlayerInputActions input;

    void Awake()
    {
        input = new PlayerInputActions();
        ShowUI(false);
    }

    void Update()
    {
        Check();
    }

    void OnEnable()
    {
        input.Enable();

        input.Player.Interact.performed += OnInteract;
    }

    void OnDisable()
    {
        input.Player.Interact.performed -= OnInteract;

        input.Disable();
    }

    void OnInteract(InputAction.CallbackContext ctx)
    {
        Ray ray =
            cam.ViewportPointToRay(
                new Vector3(0.5f, 0.5f)
            );

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            IInteractable interactable =
                hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact(this);
            }
        }
    }

    void ShowUI(bool show)
    {
        interactUI.alpha = show ? 1f : 0f;
        // interactUI.blocksRaycasts = false;
    }

    void Check()
    {
        Ray ray =
            cam.ViewportPointToRay(
                new Vector3(0.5f, 0.5f)
            );

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            IInteractable interactable =
                hit.collider.GetComponent<IInteractable>();

            ShowUI(interactable != null);
        }
        else
        {
            ShowUI(false);
        }
    }
}