using UnityEngine;

//Trap button activates trap and has a points requirement to use.
//When button is pushed it changes a colour above the button to show if its interactable or not, it updates UI and activates trap. 
public class ParticleTrapButton : PlayerInteractableObject
{
    //Components.
    AudioSource audioSource;
    Animator animator;

    [Header("Trap It Controls")]
    public ParticleTrap particleTrap;

    [Header("Points Requirement")]
    public int pointsToUse;

    public GameObject buttonLight;
    public Material buttonInteractableMaterial;
    public Material buttonNotInteractableMaterial;
    private MeshRenderer buttonLightMeshRenderer;

    public override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        buttonLightMeshRenderer = buttonLight.GetComponent<MeshRenderer>();

        particleTrap.CooldownEndedEvent += SwitchInteractableStatus;
    }
    public override void PlayerInteracted()
    {
        if(IsInteractable)
        {
            SwitchInteractableStatus();
            animator.SetBool("Interacted", true);
            particleTrap.Activate();
            playerData.RemovePoints(pointsToUse);
        }
    }
    public override void DetermineIfInteractable()
    {
        if (!particleTrap.IsActive && !particleTrap.IsInCooldown && playerData.points >= pointsToUse)
            IsInteractable = true;
        else
            IsInteractable = false;
    }

    public override void SwitchInteractableStatus()
    {
        if (IsInteractable)
        {
            buttonLightMeshRenderer.material = buttonNotInteractableMaterial;
            IsInteractable = false;
            UIMessageIfPlayerLooksAtObjectNotInteractable = "Trap is active";
            uiBehaviour.ShowPlayerInteractMessage(UIMessageIfPlayerLooksAtObjectNotInteractable, true, Color.red);
        }
        else
        {
            buttonLightMeshRenderer.material = buttonInteractableMaterial;
            if(playerData.points >= pointsToUse)
            {
                IsInteractable = true;
                UIMessageIfPlayerLooksAtObjectNotInteractable = "Press F To Use Trap - " + pointsToUse + " Points";
                uiBehaviour.ShowPlayerInteractMessage(UIMessageIfPlayerLooksAtObjectNotInteractable, true, Color.green);
            }
            else
            {
                UIMessageIfPlayerLooksAtObjectNotInteractable = "Not Enough Points To Use Trap - " + pointsToUse + " Required";
                uiBehaviour.ShowPlayerInteractMessage(UIMessageIfPlayerLooksAtObjectNotInteractable, true, Color.red);
            }
        }

        uiBehaviour.HidePlayerInteractMessage();
    }
    public void PlayInteractNoise()
    {
        audioSource.Play();
    }
}
