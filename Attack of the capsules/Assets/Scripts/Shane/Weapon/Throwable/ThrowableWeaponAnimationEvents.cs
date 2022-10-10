using UnityEngine;

public class ThrowableWeaponAnimationEvents : MonoBehaviour
{
    ThrowableWeapon throwableWeapon;
    void Start()
    {
        throwableWeapon = GetComponentInParent<ThrowableWeapon>();
    }

    public void Throw()
    {
        throwableWeapon.Throw();
    }
}
