using UnityEngine;

public class PlayerPickupItem : PlayerInteractableObject
{
    //Components.
    PlayerWeaponInventory playerWeaponInventory;

    [Header("Pickup Info")]
    public GameObject prefabToPickup;
    public string ObjectType;

    public override void Start()
    {
        base.Start();
        playerWeaponInventory = player.GetComponentInChildren<PlayerWeaponInventory>();
    }

    public override void PlayerInteracted()
    {
             
        switch (ObjectType)
        {
            case "Weapon":
                playerWeaponInventory.AddWeaponToInventory(prefabToPickup.GetComponent<Weapon>());
                break;
            case "Grenade":
                playerWeaponInventory.AddThrowableToInventory(prefabToPickup.GetComponent<Grenade>());
                break;
        }

        uiBehaviour.HidePlayerInteractMessage();
        Destroy(gameObject);
    }
}
