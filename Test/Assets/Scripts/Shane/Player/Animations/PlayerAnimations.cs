using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimations : MonoBehaviour
{
    HealthComponent playerHealthComponent;
    Animator animator;

    [Header("Death")]
    public float fadeToBlackSpeed;

    void Start()
    {
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
        StartCoroutine(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().FadeToBlackAndLoadScene(0,fadeToBlackSpeed,2));
    }
}
