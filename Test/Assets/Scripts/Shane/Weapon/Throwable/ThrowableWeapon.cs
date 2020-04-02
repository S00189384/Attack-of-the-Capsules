using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowableWeapon : Weapon
{
    //Components.
    protected Rigidbody rigidbody;

    [Header("Throwable Weapon")]
    public bool ThrowOnSpawn;

    //public int numberOfThrowableInPlayerInventory;

    [Header("Throw Options")]
    public float throwForce;
    public bool CanDamageTarget = true;

    [Header("Throwable Weapon Audio Clips & Animations")]
    public AudioClip throwAudio;
    public AnimationClip throwAnimation;

    public override void Awake()
    {
        base.Awake();
        rigidbody = GetComponent<Rigidbody>();
        transform.forward = Camera.main.transform.forward;

        if(ThrowOnSpawn)
            Throw();
    }

    public virtual void Throw()
    {
        audioSource.PlayOneShot(throwAudio);
        transform.parent = null;
        rigidbody.AddForce((transform.forward) * throwForce, ForceMode.Impulse);
    }

    public override void OnDisable()
    {
        throwAnimation.SampleAnimation(transform.GetChild(0).gameObject, 0);
    }
}
