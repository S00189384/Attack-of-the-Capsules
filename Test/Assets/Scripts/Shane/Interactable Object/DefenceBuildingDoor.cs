using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void UnlockDoorDelegate(int indexOfAreaUnlocked);
[RequireComponent(typeof(AudioSource))]
public class DefenceBuildingDoor : PlayerInteractableObject
{
    //Components.
    AudioSource audioSource;
    WaveController waveController;

    [Header("Door")]
    public int indexOfAreaUnlocked;
    public int pointsToUnlock;
    public AudioClip unlockSound;

    public UnlockDoorDelegate UnlockDoorEvent;

    public override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        waveController = GameObject.FindGameObjectWithTag("WaveController").GetComponent<WaveController>();
        UnlockDoorEvent += waveController.AddSpawnersToActiveSpawnerList;      
    }

    public override void PlayerInteracted()
    {
        //Get which side the player interacted and determine which area index they opened.
        print(playerInteractRaycast.hitInfo.normal);
        print(-transform.forward);




        UnlockDoorEvent(indexOfAreaUnlocked);

        print("unlocked door");
        Destroy(gameObject);
        //if player has enough points
        //destroy / unlock door
        uiBehaviour.HidePlayerInteractMessage();


    }



}
