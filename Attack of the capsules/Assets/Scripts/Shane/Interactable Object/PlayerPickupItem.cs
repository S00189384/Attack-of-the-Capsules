﻿using UnityEngine;

//At the moment only item that can be picked up is weapons.
//When picked up, checks which type of item / weapon it is and adds to inventory.

public class PlayerPickupItem : PlayerInteractableObject
{
    //Components.
    PlayerWeaponInventory playerWeaponInventory;

    [Header("Pickup Info")]
    public AmmoPurchase ammoPurchase;
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
            if(ammoPurchase != null)
                ammoPurchase.gameObject.SetActive(true);
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
