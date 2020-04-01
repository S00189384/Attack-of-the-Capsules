using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnDeath();
public class HealthComponent : MonoBehaviour
{
    [Header("Alive or Not")]
    public bool IsAlive = true;

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
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            IsAlive = false;

            if(DestroyOnDeath)
                Destroy(gameObject);

            OnDeathEvent?.Invoke();
        }
    }

    //If you want to store the direction a raycast hits object for use (particle effect death effect for example).
    public void ApplyDamage(float amount,Vector3 directionOfHit)
    {
        this.directionOfHit = directionOfHit;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            IsAlive = false;

            if (DestroyOnDeath)
                Destroy(gameObject);

            OnDeathEvent?.Invoke();
        }
    }

}
