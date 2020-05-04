using System.Collections;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(GunMuzzleFlash))]
[RequireComponent(typeof(AudioSource))]
public class Gun : Weapon
{
    //Components.
    GunMuzzleFlash muzzleFlash;

    [Header("Casing ejection")]
    public bool EjectCasingOnShoot;
    public GameObject casingPrefab;
    public Transform casingEjectPoint;

    [Header("Shoot Position")]
    public bool IsShooting;
    public Transform shootPosition;

    [Header("Inventory")]
    public int gunInventoryIndex;

    [Header("Aiming")]
    [Tooltip("Normal position relative to Gun holder game object attached to player.")]
    public Vector3 weaponPosAiming;

    public Camera playerCamera;

    [Header("Ammo")]
    public int MaxReserves;
    public int Reserves;

    public int MaxMagazine;
    public int Magazine;

    public int AmmoUsePerShot;

    [Tooltip("What percentage of the max magazine do you consider the gun to be low on ammo? UI shows ammo count as red when its low on ammo.")]
    [Range(0, 1)]
    public float lowOnAmmoPercentage;

    [Header("Single Fire")]
    public float singleRateOfFire = 2;

    [Header("Automatic")]
    public bool IsAutomatic;
    public float automaticRateOfFire = 5;
    public float lastFired;

    [Header("Reloading")]
    public bool AutoReload;
    public bool IsReloading;
    public float reloadTime = 2;
    public Coroutine reloadCoroutine;

    [Header("Animations")]
    public AnimationClip reloadAnimation;
    public AnimationClip idleAnimation;

    [Header("Audio")]
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

    [Header("Gun Kick Back")]
    public Vector2 kickMinMax = new Vector2(0.05f, 0.2f);

    public override void Awake()
    {
        base.Awake();
        muzzleFlash = GetComponent<GunMuzzleFlash>();
        playerCamera = Camera.main;
        Magazine = MaxMagazine;
        Reserves = MaxReserves;
    }

    public virtual void Fire(Vector3 fireFromPosition,Vector3 fireDirection)
    {
        Magazine -= AmmoUsePerShot;
        audioSource.PlayOneShot(shootAudio);

        uiBehaviour.UpdateAmmoCount(this);

        //Visuals
        transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
        muzzleFlash.Activate();

        if (EjectCasingOnShoot)
            EjectCasing();

        if (AutoReload && Magazine <= 0)
            Reload();
    }

    public void EjectCasing()
    {
        Instantiate(casingPrefab, casingEjectPoint.transform.position, Quaternion.identity);
    }
    public bool HasAmmoToShoot()
    {
        return Magazine >= AmmoUsePerShot;
    }
    public bool HasAmmoToReload()
    {
        return Magazine < MaxMagazine && Reserves > 0;
    }
    public virtual void Reload()
    {
        if (!IsReloading && HasAmmoToReload() && reloadCoroutine == null)
        {
            reloadCoroutine = StartCoroutine(_Reload());
        }
    }
    public IEnumerator _Reload()
    {
        IsReloading = true;
        animator.SetBool("Reloading", true);
        audioSource.PlayOneShot(reloadAudio);

        yield return new WaitForSeconds(reloadTime);

        int ammoToReload = MaxMagazine - Magazine;

        if (Reserves >= ammoToReload)
        {
            Magazine += ammoToReload;
            Reserves -= ammoToReload;
        }
        else
        {
            Magazine += Reserves;
            Reserves -= Reserves;
        }

        reloadCoroutine = null;
        IsReloading = false;
        animator.SetBool("Reloading", false);

        uiBehaviour.UpdateAmmoCount(this);
        uiBehaviour.UpdateReserveCount(this);
    }

    //If reload animation is playing when player switches weapon, reset the animation.
    //Animation is playing on the gameobject that is the first child of the Gun - hence GetChild(0).
    public override void OnDisable()
    {
        reloadAnimation.SampleAnimation(transform.GetChild(0).gameObject, 0);
    }
}
