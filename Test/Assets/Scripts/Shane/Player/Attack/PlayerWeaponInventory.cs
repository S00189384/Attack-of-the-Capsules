using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerWeaponInventory : MonoBehaviour
{
    //Components.
    GameManager gameManager;
    PlayerGunAttack playerAttack;
    HealthComponent playerHealthComponent;
    UIBehaviour uiBehaviour;

    [Header("Weapons")]
    public Weapon[] weaponInventory = new Weapon[7];
    public Weapon activeWeapon;
    public int activeWeaponIndex = -1;

    [Header("Guns")]
    public Gun activeGun;

    [Header("Melee Weapons")]
    public MeleeWeapon activeMeleeWeapon;

    [Header("Throwable Weapons")]
    public ThrowableWeapon activeThrowableWeapon;

    public int numberOfGrenadesInInventory;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        uiBehaviour = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBehaviour>();
        playerAttack = GetComponent<PlayerGunAttack>();
        playerHealthComponent = GetComponentInParent<HealthComponent>();
        playerHealthComponent.OnDeathEvent += FindAndDestroyEquippedWeapon;

        if(activeWeapon)
            uiBehaviour.UpdateWeaponUIOnSwitchingWeapon(activeWeapon);
    }
    void Update()
    {
        SwitchWeaponsOnPlayerInput();
    }

    //Adding to inventory.
    public void AddWeaponToInventory(Weapon weaponToAdd)
    {
        //weaponInventory[weaponToAdd.playerInventoryIndex] = weaponToAdd;
        //uiBehaviour.inventoryUISlots[weaponToAdd.playerInventoryIndex].EnableInventoryItemPicture();
        //weaponToAdd = Instantiate(weaponInventory[weaponToAdd.playerInventoryIndex], transform.position, Quaternion.identity);
        //FixWeaponPosition(weaponToAdd);
        //transform.Find(weaponInventory[weaponToAdd.playerInventoryIndex].gameObject.name + "(Clone)").gameObject.SetActive(false);

        uiBehaviour.inventoryUISlots[weaponToAdd.playerInventoryIndex].EnableInventoryItemPicture();
        weaponToAdd = Instantiate(weaponToAdd, transform.position, Quaternion.identity);
        weaponInventory[weaponToAdd.playerInventoryIndex] = weaponToAdd;
        FixWeaponPosition(weaponToAdd);
        //transform.Find(weaponToAdd.gameObject.name).gameObject.SetActive(false);
        weaponToAdd.gameObject.SetActive(false);
    }
    public void AddThrowableToInventory(ThrowableWeapon throwableWeaponToAdd)
    {
        switch (throwableWeaponToAdd.tag)
        {
            case "Grenade":
                numberOfGrenadesInInventory++;
                uiBehaviour.UpdateThrowableRemaining(numberOfGrenadesInInventory);
                break;
        }

        //If there is none in inventory - add.
        if (weaponInventory[throwableWeaponToAdd.playerInventoryIndex] == null)
        {
            weaponInventory[throwableWeaponToAdd.playerInventoryIndex] = throwableWeaponToAdd;
            uiBehaviour.inventoryUISlots[throwableWeaponToAdd.playerInventoryIndex].EnableInventoryItemPicture();           
        }
    }

    //Switching / Equipping weapons.
    private void SwitchWeaponsOnPlayerInput()
    {
        if(gameManager.CanControlPlayer)
        {
            if (playerHealthComponent.IsAlive)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    SetActiveWeapon(0);
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    SetActiveWeapon(1);
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    SetActiveWeapon(2);
                if (Input.GetKeyDown(KeyCode.Alpha4))
                    SetActiveWeapon(3);
                if (Input.GetKeyDown(KeyCode.Alpha5))
                    SetActiveWeapon(4);
                if (Input.GetKeyDown(KeyCode.Alpha6))
                    SetActiveWeapon(5);
                if (Input.GetKeyDown(KeyCode.Alpha7))
                    SetActiveWeapon(6);
            }
        }
    }
    public void SetActiveWeapon(int index)
    {
        if (index != activeWeaponIndex)
        {
            if (weaponInventory[index] != null)
            {
                //If weapon equipped - disable it.
                if (activeGun)
                {
                    if (activeGun.IsReloading)
                    {
                        activeGun.IsReloading = false;
                        activeGun.reloadCoroutine = null;
                    }

                    activeGun.gameObject.SetActive(false);
                    activeGun = null;
                }
                if (activeMeleeWeapon)
                {
                    activeMeleeWeapon.gameObject.SetActive(false);
                    activeMeleeWeapon = null;
                }
                if(activeThrowableWeapon)
                {
                    activeThrowableWeapon.gameObject.SetActive(false);
                    activeThrowableWeapon = null;
                }

                //Check if weapon is already spawned in as a child of player.
                Transform weaponToEquip = transform.Find(weaponInventory[index].gameObject.name);

                //No - spawn it.
                if (!weaponToEquip)
                   activeWeapon = Instantiate(weaponInventory[index], transform.position, Quaternion.identity);
                //Yes - activate it.
                else
                {
                    weaponToEquip.gameObject.SetActive(true);
                    activeWeapon = weaponToEquip.GetComponent<Weapon>();
                }

                //Check what type of weapon it is.
                if (activeWeapon.CompareTag("Gun"))
                    activeGun = activeWeapon.GetComponent<Gun>();
                else if (activeWeapon.CompareTag("Melee"))
                    activeMeleeWeapon = activeWeapon.gameObject.GetComponent<MeleeWeapon>();
                else if (activeWeapon.CompareTag("Grenade"))
                    activeThrowableWeapon = activeWeapon.gameObject.GetComponent<Grenade>();

                //Set its position to be correct.
                FixWeaponPosition(activeWeapon);
                activeWeaponIndex = index;

                //Update UI.
                uiBehaviour.UpdateWeaponUIOnSwitchingWeapon(activeWeapon);
                uiBehaviour.EnableInventoryItemSlot(activeWeaponIndex);
            }
        }
    }
    public void EquipThrowable(int index)
    {
        activeWeapon = Instantiate(weaponInventory[index], transform.position, Quaternion.identity);
        activeThrowableWeapon = activeWeapon.gameObject.GetComponent<ThrowableWeapon>();

        //Set its position to be correct.
        FixWeaponPosition(activeThrowableWeapon);
        activeWeaponIndex = index;

        uiBehaviour.EnableInventoryItemSlot(activeWeaponIndex);
    }

    //Unequipping / removing weapons from player / inventory.
    public void UnequipWeaponFromPlayer(bool updateUI)
    {
        if(updateUI)
        {
            uiBehaviour.inventoryUISlots[activeWeaponIndex].ChangeBackgroundColourToUnequipped();
            uiBehaviour.inventoryUISlots[activeWeaponIndex].DisableInventoryItemPicture();
        }

        activeWeaponIndex = -1;
        activeWeapon = null;
        activeMeleeWeapon = null;
        activeThrowableWeapon = null;
        activeGun = null;
    }
    public void RemoveWeaponFromInventory(int index)
    {
        weaponInventory[index] = null;
    }
    public void FindAndDestroyEquippedWeapon()
    {
        if(activeWeaponIndex != -1)
            Destroy(transform.Find(weaponInventory[activeWeaponIndex].gameObject.name).gameObject);
    }
    public void ActivateEquippedWeapon()
    {
        if(activeWeaponIndex != -1)
            activeWeapon.gameObject.SetActive(true);
    }
    public void DisableEquippedWeapon()
    {
        if(activeWeaponIndex != -1)
            activeWeapon.gameObject.SetActive(false);

        //activeWeapon = null;
        //activeGun = null;
        //activeMeleeWeapon = null;
        //activeThrowableWeapon = null;
    }

    //Fix weapon position.
    public void FixWeaponPosition(Weapon weapon)
    {
        weapon.transform.parent = transform;
        weapon.transform.localPosition = weapon.defaultWeaponPosition;
        weapon.transform.forward = transform.forward;
        weapon.transform.localRotation = Quaternion.identity;
    }

    public Gun GetGunInPlayerInventory(string gunNameToSearchFor)
    {
        for (int i = 0; i < weaponInventory.Length; i++)
        {
            if(weaponInventory[i] != null && weaponInventory[i].nameOfWeapon == gunNameToSearchFor)
            {
                return weaponInventory[i].GetComponent<Gun>();
            }
        }

        return null;
    }
}
