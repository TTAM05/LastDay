using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public Camera cam;
    public float distance = 3f;

    private PlayerInputActions input;

    void Awake()
    {
        input = new PlayerInputActions();
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
}