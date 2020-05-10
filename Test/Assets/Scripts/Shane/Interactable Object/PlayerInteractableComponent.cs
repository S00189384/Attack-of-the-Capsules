using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerInteractableComponent : MonoBehaviour
{
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
