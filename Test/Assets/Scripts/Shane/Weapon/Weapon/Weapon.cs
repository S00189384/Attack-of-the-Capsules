using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;

    [Header("Player Inventory")]
    public Weapon weaponPrefab;
    public int playerInventoryIndex;

    [Header("Weapon Position")]
    public Vector3 defaultWeaponPosition;

    [Header("Weapon Targets")]
    public float damage;
    public LayerMask targetMask;

    public virtual void Start()
    {

    }

    public virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void Update()
    {

    }

    public virtual void OnDisable()
    {

    }

}
