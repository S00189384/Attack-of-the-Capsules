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

    private Gun gunToAddAmmoTo;

    [Header("Info")]
    public string gunNameToAddAmmoTo;
    public int pointsForPurchase;
    public int ammoToGive;

    [Header("Text Display")]
    public TextMeshPro TMProAmmoText;
    public Color textDisplayColourInteractable;
    public Color textDisplayColourNotInteractable;

    [Header("Audio")] 
    public AudioClip gunPurchase; // Played at start as this object gets enabled when gun is purchased.
    public AudioClip ammoPurchase;

    public override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
        animator.Play("AmmoPurchaseIdle");
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(gunPurchase);
        
        playerWeaponInventory = player.GetComponentInChildren<PlayerWeaponInventory>();
     
        gunToAddAmmoTo = playerWeaponInventory.GetGunInPlayerInventory(gunNameToAddAmmoTo);
        gunToAddAmmoTo.GunReloadedEvent += DetermineIfInteractable;
    }

    public override void DetermineIfInteractable()
    {
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
                UIMessageIfPlayerLooksAtObjectNotInteractable = "Not Enough Points - " + pointsForPurchase + " Required";
            }
        }
    }

    public override void PlayerInteracted()
    {
        audioSource.PlayOneShot(ammoPurchase);

        gunToAddAmmoTo.AddAmmo(ammoToGive);
        if (playerWeaponInventory.activeGun == gunToAddAmmoTo)
            uiBehaviour.UpdateReserveCount(gunToAddAmmoTo);

        playerData.RemovePoints(pointsForPurchase);

        //Hide temporarily in case player keeps looking at object when they make a purchase and it becomes not interactable anymore - therefore change colour of UI message.
        uiBehaviour.HidePlayerInteractMessage();
    }
}
