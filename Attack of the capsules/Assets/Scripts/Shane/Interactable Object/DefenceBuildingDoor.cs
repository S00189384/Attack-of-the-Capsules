using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Door which blocks path to part of the level. Need points to unlock.
//When unlocked all the spawners in the area that is unlocked gets added to active spawners which enemies can spawn out of.
public delegate void UnlockDoorDelegate(int indexOfAreaUnlocked);
[RequireComponent(typeof(AudioSource))]
public class DefenceBuildingDoor : PlayerInteractableObject
{
    //Components.
    AudioSource audioSource;
    WaveController waveController;

    //Events.
    public UnlockDoorDelegate UnlockDoorEvent;

    [Header("Door")]
    public int indexOfAreaUnlocked;
    public int areaIndexOfSide1;
    public int areaIndexOfSide2;
    public int pointsToUnlock;
    public AudioClip unlockSound;

    [Header("Sides of Door")]
    public BoxCollider sideOfDoorCheck1;
    public BoxCollider sideOfDoorCheck2;

    public override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        waveController = GameObject.FindGameObjectWithTag("WaveController").GetComponent<WaveController>();
        UnlockDoorEvent += waveController.AddSpawnersToActiveSpawnerList;      
    }

    public override void PlayerInteracted()
    {
        if(playerData.points >= pointsToUnlock)
        {
            playerData.RemovePoints(pointsToUnlock);

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

    public override void DetermineIfInteractable()
    {
        if (playerData.points >= pointsToUnlock)
            IsInteractable = true;
        else
            IsInteractable = false;
    }
}
