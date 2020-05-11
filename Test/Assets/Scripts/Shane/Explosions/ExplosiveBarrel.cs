using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Explosive barrel that can be found all around level. Exploding damages enemies and player.
[RequireComponent(typeof(HealthComponent))]
public class ExplosiveBarrel : MonoBehaviour
{
    //Components.
    HealthComponent healthComponent;

    [Header("Prefabs")]
    public GameObject explodedBarrelPrefab;
    public ExplosionParticleEffect explosionParticleEffect;

    [Header("Damage")]
    public float explosionRadius;
    public float damage;
    public LayerMask layersToCheckIfCanDealDamage;
    public LayerMask targetMask;

    [Header("Time To Destroy")]
    public float timeToDestroyAfterExplosion;

    private void Start()
    {
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.OnDeathEvent += Explode;
    }

    public void Explode()
    {
        Instantiate(explosionParticleEffect, transform.position, Quaternion.Euler(-90, 0, 0));
        Destroy(Instantiate(explodedBarrelPrefab, transform.position, Quaternion.identity),timeToDestroyAfterExplosion);

        Ray damageCheckRay;
        RaycastHit hitInfo;
        Collider[] collidersInRangeOfExplosion = Physics.OverlapSphere(transform.position, explosionRadius, targetMask, QueryTriggerInteraction.Ignore);

        //Go through all enemies in explosion radius and check if wall is in the way of applying damage.
        for (int i = 0; i < collidersInRangeOfExplosion.Length; i++)
        {
            damageCheckRay = new Ray(transform.position, collidersInRangeOfExplosion[i].gameObject.transform.position - transform.position);       
            if (Physics.Raycast(damageCheckRay, out hitInfo, explosionRadius, layersToCheckIfCanDealDamage, QueryTriggerInteraction.Ignore))
            {
                if (((1 << hitInfo.collider.gameObject.layer) & targetMask) != 0)
                {
                    collidersInRangeOfExplosion[i].GetComponent<HealthComponent>().ApplyDamage(damage);
                }
            }
        }
    }
}
