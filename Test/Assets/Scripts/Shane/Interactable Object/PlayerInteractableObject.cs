using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractableObject : PlayerInteractableComponent
{
    [Header("Object Info for UI")]
    public bool IsInteractable = true;
    public string ObjectName;
    public string UIMessageToShowIfPlayerLooksAtObject;

    public virtual void Update()
    {

    }
    public virtual void PlayerInteracted()
    {

    }
}
