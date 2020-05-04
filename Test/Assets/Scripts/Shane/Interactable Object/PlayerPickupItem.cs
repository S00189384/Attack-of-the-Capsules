using UnityEngine;

public class PlayerPickupItem : PlayerInteractableObject
{
    [Header("Pickup Info")]
    public GameObject prefabToPickup;
    public string ObjectType;

    public override void PlayerInteracted()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        switch (ObjectType)
        {
            case "Weapon":
                player.GetComponentInChildren<PlayerWeaponInventory>().AddWeaponToInventory(prefabToPickup.GetComponent<Weapon>());
                break;
            case "Grenade":
                player.GetComponentInChildren<PlayerWeaponInventory>().AddThrowableToInventory(prefabToPickup.GetComponent<Grenade>());
                Grenade.numberInPlayerInventory++;
                break;
        }

        uiBehaviour.HidePlayerInteractMessage();
        Destroy(gameObject);
    }
}
