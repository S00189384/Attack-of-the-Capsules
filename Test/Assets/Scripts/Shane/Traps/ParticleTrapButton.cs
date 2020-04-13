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

    [Header("Cooldown")]
    public float cooldownTime;
    public float cooldownTimer;

    public override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public override void PlayerInteracted()
    {
        IsInteractable = false;
        animator.SetBool("Interacted", true);
        particleTrap.Activate();
        //Decrease player points if any.
    }

    public void PlayInteractNoise()
    {
        audioSource.Play();
    }
}
