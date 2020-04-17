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
        SwitchInteractableStatus();
        animator.SetBool("Interacted", true);
        particleTrap.Activate();
        //Decrease player points if any.
    }
    public override void SwitchInteractableStatus()
    {
        if (IsInteractable)
            buttonLightMeshRenderer.material = buttonNotInteractableMaterial;
        else
            buttonLightMeshRenderer.material = buttonInteractableMaterial;

        base.SwitchInteractableStatus();
    }
    public void PlayInteractNoise()
    {
        audioSource.Play();
    }
}
