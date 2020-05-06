using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public delegate void UnlockDoorDelegate(int indexOfAreaUnlocked);
[RequireComponent(typeof(AudioSource))]
public class DefenceBuildingDoor : PlayerInteractableObject
{
    //Components.
    AudioSource audioSource;
    WaveController waveController;

    [Header("Door")]
    public int indexOfAreaUnlocked;
    public int areaIndexOfSide1;
    public int areaIndexOfSide2;
    public int pointsToUnlock;
    public AudioClip unlockSound;

    public BoxCollider sideOfDoorCheck1;
    public BoxCollider sideOfDoorCheck2;

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
        if (sideOfDoorCheck1.bounds.Contains(player.transform.position))
            indexOfAreaUnlocked = areaIndexOfSide2;
        else if (sideOfDoorCheck2.bounds.Contains(player.transform.position))
            indexOfAreaUnlocked = areaIndexOfSide1;

        UnlockDoorEvent(indexOfAreaUnlocked);
        Destroy(gameObject);
        uiBehaviour.HidePlayerInteractMessage();

        GetComponent<NavMeshObstacle>().carving = false;
    }
}
