using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponAnimations : MonoBehaviour
{
    protected MeleeWeapon meleeWeapon;

    public virtual void Start()
    {
        meleeWeapon = GetComponentInParent<MeleeWeapon>();
    }

    public void EnableHitbox()
    {
        meleeWeapon.EnableHitbox();
    }
    public void DisableHitbox()
    {
        meleeWeapon.DisableHitbox();
    }

    public void EndAttackAnimation()
    {
        meleeWeapon.EndAttackAnimation();
    }

    public void SwitchMeleeAttackStatus()
    {
        meleeWeapon.SwitchMeleeAttackStatus();
    }   
}
