using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileComputerScreen : PlayerInteractableObject
{
    //Components.
    public MissileComputerCanvas computerCanvas;

    [Header("Cooldown & Status")]
    public bool IsInCooldown;
    public float coolDownCountdownTime = 30f;
    public float coolDownTimer;

    public override void Start()
    {
        base.Start();
    }

    public void ResetCooldownTimer()
    {
        coolDownTimer = coolDownCountdownTime;
    }

    public override void PlayerInteracted()
    {
        if(!IsInCooldown)
        {
            //Set main message to be green and stop fading.
            computerCanvas.ResetMissileStatusFade();
            computerCanvas.SetMissileStatusColour(computerCanvas.missileStatusAvailableColour);
        }
    }

    public IEnumerator StartCooldown()
    {
        computerCanvas.SetMissileStatusColour(computerCanvas.missileStatusNotAvailableColour);
        StartCoroutine(computerCanvas.FadeUIText());

        //Activate countdown text on screen;

        while(coolDownTimer >= 0)
        {
            coolDownTimer -= Time.deltaTime;
            yield return null;
        }
    }
}
