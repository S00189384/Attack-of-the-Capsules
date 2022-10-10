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
