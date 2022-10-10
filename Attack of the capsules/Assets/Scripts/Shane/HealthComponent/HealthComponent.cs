using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Health component attached to barricades, player, enemies, explosive barrels etc.
public delegate void OnDeath();
public class HealthComponent : MonoBehaviour
{
    [Header("Status")]
    public bool IsAlive = true;
    public bool CanTakeDamage = true;

    [Header("Health")]
    public float maxHealth = 100;
    public float currentHealth;

    [Header("Death")]
    public bool DestroyOnDeath;
    public event OnDeath OnDeathEvent;

    [Header("DirectionOfHit (If Any)")]
    public Vector3 directionOfHit;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void ApplyDamage(float amount)
    {
        if(CanTakeDamage)
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                CanTakeDamage = false;
                IsAlive = false;

                if (DestroyOnDeath)
                    Destroy(gameObject);

                OnDeathEvent?.Invoke();
            }
        }        
    }

    //If you want to store the direction a raycast hits object for use (particle effect death effect for example).
    public void ApplyDamage(float amount,Vector3 directionOfHit)
    {
        if(CanTakeDamage)
        {
            this.directionOfHit = directionOfHit;

            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                CanTakeDamage = false;
                IsAlive = false;

                if (DestroyOnDeath)
                    Destroy(gameObject);

                OnDeathEvent?.Invoke();
            }
        }       
    }
}
