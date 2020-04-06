using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileComputerScreen : PlayerInteractableObject
{
    //Components.
    public GameManager gameManager;
    public MissileComputerCanvas computerCanvas;
    HealthComponent playerHealthComponent;

    [Header("Computer Camera")]
    public Camera computerScreenCamera;
    public RenderTexture computerScreenRenderTexture;
    private Camera playerCamera;

    [Header("Player Interaction")]
    public float interactSpeed = 5f;

    //Capturing player camera info when they interact.
    private Vector3 playerCameraPositionAtStartOfInteraction;
    private Quaternion playerCameraRotationAtStartOfInteraction;
    private float playerCameraFieldOfView;

    [Header("Cooldown & Status")]
    public bool IsInCooldown;
    public float coolDownCountdownTime = 30f;
    public float coolDownTimer;

    public override void Start()
    {
        base.Start();
        playerCameraFieldOfView = Camera.main.fieldOfView;
        playerHealthComponent = player.GetComponent<HealthComponent>();
        playerCamera = Camera.main;
    }

    public override void Update()
    {
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
    }
    IEnumerator MoveCameraBackToPlayerPosition()
    {
        playerCamera.enabled = true;

        yield return StartCoroutine(MoveAndRotatePlayerCameraToPosition(playerCameraPositionAtStartOfInteraction, playerCameraRotationAtStartOfInteraction, playerCamera.fieldOfView));

        computerScreenCamera.targetTexture = computerScreenRenderTexture;
        gameManager.EnablePlayerMovement();
        playerHealthComponent.CanTakeDamage = true;
        uiBehaviour.EnableUI();
    }

    //Interaction.
    public override void PlayerInteracted()
    {
        if(!IsInCooldown)
        {
            //Reset UI.
            uiBehaviour.DisableUI();

            //Reset computer screen text.
            computerCanvas.ResetMissileStatusFade();
            computerCanvas.SetMissileStatusColour(computerCanvas.missileStatusAvailableColour);

            //Disable movement & make player not take damage.
            gameManager.DisablePlayerMovement();
            playerHealthComponent.CanTakeDamage = false;

            //Move camera to position.
            //StartCoroutine(MoveAndRotatePlayerCameraToPosition(computerScreenCamera.transform.position,computerScreenCamera.transform.rotation,computerScreenCamera.fieldOfView));
            StartCoroutine(MoveCameraToComputerScreen());
        }
    }

    //Cooldown.
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
    public void ResetCooldownTimer()
    {
        coolDownTimer = coolDownCountdownTime;
    }
}
