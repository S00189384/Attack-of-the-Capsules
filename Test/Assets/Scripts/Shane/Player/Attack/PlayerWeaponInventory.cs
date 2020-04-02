﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerWeaponInventory : MonoBehaviour
{
    //Components.
    GameManager gameManager;
    PlayerGunAttack playerAttack;
    HealthComponent playerHealthComponent;

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

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        playerAttack = GetComponent<PlayerGunAttack>();
        playerHealthComponent = GetComponentInParent<HealthComponent>();
        playerHealthComponent.OnDeathEvent += FindAndDestroyEquippedWeapon;
    }
    void Update()
    {
        SwitchWeaponsOnPlayerInput();
    }

    //Adding to inventory.
    public void AddWeaponToInventory(Weapon weaponToAdd)
    {
        weaponInventory[weaponToAdd.playerInventoryIndex] = weaponToAdd;
    }
    public void AddThrowableToInventory(ThrowableWeapon throwableWeaponToAdd)
    {
        if(weaponInventory[throwableWeaponToAdd.playerInventoryIndex] == null)
            weaponInventory[throwableWeaponToAdd.playerInventoryIndex] = throwableWeaponToAdd;
    }

    //Switching / Equipping weapons.
    void SwitchWeaponsOnPlayerInput()
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
                Transform weaponToEquip = transform.Find(weaponInventory[index].gameObject.name + "(Clone)");

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
                FixActiveWeaponPosition();
                activeWeaponIndex = index;
            }
        }
    }
    public void EquipThrowable(int index)
    {
        activeWeapon = Instantiate(weaponInventory[index], transform.position, Quaternion.identity);
        activeThrowableWeapon = activeWeapon.gameObject.GetComponent<ThrowableWeapon>();

        //Set its position to be correct.
        FixActiveWeaponPosition();
        activeWeaponIndex = index;
    }

    //Unequipping / removing weapons from player / inventory.
    public void UnequipWeaponFromPlayer()
    {
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
            Destroy(transform.Find(weaponInventory[activeWeaponIndex].gameObject.name + "(Clone)").gameObject);
    }

    //Fix weapon position.
    public void FixActiveWeaponPosition()
    {
        activeWeapon.transform.parent = transform;
        activeWeapon.transform.localPosition = activeWeapon.defaultWeaponPosition;
        activeWeapon.transform.forward = transform.forward;
    }

    //Old weapon inventory setup.
    //public Gun[] gunInventory = new Gun[4];
    //public int activeGunIndex = -1;

    //[Header("Melee Weapons")]
    //public MeleeWeapon meleeWeaponInventory;
    //public MeleeWeapon activeMeleeWeapon;
    //if (Input.GetKeyDown(KeyCode.Alpha1))
    //    SetActiveGun(0);
    //if (Input.GetKeyDown(KeyCode.Alpha2))
    //    SetActiveGun(1);
    //if (Input.GetKeyDown(KeyCode.Alpha3))
    //    SetActiveGun(2);
    //if (Input.GetKeyDown(KeyCode.Alpha4))
    //    SetActiveGun(3);
    //if (Input.GetKeyDown(KeyCode.Alpha5))
    //    EquipKnife();

    //public void AddGunToInventory(Gun weaponToAdd)
    //{
    //    gunInventory[weaponToAdd.gunInventoryIndex] = weaponToAdd;
    //}

    ////Only a knife for now.
    //public void AddMeleeWeaponToInventory(MeleeWeapon weaponToAdd)
    //{
    //    meleeWeaponInventory = weaponToAdd;
    //}
    //void SetActiveGun(int index)
    //{
    //    if (index != activeGunIndex)
    //    {
    //        if (gunInventory[index] != null)
    //        {
    //            if (activeGun)
    //            {
    //                if (activeGun.IsReloading)
    //                {
    //                    activeGun.IsReloading = false;
    //                    activeGun.reloadCoroutine = null;
    //                }

    //                activeGun.gameObject.SetActive(false);
    //            }

    //            if(activeMeleeWeapon)
    //            {
    //                activeMeleeWeapon.gameObject.SetActive(false);
    //                activeMeleeWeapon = null;
    //            }

    //            Transform gunToEquip = transform.Find(gunInventory[index].gameObject.name + "(Clone)");

    //            if (!gunToEquip) 
    //                activeGun = Instantiate(gunInventory[index], transform.position, Quaternion.identity);
    //            else
    //            {
    //                gunToEquip.gameObject.SetActive(true);
    //                activeGun = gunToEquip.GetComponent<Gun>();
    //            }

    //            activeGun.transform.parent = transform;
    //            activeGun.transform.localPosition = activeGun.weaponPosNotAiming;
    //            activeGun.transform.forward = transform.forward;
    //            activeGunIndex = index;
    //        }
    //    }
    //}

    //public void EquipKnife()
    //{
    //    if(!activeMeleeWeapon)
    //    {
    //        //If any guns are equipped deactivate them.
    //        if (activeGun)
    //        {
    //            if (activeGun.IsReloading)
    //            {
    //                activeGun.IsReloading = false;
    //                activeGun.reloadCoroutine = null;
    //            }

    //            activeGun.gameObject.SetActive(false);
    //            activeGun = null;
    //            activeGunIndex = -1;
    //        }

    //        Transform meleeWeaponToEquip = transform.Find(meleeWeaponInventory.gameObject.name + "(Clone)");

    //        if(!meleeWeaponToEquip)
    //            activeMeleeWeapon = Instantiate(meleeWeaponInventory, meleeWeaponInventory.weaponPosition, Quaternion.identity);
    //        else
    //        {
    //            meleeWeaponToEquip.gameObject.SetActive(true);
    //            activeMeleeWeapon = meleeWeaponToEquip.GetComponent<MeleeWeapon>();
    //        }

    //        activeMeleeWeapon.transform.parent = transform;
    //        activeMeleeWeapon.transform.localPosition = activeMeleeWeapon.weaponPosition;
    //        activeMeleeWeapon.transform.forward = transform.forward;
    //    }     
    //}
}
