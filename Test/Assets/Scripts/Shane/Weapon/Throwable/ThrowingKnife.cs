using UnityEngine;

public class ThrowingKnife : ThrowableWeapon
{
    [Header("Throwing Knife")]
    public bool CanRotate = true;
    public float rotationSpeed = 50;

    [Header("Throwing Knife Audio")]
    public AudioClip impactOnEnemy;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        if (CanRotate)
            transform.Rotate(Vector3.right, rotationSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CanRotate = false;

        //Hit one of the target layers.
        if (CanDamageTarget && (((1 << collision.gameObject.layer) & targetMask) != 0))
        {
            collision.gameObject.GetComponent<HealthComponent>().ApplyDamage(damage);
            CanDamageTarget = false;
        }
        else if(collision.gameObject.tag != "Player")
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            transform.forward = -collision.GetContact(0).normal;
        }
    }
}
