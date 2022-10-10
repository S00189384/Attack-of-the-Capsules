using UnityEngine;

//Interactable object base class - pickable items, missile screen etc. inherit from this.
public class PlayerInteractableObject : PlayerInteractableComponent
{
    [Header("Object Info for UI")]
    public bool IsInteractable = true;
    public string ObjectName;
    public string UIMessageIfPlayerLooksAtObjectInteractable;
    public string UIMessageIfPlayerLooksAtObjectNotInteractable;

    public virtual void Update()
    {

    }
    public virtual void PlayerInteracted()
    {

    }

    public virtual void SwitchInteractableStatus()
    {
        IsInteractable = !IsInteractable;
    }

    public virtual void DetermineIfInteractable()
    {

    }
}
