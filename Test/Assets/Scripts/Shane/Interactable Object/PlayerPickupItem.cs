using UnityEngine;

public class PlayerPickupItem : PlayerInteractableObject
{
    //Components.
    PlayerWeaponInventory playerWeaponInventory;

    [Header("Pickup Info")]
    public GameObject prefabToPickup;
    public string ObjectType;
    public int pointsRequiredToPickup;
    private bool itemCanBePickedUp;

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
                if(playerData.points >= pointsRequiredToPickup)
                {
                    playerWeaponInventory.AddWeaponToInventory(prefabToPickup.GetComponent<Weapon>());
                    itemCanBePickedUp = true;
                }
                break;
            case "Grenade":
                playerWeaponInventory.AddThrowableToInventory(prefabToPickup.GetComponent<Grenade>());
                itemCanBePickedUp = true;
                break;
        }

        if(itemCanBePickedUp)
        {
            playerData.RemovePoints(pointsRequiredToPickup);
            uiBehaviour.HidePlayerInteractMessage();
            Destroy(gameObject);
        }
    }

    public override void DetermineIfInteractable()
    {
        if (playerData.points >= pointsRequiredToPickup)
            IsInteractable = true;
        else
            IsInteractable = false;
    }
}
