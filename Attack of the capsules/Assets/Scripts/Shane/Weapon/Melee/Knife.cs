using UnityEngine;

public class Knife : MeleeWeapon
{
    //Components.
    Rigidbody rigidbody;
    PlayerWeaponInventory playerWeaponInventory;

    [Header("Throwing Knife")]
    public GameObject throwingKnife;
    public float throwForce;

    public AnimationClip throwingAnimation;

    public override void Awake()
    {
        base.Awake();
        rigidbody = GetComponent<Rigidbody>();
        playerWeaponInventory = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerWeaponInventory>();
    }

    //Trigger box collider attached is hitbox.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<NavMeshEnemy>())
        {
            other.gameObject.GetComponent<HealthComponent>().ApplyDamage(damage);
        }
    }

    public override void Update()
    {
        if(Input.GetMouseButton(1) && !IsDoingMeleeAttack)
        {
            animator.SetBool("Throw", true);
        }
    }

    public void ThrowKnife()
    {
        GameObject knife = Instantiate(throwingKnife, transform.position, Quaternion.identity);
        knife.transform.forward = transform.forward;

        playerWeaponInventory.UnequipWeaponFromPlayer(true);
        playerWeaponInventory.RemoveWeaponFromInventory(playerInventoryIndex);
        Destroy(gameObject);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        throwingAnimation.SampleAnimation(transform.GetChild(0).gameObject, 0);
    }
}
