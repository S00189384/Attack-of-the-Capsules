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
    public float explosionRadius = 10;
    public float timeUntilExplosion = 3f;
    public AudioClip[] explosionSounds;

    public static int numberInPlayerInventory;

    public override void Awake()
    {
        base.Awake();
        playerWeaponInventory = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerWeaponInventory>();
    }

    public void ActivateGrenade()
    {
        IsActivated = true;
        Invoke("Explode", timeUntilExplosion);
    }

    public void Explode()
    {
        //Spawn explosion effect

        Collider[] collidersInRangeOfExplosion = Physics.OverlapSphere(transform.position, explosionRadius, targetMask, QueryTriggerInteraction.Ignore);

        //Visualising explosion radius.
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.position = transform.position;

        for (int i = 0; i < collidersInRangeOfExplosion.Length; i++)
        {
            if(((1 << collidersInRangeOfExplosion[i].gameObject.layer) & targetMask) != 0)
            {
                collidersInRangeOfExplosion[i].GetComponent<HealthComponent>().ApplyDamage(damage);
                print(collidersInRangeOfExplosion[i].gameObject.name);
            }
        }

        Destroy(gameObject);
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
