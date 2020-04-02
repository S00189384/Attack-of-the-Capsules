using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeWeaponAttack : MonoBehaviour
{
    GameManager gameManager;
    PlayerWeaponInventory PlayerWeaponInventory;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        PlayerWeaponInventory = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerWeaponInventory>();
    }

    void Update()
    {
        if(gameManager.CanControlPlayer)
        {
            if (PlayerWeaponInventory.activeMeleeWeapon)
            {
                if (Input.GetMouseButtonDown(0) && Time.time >= PlayerWeaponInventory.activeMeleeWeapon.nextTimeCanAttack)
                {
                    PlayerWeaponInventory.activeMeleeWeapon.nextTimeCanAttack = Time.time + PlayerWeaponInventory.activeMeleeWeapon.coolDownTime;
                    PlayerWeaponInventory.activeMeleeWeapon.animator.SetBool("Attacking", true);
                }
            }
        }
    }
}
