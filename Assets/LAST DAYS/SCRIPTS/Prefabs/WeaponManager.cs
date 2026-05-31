using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    public GameObject[] weapons;

    public int currentWeapon;

    private PlayerInputActions input;

    // =================================================
    // AWAKE
    // =================================================
    void Awake()
    {
        input = new PlayerInputActions();
    }

    // =================================================
    // ENABLE
    // =================================================
    void OnEnable()
    {
        input.Enable();

        // number keys
        input.Player.Weapon1.performed += OnWeapon1;
        input.Player.Weapon2.performed += OnWeapon2;
        input.Player.Weapon3.performed += OnWeapon3;

        // scroll
        input.Player.ScrollWeapon.performed += OnScrollWeapon;
    }

    // =================================================
    // DISABLE
    // =================================================
    void OnDisable()
    {
        input.Player.Weapon1.performed -= OnWeapon1;
        input.Player.Weapon2.performed -= OnWeapon2;
        input.Player.Weapon3.performed -= OnWeapon3;

        input.Player.ScrollWeapon.performed -= OnScrollWeapon;

        input.Disable();
    }

    // =================================================
    // START
    // =================================================
    void Start()
    {
        SelectWeapon(0);
    }

    // =================================================
    // SELECT WEAPON
    // =================================================
    void SelectWeapon(int index)
    {
        // tránh lỗi index
        if (index < 0 || index >= weapons.Length)
            return;

        // tắt tất cả
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }

        // bật weapon được chọn
        weapons[index].SetActive(true);

        currentWeapon = index;
    }

    // =================================================
    // SCROLL
    // =================================================
    void OnScrollWeapon(InputAction.CallbackContext ctx)
    {
        Vector2 scroll = ctx.ReadValue<Vector2>();

        // scroll up
        if (scroll.y > 0)
        {
            currentWeapon++;

            if (currentWeapon >= weapons.Length)
            {
                currentWeapon = 0;
            }

            SelectWeapon(currentWeapon);
        }

        // scroll down
        if (scroll.y < 0)
        {
            currentWeapon--;

            if (currentWeapon < 0)
            {
                currentWeapon = weapons.Length - 1;
            }

            SelectWeapon(currentWeapon);
        }
    }

    // =================================================
    // NUMBER KEYS
    // =================================================
    void OnWeapon1(InputAction.CallbackContext ctx)
    {
        SelectWeapon(0);
    }

    void OnWeapon2(InputAction.CallbackContext ctx)
    {
        SelectWeapon(1);
    }

    void OnWeapon3(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        SelectWeapon(2);
    }

}