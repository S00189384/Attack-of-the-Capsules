using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for interactable area. Checks if player is in range and wants to interact.
//Logic in update is bad, need to fix.
public class PlayerInteractableArea : PlayerInteractableComponent
{
    [Header("Player Status")]
    public bool PlayerInRange;
    public bool PlayerIsInteracting;
    public bool PlayerInteractedOnce;

    public override void Start()
    {
        base.Start();
    }

    public virtual void Update()
    {
        if(PlayerInRange)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                PlayerInteractedOnce = true;
            }

            if(Input.GetKey(KeyCode.F))
            {
                PlayerIsInteracting = true;
            }

            if(Input.GetKeyUp(KeyCode.F))
            {
                PlayerIsInteracting = false;
                PlayerInteractedOnce = false;
            }
        }

    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            PlayerInRange = true;
        }
    }
    public virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            PlayerInRange = false;
            PlayerIsInteracting = false;
            PlayerInteractedOnce = false;
        }
    }
}
