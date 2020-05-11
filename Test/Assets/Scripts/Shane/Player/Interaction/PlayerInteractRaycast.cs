using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractRaycast : MonoBehaviour
{
    //Components.
    GameManager gameManager;
    UIBehaviour uiBehaviour;
    public PlayerInteractableObject interactableObject;
    PlayerInteractableArea interactableArea;
    PlayerCameraRotation playerCameraComponent;
    PlayerGunAttack playerGunAttack;

    public Ray ray;
    public RaycastHit hitInfo;
    private bool CheckForRaycastLeavingInteractableObject;

    public bool Interacted = false;

    public LayerMask layerMask;
    public float interactDistance = 200;

    [Header("Looking at interactable")]
    public Color aimDotColorWhenLookingAtObjectInteractable = Color.green;
    public Color aimDotColorWhenLookingAtObjectNotInteractable = Color.red;
    public float aimDotFadeSpeed = 2;
    public bool IsLookingAtInteractableObject;
    public bool CheckForInteractableObjects;
    private bool LookingAtObjectInteractableCheck = true;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        uiBehaviour = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBehaviour>();

        playerCameraComponent = GetComponentInChildren<PlayerCameraRotation>();
        playerGunAttack = GetComponentInChildren<PlayerGunAttack>();
    }
    private void Update()
    {
        if(gameManager.CanControlPlayer)
        {
            //Putting this in update as checking for player input in ontriggerstay was giving issues (not registering button presses / method firing twice etc.)
            if (IsLookingAtInteractableObject && interactableObject != null)
            {
                //Check for interaction.
                if (Input.GetKeyDown(KeyCode.F) && interactableObject.IsInteractable)
                {
                    IsLookingAtInteractableObject = false;
                    interactableObject.PlayerInteracted();
                    LookingAtObjectInteractableCheck = true;
                    uiBehaviour.ResetAimDotColour();
                }
            }
        }
    }

    //Inside interactable area? Check if player looks at interactable object.
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "InteractableArea")
        {
            if (playerCameraComponent.playerCamera.enabled)
            {
                if(CheckForInteractableObjects)
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hitInfo, interactDistance, layerMask, QueryTriggerInteraction.Collide)) //Looks at interactable.
                    {
                        if (!CheckForRaycastLeavingInteractableObject)
                            CheckForRaycastLeavingInteractableObject = true;

                        if (interactableObject == null || hitInfo.collider.gameObject != interactableObject.gameObject)
                            interactableObject = hitInfo.collider.GetComponent<PlayerInteractableObject>();

                        if (interactableObject != null)
                        {
                            if (LookingAtObjectInteractableCheck)
                            {
                                interactableObject.DetermineIfInteractable();
                                LookingAtObjectInteractableCheck = false;
                            }

                            if (!playerGunAttack.IsAiming)
                            {
                                IsLookingAtInteractableObject = true;

                                if (interactableObject.IsInteractable)
                                {
                                    uiBehaviour.ShowPlayerInteractMessage(interactableObject.UIMessageIfPlayerLooksAtObjectInteractable, true, Color.green);
                                    if (uiBehaviour._imgPlayerAimDot.color != aimDotColorWhenLookingAtObjectInteractable)
                                        uiBehaviour.FadeAimDotColour(aimDotFadeSpeed, aimDotColorWhenLookingAtObjectInteractable);
                                }
                                else
                                {
                                    uiBehaviour.ShowPlayerInteractMessage(interactableObject.UIMessageIfPlayerLooksAtObjectNotInteractable, true, Color.red);
                                    if (uiBehaviour._imgPlayerAimDot.color != aimDotColorWhenLookingAtObjectNotInteractable)
                                        uiBehaviour.FadeAimDotColour(aimDotFadeSpeed, aimDotColorWhenLookingAtObjectNotInteractable);
                                }
                            }
                        }
                    }
                    else //Looks away from interactable.
                    {
                        if (interactableObject != null)
                            interactableObject = null;

                        if (LookingAtObjectInteractableCheck == false)
                        {
                            LookingAtObjectInteractableCheck = true;
                        }
                    }
                }              
            }
        }

            //Checkforleave bool makes below code execute once - when the raycast leaves the object. 
            if (CheckForRaycastLeavingInteractableObject && interactableObject == null)
            {
                CheckForRaycastLeavingInteractableObject = false;
                uiBehaviour.HidePlayerInteractMessage();
                IsLookingAtInteractableObject = false;

                if (uiBehaviour._imgPlayerAimDot.color != uiBehaviour.aimDotOriginalColour && !playerGunAttack.IsAiming)
                    uiBehaviour._imgPlayerAimDot.color = uiBehaviour.aimDotOriginalColour;
            }
    }

    //Leaves interactable area trigger? Reset UI if on screen
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "InteractableArea")
        {
            if (interactableObject != null)
            {
                IsLookingAtInteractableObject = false;
                uiBehaviour.HidePlayerInteractMessage();
                uiBehaviour.ResetAimDotColour();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "InteractableArea")
        {
            interactableArea = other.GetComponent<PlayerInteractableArea>();
        }
    }
}
