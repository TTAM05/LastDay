using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public Camera cam;
    public float distance = 3f;
    public TMP_Text[] interactUI;
    private PlayerInputActions input;

    void Awake()
    {
        input = new PlayerInputActions();
        if (cam == null)
            cam = Camera.main;

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
            if (hit.collider.CompareTag("Ammo"))
            {
                IInteractable interactable =
                    hit.collider.GetComponent<IInteractable>();
                    
                if (interactable != null)
                {
                    interactable.Interact(this);
                   
                }
            }
            else if (hit.collider.CompareTag("Hunter"))
            {
                HunterDialogueZone hunterDialogueZone =
                hit.collider.transform.root.GetComponentInChildren<HunterDialogueZone>();

                if (hunterDialogueZone != null)
                {
                    hunterDialogueZone.Interact(this);
    
                }

                
            }
        }
    }

    void ShowUI(bool show, int index = 0)
    {
        if (interactUI == null || interactUI.Length == 0)
            return;

        for (int i = 0; i < interactUI.Length; i++)
        {
            var txt = interactUI[i];
            if (txt == null)
                continue;

            txt.gameObject.SetActive(show && i == index);
        }
    }

    void Check()
    {
        Ray ray =
            cam.ViewportPointToRay(
                new Vector3(0.5f, 0.5f)
            );

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            if (hit.collider.CompareTag("Ammo"))
            {
                ShowUI(true, 0);
                return;
            }
            else if (hit.collider.CompareTag("Hunter"))
            {
                ShowUI(true, 1);
                return;
            }
        }

        ShowUI(false, 0);
    }
}