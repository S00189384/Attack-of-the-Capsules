using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
   Computer screen script handles player camera zooming in and out of screen and enables / disables cooldown. 
   Canvas script handles everything else (mainly updating what's on screen - cooldown timer countdown, status message etc.)

    The missile screen canvas scaler is constant pixel size by default, but when you interact and zoom in the camera changes and the canvas has to be readjusted to take into account possible different screen sizes.
    There was an issue with changing the scaling from constant pixel size to scale with screen size once you're fully zoomed in - sometimes the switching between the two was really noticable and just looked bad. 
    A fix I came up with was to apply the scaling change just before the camera fully zoomed in in the coroutine (close to 100% worked quite well).   
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
    CanvasScaler computerCanvasScaler;
    public Camera computerScreenCamera;
    public RenderTexture computerScreenRenderTexture;
    [Range(0, 1)]
    public float zoomInApplyCanvasScalingPercentage;
    private bool appliedScaling;
    private Camera playerCamera;

    [Header("Player Interaction")]
    //public bool UsingComputerScreen;
    public float interactSpeed = 5f;
    public int pointsRequiredToUse;

    [Header("Cooldown")]
    public string interactMessageIfInCooldown;
    public bool IsInCooldown;

    public override void Start()
    {
        base.Start();
        computerCanvasScaler = computerCanvas.GetComponent<CanvasScaler>();
        appliedScaling = false;

        playerCameraFieldOfView = Camera.main.fieldOfView;
        playerHealthComponent = player.GetComponent<HealthComponent>();
        playerAudioSource = player.GetComponent<AudioSource>();
        playerWeaponInventory = player.GetComponentInChildren<PlayerWeaponInventory>();
        playerCamera = Camera.main;

        computerCanvas.CooldownStartedEvent += ActivateCooldown;
        computerCanvas.CooldownEndedEvent += DisableCooldown;
    }

    //Moving player camera to position;
    IEnumerator MoveAndRotatePlayerCameraToPosition(Vector3 targetPosition,Quaternion targetRotation,float targetFieldOfView)
    {
        playerCameraPositionAtStartOfInteraction = Camera.main.transform.position;
        playerCameraRotationAtStartOfInteraction = Camera.main.transform.rotation;
        float percentageComplete = 0;

        while (percentageComplete <= 1)
        {
            if (!appliedScaling && percentageComplete > zoomInApplyCanvasScalingPercentage)
            {
                computerCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                appliedScaling = true;
            }

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
    }
    IEnumerator MoveCameraBackToPlayerPosition()
    {
        computerCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

        //Re-enable the comp screen.
        computerScreenCamera.enabled = true;
        computerScreenCamera.targetTexture = computerScreenRenderTexture;
        playerCamera.enabled = true;

        yield return StartCoroutine(MoveAndRotatePlayerCameraToPosition(playerCameraPositionAtStartOfInteraction, playerCameraRotationAtStartOfInteraction, playerCameraFieldOfView));

        PlayerLookedAwayFromScreenEvent();

        //Reset player state.
        playerInteractRaycast.CheckForInteractableObjects = true;
        gameManager.EnablePlayerMovement();
        playerWeaponInventory.ActivateEquippedWeapon();
        playerHealthComponent.CanTakeDamage = true;

        //Re-enable UI.
        uiBehaviour.EnableUIExceptInteractMessage();
        UIMessageIfPlayerLooksAtObjectNotInteractable = "";

        //Set up scaling again for next interaction.
        appliedScaling = false;
    }

    //Interaction.
    public override void PlayerInteracted()
    {
        if(!IsInCooldown)
        {
            playerData.RemovePoints(pointsRequiredToUse);

            playerInteractRaycast.CheckForInteractableObjects = false;
            UIMessageIfPlayerLooksAtObjectNotInteractable = "";

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

    public override void DetermineIfInteractable()
    {
        if(!IsInCooldown && playerData.points >= pointsRequiredToUse)
            IsInteractable = true;
        else
            IsInteractable = false;
    }

    public void ActivateCooldown()
    {
        IsInteractable = false;
        IsInCooldown = true;
    }
    public void DisableCooldown()
    {
        DetermineIfInteractable();
        IsInCooldown = false;

        uiBehaviour.HidePlayerInteractMessage();
        UIMessageIfPlayerLooksAtObjectNotInteractable = "Not Enough Points - " + pointsRequiredToUse  + " Required";
    }

    //Recontrolling player after missile explodes.
    public void RecontrolPlayer()
    {
        StartCoroutine(MoveCameraBackToPlayerPosition());
    }
}

