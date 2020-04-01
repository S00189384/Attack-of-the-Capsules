using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeAnimationEvents : MeleeWeaponAnimations
{
    public override void Start()
    {
        base.Start();
    }

    public void ThrowKnife()
    {
        meleeWeapon.GetComponent<Knife>().ThrowKnife();
    }

}
