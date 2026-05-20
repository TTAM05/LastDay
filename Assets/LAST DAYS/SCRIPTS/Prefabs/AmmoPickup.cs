using UnityEngine;

public class AmmoPickup : MonoBehaviour, IInteractable
{
    public int ammoAmount ;
    public float rotationSpeed = 50f;

    void Start()
    {
        ammoAmount = Random.Range(15 , 30);

       
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void Interact(PlayerInteract player)
    {
        AmmoInventory inventory =
            player.GetComponent<AmmoInventory>();

        inventory.AddAmmo(ammoAmount);

        Destroy(gameObject);
    }
}