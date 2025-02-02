﻿using UnityEngine;

public class Grenade : ThrowableWeapon
{
    //Components.
    PlayerWeaponInventory playerWeaponInventory;

    [Header("Grenade")]
    public bool IsActivated;

    [Header("Explosion")]
    public GameObject explosionEffect;
    public GameObject explosion;
    public float explosionRadius = 10;
    public float timeUntilExplosion = 3f;
    public LayerMask layersToCheckIfCanDealDamage;

    public override void Awake()
    {
        base.Awake();
        playerWeaponInventory = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerWeaponInventory>();
    }

    public void ActivateGrenade()
    {
        GetComponentInChildren<MeshCollider>().enabled = true;
        IsActivated = true;
        Invoke("Explode", timeUntilExplosion);
    }

    public void Explode()
    {
        //Spawn explosion effect.
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.Euler(-90,0,0));

        DamageNearbyTargets();
        Destroy(gameObject);
    }

    private void DamageNearbyTargets()
    {
        Ray damageCheckRay;
        RaycastHit hitInfo;
        Collider[] collidersInRangeOfExplosion = Physics.OverlapSphere(transform.position, explosionRadius, targetMask, QueryTriggerInteraction.Ignore);

        //Go through all enemies in explosion radius and check if wall is in the way of applying damage.
        for (int i = 0; i < collidersInRangeOfExplosion.Length; i++)
        {
            damageCheckRay = new Ray(transform.position, collidersInRangeOfExplosion[i].gameObject.transform.position - transform.position);
            Debug.DrawRay(damageCheckRay.origin, damageCheckRay.direction * explosionRadius, Color.green, 200);
            if (Physics.Raycast(damageCheckRay, out hitInfo, explosionRadius, layersToCheckIfCanDealDamage, QueryTriggerInteraction.Ignore))
            {
                if (((1 << hitInfo.collider.gameObject.layer) & targetMask) != 0)
                {
                    collidersInRangeOfExplosion[i].GetComponent<HealthComponent>().ApplyDamage(damage);
                }
            }
        }
    }
    public override void Throw()
    {
        base.Throw();

        rigidbody.useGravity = true;
        ActivateGrenade();

        //Updating Inventory.
        playerWeaponInventory.numberOfGrenadesInInventory--;

        //Update UI.
        uiBehaviour.UpdateThrowableRemaining(playerWeaponInventory.numberOfGrenadesInInventory);

        if (playerWeaponInventory.numberOfGrenadesInInventory > 0)
        {
            playerWeaponInventory.EquipThrowable(playerInventoryIndex);
        } 
        else
        {
            playerWeaponInventory.UnequipWeaponFromPlayer(true);
            playerWeaponInventory.RemoveWeaponFromInventory(playerInventoryIndex);
            uiBehaviour.DisableThrowableUI();
        }
    }
}
