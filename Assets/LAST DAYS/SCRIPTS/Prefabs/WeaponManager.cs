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
        if (index < 0 || index >= weapons.Length)
            return;

        GunSystem currentGun =
            weapons[currentWeapon].GetComponentInChildren<GunSystem>(true);

        if (currentGun != null && currentGun.IsReloading())
        {
            Debug.Log("Đang reload, không đổi súng");
            return;
        }

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }

        weapons[index].SetActive(true);

        currentWeapon = index;
    }

    // =================================================
    // SCROLL
    // =================================================
    void OnScrollWeapon(InputAction.CallbackContext ctx)
    {
        if (Time.timeScale == 0f) return;

        Vector2 scroll = ctx.ReadValue<Vector2>();

        int nextWeapon = currentWeapon;

        if (scroll.y > 0)
        {
            nextWeapon++;

            if (nextWeapon >= weapons.Length)
                nextWeapon = 0;
        }
        else if (scroll.y < 0)
        {
            nextWeapon--;

            if (nextWeapon < 0)
                nextWeapon = weapons.Length - 1;
        }

        SelectWeapon(nextWeapon);
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