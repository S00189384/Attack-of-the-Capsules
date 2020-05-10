using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrapButton : PlayerInteractableObject
{
    //Components.
    AudioSource audioSource;
    Animator animator;

    [Header("Trap It Controls")]
    public ParticleTrap particleTrap;

    [Header("Points Requirement")]
    public int pointsToUse;

    public GameObject buttonLight;
    public Material buttonInteractableMaterial;
    public Material buttonNotInteractableMaterial;
    private MeshRenderer buttonLightMeshRenderer;

    public override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        buttonLightMeshRenderer = buttonLight.GetComponent<MeshRenderer>();

        particleTrap.CooldownEndedEvent += SwitchInteractableStatus;
    }
    public override void PlayerInteracted()
    {
        if(IsInteractable)
        {
            SwitchInteractableStatus();
            animator.SetBool("Interacted", true);
            particleTrap.Activate();
            playerData.RemovePoints(pointsToUse);
        }
    }
    public override void DetermineIfInteractable()
    {
        if (!particleTrap.IsActive && !particleTrap.IsInCooldown && playerData.points >= pointsToUse)
            IsInteractable = true;
        else
            IsInteractable = false;
    }

    public override void SwitchInteractableStatus()
    {
        uiBehaviour.HidePlayerInteractMessage();

        if (IsInteractable)
        {
            buttonLightMeshRenderer.material = buttonNotInteractableMaterial;
            IsInteractable = false;
            UIMessageIfPlayerLooksAtObjectNotInteractable = "Trap is active";
            uiBehaviour.ShowPlayerInteractMessage(UIMessageIfPlayerLooksAtObjectNotInteractable, true, Color.red);
        }
        else
        {
            buttonLightMeshRenderer.material = buttonInteractableMaterial;
            if(playerData.points >= pointsToUse)
            {
                IsInteractable = true;
                UIMessageIfPlayerLooksAtObjectNotInteractable = "Press F To Use Trap - " + pointsToUse + " Points";
                uiBehaviour.ShowPlayerInteractMessage(UIMessageIfPlayerLooksAtObjectNotInteractable, true, Color.green);
            }
            else
            {
                UIMessageIfPlayerLooksAtObjectNotInteractable = "Not Enough Points To Use Trap - " + pointsToUse + " Required";
                uiBehaviour.ShowPlayerInteractMessage(UIMessageIfPlayerLooksAtObjectNotInteractable, true, Color.red);
            }
        }
    }
    public void PlayInteractNoise()
    {
        audioSource.Play();
    }
}
