using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractableArea : PlayerInteractableComponent
{
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
