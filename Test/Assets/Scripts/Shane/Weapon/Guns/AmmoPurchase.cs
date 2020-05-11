using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//After buying weapon on wall it is replaced by an ammo purchase option.
//Don't need to check if weapon is in inventory as they have to buy the weapon first and they have no option to drop weapons.
public class AmmoPurchase : PlayerInteractableObject
{
    //Components.
    Animator animator;
    AudioSource audioSource;
    PlayerWeaponInventory playerWeaponInventory;

    [Header("Info")]
    public string gunNameToAddAmmoTo;
    public int pointsForPurchase;
    public int ammoToGive;

    [Header("Text Display")]
    public TextMeshPro TMProAmmoText;
    public Color textDisplayColourInteractable;
    public Color textDisplayColourNotInteractable;

    [Header("Audio")] 
    public AudioClip gunPurchase; // Played when enabled as this object gets enabled when gun is purchased.
    public AudioClip ammoPurchase;

    public override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
        animator.Play("AmmoPurchaseIdle");
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(gunPurchase);
        
        playerWeaponInventory = player.GetComponentInChildren<PlayerWeaponInventory>();
    }

    public override void DetermineIfInteractable()
    {
        Gun gunToAddAmmoTo = playerWeaponInventory.GetGunInPlayerInventory(gunNameToAddAmmoTo);

        if (playerData.points >= pointsForPurchase && gunToAddAmmoTo.reserves < gunToAddAmmoTo.maxReserves)
        {
            IsInteractable = true;
            TMProAmmoText.color = textDisplayColourInteractable;
        }
        else
        {
            IsInteractable = false;
            TMProAmmoText.color = textDisplayColourNotInteractable;

            if (gunToAddAmmoTo.reserves >= gunToAddAmmoTo.maxReserves)
            {
                UIMessageIfPlayerLooksAtObjectNotInteractable = gunToAddAmmoTo.nameOfWeapon + " Has Max Amount Of Ammo";
            }
            else
            {
                UIMessageIfPlayerLooksAtObjectNotInteractable = "Not Enough Points - " + pointsForPurchase + "Required";
            }
        }
    }

    public override void PlayerInteracted()
    {
        Gun gun = null;

        for (int i = 0; i < playerWeaponInventory.weaponInventory.Length; i++)
        {
            //Find Gun to add ammo to in inventory.
            if(playerWeaponInventory.weaponInventory[i] != null && playerWeaponInventory.weaponInventory[i].nameOfWeapon == gunNameToAddAmmoTo)
            {
                gun = playerWeaponInventory.weaponInventory[i].GetComponent<Gun>();
                
                //Check if gun already was spawned in and is child of player.
                //Transform gunChildOfPlayer = playerWeaponInventory.transform.Find(gun.gameObject.name + "(Clone)");
                //gun = gunChildOfPlayer.GetComponent<Gun>();

                gun.AddAmmo(ammoToGive);

                //If weapon is equipped while purchasing - update ui reserve count.
                if (playerWeaponInventory.activeGun == gun)
                    uiBehaviour.UpdateReserveCount(gun);

                playerData.RemovePoints(pointsForPurchase);

                //Hide temporarily in case player keeps looking at object when they make a purchase and it becomes not interactable anymore - therefore change colour of UI message.
                uiBehaviour.HidePlayerInteractMessage();
            }
        }
    }
}
