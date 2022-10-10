using UnityEngine;

//Base class for interactable objects.
//Interactable objects often need reference to UI, player + player interact raycast so its good to have a base class that has this information.
[RequireComponent(typeof(Collider))]
public class PlayerInteractableComponent : MonoBehaviour
{
    //Components.
    protected UIBehaviour uiBehaviour;
    protected PlayerInteractRaycast playerInteractRaycast;
    protected PlayerData playerData;

    //Detecting player.
    protected GameObject player;

    public virtual void Start()
    {
        uiBehaviour = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBehaviour>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerInteractRaycast = player.GetComponent<PlayerInteractRaycast>();
        playerData = player.GetComponent<PlayerData>();
    }
}
