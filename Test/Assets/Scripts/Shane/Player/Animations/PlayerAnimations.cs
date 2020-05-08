using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        uiBehaviour.FadeToBlackAndLoadScene(fadeToBlackSpeed,1);
    }
}
