using UnityEngine;

public class AmmoPickup : MonoBehaviour, IInteractable
{
    public int ammoAmount ;

    void Start()
    {
        ammoAmount = Random.Range(15 , 30);
    }

    public void Interact(PlayerInteract player)
    {
        AmmoInventory inventory =
            player.GetComponent<AmmoInventory>();

        inventory.AddAmmo(ammoAmount);

        Destroy(gameObject);
    }
}