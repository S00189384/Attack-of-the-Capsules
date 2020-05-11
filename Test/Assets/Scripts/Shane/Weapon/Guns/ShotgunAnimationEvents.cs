using UnityEngine;

/* 
 Temporary and really bad fix to call animation events from the animator which is in the child of the shotgun.
 Shotgun parent has the methods the animation events need to call and this is a way to access them.
     
     (I need animator in the child of the shotgun as I was having issues with moving shotgun during animation
    would reset the shotguns position to the origin. Having it as a child of another object fixed this. 
   )
*/

public class ShotgunAnimationEvents : MonoBehaviour
{
    Shotgun shotgun;

    void Start()
    {
        shotgun = GetComponentInParent<Shotgun>();
    }
    void EjectCasing()
    {
        shotgun.EjectCasing();
    }

    void ResetAnimationAfterPump()
    {
        shotgun.ResetAnimationAfterPump();
    }

    void SwitchToReload()
    {
        shotgun.SwitchToReloadAnimation();
    }
    void LoadShell()
    {
        shotgun.LoadShotgunShell();
    }
    void SwitchToIdleFromReload()
    {
        shotgun.SwitchToIdleFromReload();
    }
    void SwitchCheckingForEarlyReloadState()
    {
        shotgun.SwitchCheckingForEarlyReloadState();
    }
    void PlayPumpSound()
    {
        shotgun.PlayPumpSound();
    }
    void PlayerReloadSound()
    {
        shotgun.PlayReloadAudio();
    }
}
