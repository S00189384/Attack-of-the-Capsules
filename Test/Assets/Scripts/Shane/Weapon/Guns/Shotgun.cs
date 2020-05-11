using System.Collections;
using UnityEngine;


/*Animation states for reloading - 
Initial reloading - Shotgun rotates into position.
Reloading - shell enters gun (checking if user presses shoot again to signal an early reload stoppage)
End Reloading - Shotgun rotates back into original position (Reset variables / animations)
*/
public class Shotgun : RaycastGun
{
    [Header("Shotgun Shooting")]
    public Vector3 spread = new Vector3(0.01f,0,0);
    public AudioClip pumpSound;

    [Header("Shotgun Animations")]
    public AnimationClip initialReloadAnimation;
    public AnimationClip pumpAfterShotAnimation;
    Coroutine pumpShotgunCoroutine;

    [Header("Shotgun Reloading")]
    public bool StopReloadEarly;
    public bool CheckingForEarlyReloadStop;
    int numberOfRoundsToReload;

    public override void Update()
    {
        if(CheckingForEarlyReloadStop)
        {
            if (Input.GetButtonDown("Fire1"))
                StopReloadEarly = true;
        }
    }

    public override void Fire(Vector3 fireFromPosition,Vector3 fireDirection)
    {
        base.Fire(fireFromPosition, fireDirection);
        ShootRayToApplyDamage(fireFromPosition, transform.forward + spread);
        ShootRayToApplyDamage(fireFromPosition, transform.forward - spread);

        StartPumpShotgunAnimation();
    }

    //Pump after Shooting.
    private void StartPumpShotgunAnimation()
    {
        if (pumpShotgunCoroutine == null)
            pumpShotgunCoroutine = StartCoroutine(PumpShotgun());
    }
    IEnumerator PumpShotgun()
    {
        if(magazine > 0)
        {
            yield return new WaitForSeconds(singleRateOfFire / 2.0f);

            animator.SetBool("Pump", true);
            PlayPumpSound();
        }
    }
    public void ResetAnimationAfterPump()
    {
        animator.SetBool("Pump", false);
        pumpShotgunCoroutine = null;
    }

    //Reloading.
    public override void Reload()
    {
        if (!IsReloading && HasAmmoToReload())
        {
            numberOfRoundsToReload = maxMagazine - magazine;
            IsReloading = true;
            animator.SetBool("InitialReloading", true);
        }
    }
    //Reload Animations.
    public void SwitchToReloadAnimation()
    {
        animator.SetBool("InitialReloading", false);
        animator.SetBool("Reloading", true);
    }
    public void LoadShotgunShell()
    {
        numberOfRoundsToReload--;
        reserves--;
        magazine++;

        uiBehaviour.UpdateAmmoCount(this);
        uiBehaviour.UpdateReserveCount(this);

        //Check to stop reload animation.
        if (numberOfRoundsToReload <= 0 || StopReloadEarly || reserves <= 0)
        {
            animator.SetBool("Reloading", false);
            animator.SetBool("EndReloading", true);
        }
    }
    public void SwitchToIdleFromReload()
    {
        animator.SetBool("EndReloading", false);
        IsReloading = false;
        StopReloadEarly = false;
    }
    public void SwitchCheckingForEarlyReloadState()
    {
        CheckingForEarlyReloadStop = !CheckingForEarlyReloadStop;
    }

    //Animation audio.
    public void PlayPumpSound()
    {
        audioSource.PlayOneShot(pumpSound);
    }
    public void PlayReloadAudio()
    {
        audioSource.PlayOneShot(reloadAudio);
    }

    //On switching weapon - reset animations.
    public override void OnDisable()
    {
        animator.SetBool("InitialReloading", false);
        animator.SetBool("Reloading", false);
        animator.SetBool("EndReloading", false);
        base.OnDisable();
        pumpAfterShotAnimation.SampleAnimation(transform.GetChild(0).gameObject, 0);
        initialReloadAnimation.SampleAnimation(transform.GetChild(0).gameObject, 0);

        StopReloadEarly = false;
        pumpShotgunCoroutine = null;
    }
}
