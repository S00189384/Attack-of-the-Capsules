using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
   Computer screen script handles player camera zooming in and out of screen and enables / disables cooldown. 
   Canvas script handles everything else (mainly updating what's on screen - cooldown timer countdown, status message etc.)
 */


public delegate void PlayerInteractionWithScreen();
public class MissileComputerScreen : PlayerInteractableObject
{
    //Components.
    public GameManager gameManager;
    public MissileComputerCanvas computerCanvas;
    HealthComponent playerHealthComponent;
    PlayerWeaponInventory playerWeaponInventory;
    AudioSource playerAudioSource;

    //Events.
    public PlayerInteractionWithScreen PlayerLookedAtScreenEvent;
    public PlayerInteractionWithScreen PlayerLookedAwayFromScreenEvent;

    //Capturing player camera info when they interact.
    private Vector3 playerCameraPositionAtStartOfInteraction;
    private Quaternion playerCameraRotationAtStartOfInteraction;
    private float playerCameraFieldOfView;

    [Header("Computer Camera")]
    public Camera computerScreenCamera;
    public RenderTexture computerScreenRenderTexture;
    private Camera playerCamera;

    [Header("Player Interaction")]
    public float interactSpeed = 5f;

    [Header("Cooldown")]
    public bool IsInCooldown;


    public override void Start()
    {
        base.Start();
        playerCameraFieldOfView = Camera.main.fieldOfView;
        playerHealthComponent = player.GetComponent<HealthComponent>();
        playerAudioSource = player.GetComponent<AudioSource>();
        playerWeaponInventory = player.GetComponentInChildren<PlayerWeaponInventory>();
        playerCamera = Camera.main;

        computerCanvas.CooldownStartedEvent += ActivateCooldown;
        computerCanvas.CooldownEndedEvent += DisableCooldown;
    }
    public override void Update()
    {
        //Testing.
        if(Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(MoveCameraBackToPlayerPosition());
        }
    }

    //Moving player camera to position;
    IEnumerator MoveAndRotatePlayerCameraToPosition(Vector3 targetPosition,Quaternion targetRotation,float targetFieldOfView)
    {
        playerCameraPositionAtStartOfInteraction = Camera.main.transform.position;
        playerCameraRotationAtStartOfInteraction = Camera.main.transform.rotation;
        float percentageComplete = 0;

        while(percentageComplete <= 1)
        {
            percentageComplete += interactSpeed * Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(playerCameraPositionAtStartOfInteraction, targetPosition, percentageComplete);
            Camera.main.transform.rotation = Quaternion.Lerp(playerCameraRotationAtStartOfInteraction, targetRotation, percentageComplete);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFieldOfView, percentageComplete);
            yield return null;
        }
    }
    IEnumerator MoveCameraToComputerScreen()
    {
        playerCameraPositionAtStartOfInteraction = Camera.main.transform.position;
        playerCameraRotationAtStartOfInteraction = Camera.main.transform.rotation;

        yield return StartCoroutine(MoveAndRotatePlayerCameraToPosition(computerScreenCamera.transform.position, computerScreenCamera.transform.rotation, computerScreenCamera.fieldOfView));

        computerScreenCamera.targetTexture = null;
        playerCamera.enabled = false;
        PlayerLookedAtScreenEvent();
        computerCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    }
    IEnumerator MoveCameraBackToPlayerPosition()
    {
        //Re-enable the comp screen.
        computerScreenCamera.enabled = true;
        computerScreenCamera.targetTexture = computerScreenRenderTexture;
        playerCamera.enabled = true;

        yield return StartCoroutine(MoveAndRotatePlayerCameraToPosition(playerCameraPositionAtStartOfInteraction, playerCameraRotationAtStartOfInteraction, playerCameraFieldOfView));

        PlayerLookedAwayFromScreenEvent();

        //Reset player state.
        gameManager.EnablePlayerMovement();
        playerWeaponInventory.ActivateEquippedWeapon();
        playerHealthComponent.CanTakeDamage = true;

        //Re-enable UI.
        uiBehaviour.EnableUIExceptInteractMessage();
    }

    //Interaction.
    public override void PlayerInteracted()
    {
        if(!IsInCooldown)
        {
            //Disable footstep audio from player if they were walking when they interacted.
            playerAudioSource.Stop();

            //Disable weapon temporarily if they have any. Not unequip but just make it not visible.
            playerWeaponInventory.DisableEquippedWeapon();

            //Reset UI.
            IsInteractable = false;
            uiBehaviour.DisableUIExceptInteractMessage();
            uiBehaviour.HidePlayerInteractMessage();

            //Disable movement & make player not take damage.
            gameManager.DisablePlayerMovement();
            playerHealthComponent.CanTakeDamage = false;

            //Move camera to position.
            StartCoroutine(MoveCameraToComputerScreen());
        }
    }

    public void ActivateCooldown()
    {
        IsInteractable = false;
        IsInCooldown = true;
    }
    public void DisableCooldown()
    {
        IsInteractable = true;
        IsInCooldown = false;
    }

    //Recontrolling player after missile explodes.
    public void RecontrolPlayer()
    {
        StartCoroutine(MoveCameraBackToPlayerPosition());
    }
}

