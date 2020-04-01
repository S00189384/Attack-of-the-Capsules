using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowableAttack : MonoBehaviour
{
    GameManager gameManager;
    PlayerWeaponInventory PlayerWeaponInventory;

    public float throwCooldownTime = 2;
    public float nextTimeCanThrow;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        PlayerWeaponInventory = GetComponent<PlayerWeaponInventory>();
    }

    private void Update()
    {
        if(gameManager.CanControlPlayer)
        {
            if (PlayerWeaponInventory.activeThrowableWeapon)
            {
                if (Input.GetMouseButtonDown(0) && Time.time > nextTimeCanThrow)
                {
                    nextTimeCanThrow = Time.time + throwCooldownTime;
                    PlayerWeaponInventory.activeThrowableWeapon.animator.SetBool("Throw", true);
                }
            }
        }
    }

}
