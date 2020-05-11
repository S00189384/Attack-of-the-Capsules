using UnityEngine;

public class MeleeWeapon : Weapon
{
    //Components.
    BoxCollider attackHitbox;

    [Header("Attacking")]
    public float nextTimeCanAttack;
    public float coolDownTime = 2;
    public bool IsDoingMeleeAttack;

    public override void Awake()
    {
        //Hitbox
        base.Awake();
        attackHitbox = GetComponent<BoxCollider>();
        DisableHitbox();
    }

    public void EnableHitbox()
    {
        attackHitbox.enabled = true;
    }
    public void DisableHitbox()
    {
        attackHitbox.enabled = false;
    }

    public void EndAttackAnimation()
    {
        animator.SetBool("Attacking", false);
    }

    public void SwitchMeleeAttackStatus()
    {
        IsDoingMeleeAttack = !IsDoingMeleeAttack;
    }

    public override void OnDisable()
    {
        IsDoingMeleeAttack = false;
    }
}
