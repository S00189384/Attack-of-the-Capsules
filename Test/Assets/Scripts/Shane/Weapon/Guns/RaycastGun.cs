using UnityEngine;

//Gun that shoots raycast.
public class RaycastGun : Gun
{
    protected Ray ray;
    protected RaycastHit hitInfo;
    protected RaycastHit[] objectsHit;
    [Header("Raycast Info")]
    public float range;

    public GameObject bulletHoleEffect;

    protected void ApplyDamage(HealthComponent healthComponent)
    {
        healthComponent.ApplyDamage(damage);
    }

    public void ShootRay(Vector3 origin, Vector3 direction)
    {
        ray = new Ray(origin, direction);
        Physics.Raycast(ray, out hitInfo, range,targetMask,QueryTriggerInteraction.Ignore);
    }

    public void ShootRayToApplyDamage(Vector3 origin, Vector3 direction)
    {
        ShootRay(origin, direction);

        if(Physics.Raycast(ray, out hitInfo, range, targetMask, QueryTriggerInteraction.Ignore))
        {
            HealthComponent healthComponent;
            hitInfo.collider.TryGetComponent(out healthComponent);

            if (healthComponent)
                healthComponent.ApplyDamage(damage,ray.direction);
        }
    }

    public override void Fire(Vector3 fireFromPosition,Vector3 fireDirection)
    {
        base.Fire(fireFromPosition,fireDirection);
        ShootRayToApplyDamage(fireFromPosition, fireDirection);
    }

    public override void Awake()
    {
        base.Awake();
    }
}
