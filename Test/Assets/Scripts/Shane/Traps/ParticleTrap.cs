using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrap : MonoBehaviour
{
    //Components
    public ParticleSystem[] particleSystems;
    public ParticleTrapButton trapButton;
    BoxCollider hitbox;

    public bool IsActive;

    [Header("Trap Hitbox")]
    public float hitboxEnableDelay;
    public float hitboxDisableDelay;

    [Header("Trap Properties")]
    public float damage;
    public float durationOfTrap;
    public float trapActiveTimer;

    [Header("Cooldown")]
    public bool IsInCooldown;
    public float cooldownTimer;
    public float cooldownTime;

    public CooldownDelegate CooldownEndedEvent;

    void Start()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        hitbox = GetComponent<BoxCollider>();
        hitbox.enabled = false;
    }

    public void Activate()
    {
        IsActive = true;
        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Play();
        }

        StartCoroutine(SwitchHitboxStatus(hitboxEnableDelay));
        StartCoroutine(CountdownToDisable());
    }
    public void Deactivate()
    {
        IsActive = false;
        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Stop();
        }

        StartCoroutine(SwitchHitboxStatus(hitboxDisableDelay));
    }
    IEnumerator SwitchHitboxStatus(float delayBeforeSwitching)
    {
        yield return new WaitForSeconds(delayBeforeSwitching);
        hitbox.enabled = !hitbox.enabled;
    }
    IEnumerator CountdownToDisable()
    {
        while(trapActiveTimer <= durationOfTrap)
        {
            trapActiveTimer += Time.deltaTime;
            yield return null;
        }

        Deactivate();
        trapActiveTimer = 0;

        StartCoroutine(CooldownCountdown());
    }
    IEnumerator CooldownCountdown()
    {
        IsInCooldown = true;

        while (cooldownTimer <= cooldownTime)
        {
            cooldownTimer += Time.deltaTime;
            yield return null;
        }

        CooldownEndedEvent();
        IsInCooldown = false;
        cooldownTimer = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("BreakInEnemy"))
        {
            other.GetComponent<HealthComponent>().ApplyDamage(damage,Vector3.up);
        }
    }
}
