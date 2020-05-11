using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Had an issue where the players camera couldn't move when the animator was playing the players idle animation.
//To get around this I just have one animation for death - when the player dies I enable the animator and start the death animation;
//If I wanted to give the player more animations I would have to fix this but it works for now.
[RequireComponent(typeof(Animator))]
public class PlayerAnimations : MonoBehaviour
{
    UIBehaviour uiBehaviour;
    HealthComponent playerHealthComponent;
    Animator animator;

    [Header("Death")]
    public float fadeToBlackSpeed;

    void Start()
    {
        uiBehaviour = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBehaviour>();
        playerHealthComponent = GetComponent<HealthComponent>();
        playerHealthComponent.OnDeathEvent += StartDeathAnimation;

        animator = GetComponent<Animator>();
    }

    private void StartDeathAnimation()
    {
        GetComponent<Animator>().enabled = true;
        animator.SetBool("Death", true);
    }

    private void StartFadeToBlack()
    {
        uiBehaviour.FadeToBlackAndLoadScene(fadeToBlackSpeed,2);
    }
}
