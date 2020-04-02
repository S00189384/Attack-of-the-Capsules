﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractRaycast : MonoBehaviour
{
    GameManager gameManager;
    UIBehaviour uiBehaviour;
    PlayerInteractableObject interactableObject;
    PlayerInteractableArea interactableArea;
    PlayerGunAttack playerGunAttack;

    public Ray ray;
    public RaycastHit hitInfo;

    public bool Interacted = false;

    public LayerMask layerMask;
    public float interactDistance = 200;

    [Header("Looking at interactable")]
    public Color aimDotColorWhenLooking = Color.green;
    public float aimDotFadeSpeed = 2;
    public bool IsLookingAtInteractableObject;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        uiBehaviour = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBehaviour>();
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
                if (Input.GetKeyDown(KeyCode.F))
                {
                    interactableObject.PlayerInteracted();
                    uiBehaviour.ResetAimDotColour();
                    IsLookingAtInteractableObject = false;
                }
            }
        }
    }

    //Inside interactable area? Check if player looks at interactable object.
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "InteractableArea")
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hitInfo, interactDistance, layerMask, QueryTriggerInteraction.Collide))
            {
                if (interactableObject == null || hitInfo.collider.gameObject != interactableObject.gameObject)
                    interactableObject = hitInfo.collider.GetComponent<PlayerInteractableObject>();

                if(interactableObject != null)
                {
                    if (interactableObject.IsInteractable && !playerGunAttack.IsAiming)
                    {
                        IsLookingAtInteractableObject = true;

                        //Change UI stuff.
                        uiBehaviour.ShowPlayerInteractMessage(interactableObject.UIMessageToShowIfPlayerLooksAtObject, true);

                        if (uiBehaviour._imgPlayerAimDot.color != aimDotColorWhenLooking)
                            uiBehaviour.FadeAimDotColour(aimDotFadeSpeed, aimDotColorWhenLooking);
                    }
                }
            }
            else //Raycast not hitting interactable object.
            {

                if (interactableObject != null)
                {
                    interactableObject = null;
                    uiBehaviour.HidePlayerInteractMessage();
                }

                IsLookingAtInteractableObject = false;

                if (uiBehaviour._imgPlayerAimDot.color != uiBehaviour.aimDotOriginalColour && !playerGunAttack.IsAiming)
                    uiBehaviour._imgPlayerAimDot.color = Color.Lerp(uiBehaviour._imgPlayerAimDot.color, uiBehaviour.aimDotOriginalColour, aimDotFadeSpeed * Time.deltaTime);
            }
        }
    }

    //Leaves interactable area trigger? Reset UI if on screen
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "InteractableArea")
        {
            if (interactableObject != null)
            {
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
