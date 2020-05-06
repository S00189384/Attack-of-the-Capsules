using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : ThrowableWeapon
{
    //Components.
    PlayerWeaponInventory playerWeaponInventory;

    [Header("Grenade")]
    public bool IsActivated;

    [Header("Explosion")]
    public GameObject explosionEffect;
    public float explosionRadius = 10;
    public float timeUntilExplosion = 3f;

    public static int numberInPlayerInventory;

    public override void Awake()
    {
        base.Awake();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerWeaponInventory = player.GetComponentInChildren<PlayerWeaponInventory>();
        //Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponentInChildren<Collider>(), true);
    }

    public void ActivateGrenade()
    {
        IsActivated = true;
        Invoke("Explode", timeUntilExplosion);
    }

    public void Explode()
    {
        //Spawn explosion effect.
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.Euler(-90,0,0));

        //Makeshift way to visualise explosion radius.
        //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //go.transform.position = transform.position;
        //go.GetComponent<SphereCollider>().radius = explosionRadius;
        //go.GetComponent<SphereCollider>().isTrigger = true;

        DamageNearbyTargets();
        Destroy(gameObject);
    }

    private void DamageNearbyTargets()
    {
        Collider[] collidersInRangeOfExplosion = Physics.OverlapSphere(transform.position, explosionRadius, targetMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < collidersInRangeOfExplosion.Length; i++)
        {
            if (((1 << collidersInRangeOfExplosion[i].gameObject.layer) & targetMask) != 0)
            {
                collidersInRangeOfExplosion[i].GetComponent<HealthComponent>().ApplyDamage(damage);
                print(collidersInRangeOfExplosion[i].gameObject.name);
            }
        }
    }
    public override void Throw()
    {
        base.Throw();

        rigidbody.useGravity = true;
        ActivateGrenade();

        //Updating Inventory.
        numberInPlayerInventory--;

        //Update UI.
        uiBehaviour.UpdateThrowableRemaining(numberInPlayerInventory);

        if (numberInPlayerInventory > 0)
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
