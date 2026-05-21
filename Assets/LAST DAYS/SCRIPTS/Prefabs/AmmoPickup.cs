using UnityEngine;

public class AmmoPickup : MonoBehaviour, IInteractable
{
    public int ammoAmount;
    public int weaponIndex; // loại súng nhận ammo
    public float rotationSpeed = 50f;

    void Start()
    {
        ammoAmount = Random.Range(15, 30);

        // nếu không set thì random weapon
        // weaponIndex = Random.Range(0, 2);
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void Interact(PlayerInteract player)
    {
        AmmoInventory inventory = player.GetComponent<AmmoInventory>();

        inventory.AddAmmo(weaponIndex, ammoAmount);

        Destroy(gameObject);
    }
}