﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

//Waves are just like Cod5 Nazi zombies.
//Audio plays at start of game, when audio ends the waves start.
//Wave controller waits until all enemies are spawned, then it waits for the player to kill all enemies.
//When all enemies are killed it goes to next wave - next wave in wave list or it updates the single infinite wave object to get harder if the player chose infinite wave setting.
//Between each wave there is an audio transition, when the transition ends the wave starts.
[RequireComponent(typeof(AudioSource))]
public class WaveController : MonoBehaviour
{
    //Components.
    GameManager gameManager;
    AudioSource audioSource;
    UIBehaviour uiBehaviour;

    [Header("Waves")]
    public int waveNumber = 1;
    public int currentWaveIndex;
    public Wave currentWave;
    public List<Wave> wavesList = new List<Wave>();

    public bool allEnemiesKilledInWave = false;
    public int numberOfEnemiesKilledInWave;
    public int totalNumberOfEnemiesKilled;

    [Header("Wave Transitions")]
    [Tooltip("How far into the transition audio do you want the wave to start? 0 - at the start. 1 at the end.")]
    [Range(0, 1)]
    public float transitionAudioWaveStarterOffset;
    public AudioClip startOfGameAudio;
    public AudioClip transitionBetweenWavesAudio;

    [Header("End of Waves - End Game")]
    public float endGameFadeToBlackSpeed = 0.5f;
    public float endGameFadeToBlackDelay = 2f;
    public float delayBeforeDisablingPlayerControl;

    [Header("Spawners")]
    public List<Spawner> activeSpawnersList = new List<Spawner>();
    public List<Spawner> allSpawnersList = new List<Spawner>();

    [Header("Infinite Waves (Potentially)")]
    public GameObject prefabToSpawn;
    public Wave infiniteWaveToStartFrom;
    private int infiniteWaveStartingNumberEnemiesInWave;
    public int minNumberOfEnemiesToAddToEachWave;
    public int maxNumberOfEnemiesToAddToEachWave;
    public float minTimeToReduceForMinTimeBetweenEnemySpawns;
    public float maxTimeToReduceForMaxTimeBetweenEnemySpawns;

    IEnumerator Start()
    {
        infiniteWaveStartingNumberEnemiesInWave = infiniteWaveToStartFrom.numberOfEnemiesInWave;

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
        uiBehaviour = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBehaviour>();
        HealthComponent playerHealthComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthComponent>();

        playerHealthComponent.OnDeathEvent += OnPlayerDeath;


        currentWave = wavesList[currentWaveIndex];

        //Play audio at start - when it ends start to spawn waves.
        uiBehaviour.FadeRoundNumberOnRoundStart();

        //Start of game - fade number UI from transparent to its colour.
        audioSource.clip = startOfGameAudio;
        audioSource.Play();
        yield return new WaitForSeconds(startOfGameAudio.length * transitionAudioWaveStarterOffset);

        StartCoroutine(SpawnAllWaves());
    }

    private void OnPlayerDeath()
    {
        GameOverUI.SetGameOverText("You died!", waveNumber - 1, totalNumberOfEnemiesKilled);
    }

    IEnumerator SpawnAllWaves()
    {
        if(Settings.InfiniteWaves)
        {
            wavesList.Clear();
            wavesList.Add(infiniteWaveToStartFrom);
            currentWave = infiniteWaveToStartFrom;

            while(true)
            {
                yield return SpawnAllEnemiesInWave();
            }
        }
        else
        {
            while (currentWaveIndex < wavesList.Count)
            {
                yield return SpawnAllEnemiesInWave();
            }
        }
    }
    IEnumerator SpawnAllEnemiesInWave()
    {
        while (currentWave.numberOfEnemiesSpawned < currentWave.numberOfEnemiesInWave)
        {
            //If the chosen random spawner already has max no. of enemies dont spawn anything & wait until next spawn time.
            Spawner randomSpawner = GetRandomSpawner();
            if (!randomSpawner.DefencePointEnemyBehaviour.HasMaxNumberOfEnemies())
            {
                randomSpawner.SpawnEnemy(currentWave.prefabToSpawn);
                currentWave.numberOfEnemiesSpawned++;
            }

            yield return new WaitForSeconds(currentWave.GetNextSpawnTime());
        }

        //Wait until all the enemies are killed.
        while (allEnemiesKilledInWave == false)
        {
            yield return null;
        }

        //If infinite waves is active - I want to keep the list of waves always at 1 and not to add lots to it.
        //Clear list and add a wave that is generated using a simple algorithm (increases difficulty as waves get higher).
        //If not infinite waves, increase index and check if its the last wave.
        if(Settings.InfiniteWaves)
            IncreaseDifficultyOfInfiniteWave();
        else
            currentWaveIndex++;

        if(currentWaveIndex < wavesList.Count)
        {
            waveNumber++;
            currentWave = wavesList[currentWaveIndex];
            numberOfEnemiesKilledInWave = 0;
            allEnemiesKilledInWave = false;
            yield return BeforeWaveTransition(transitionBetweenWavesAudio);
        }
        else
        {
            GameOverUI.SetGameOverText("You Survived!", waveNumber - 1, totalNumberOfEnemiesKilled);

            yield return new WaitForSeconds(endGameFadeToBlackDelay);

            uiBehaviour.FadeToBlackAndLoadScene(endGameFadeToBlackSpeed, 2);
            StartCoroutine(uiBehaviour.FadeUI());
            gameManager.DisablePlayerMovement();
        }      
    }
    IEnumerator BeforeWaveTransition(AudioClip transitionAudio)
    {
        //Play audio.
        audioSource.clip = transitionAudio;
        audioSource.Play();

        StartCoroutine(uiBehaviour.UpdateRoundNumberOnRoundTransition(waveNumber));

        //Wait until audio ends.
        yield return new WaitForSeconds(transitionAudio.length * transitionAudioWaveStarterOffset);
    }
    public Spawner GetRandomSpawner()
    {
        return activeSpawnersList[Random.Range(0, activeSpawnersList.Count)];
    }

    //When player opens door to new area - spawners in that area become active.
    public void AddSpawnersToActiveSpawnerList(int areaIndex)
    {
        if (activeSpawnersList.Any(s => s.areaIndex == areaIndex))
            return;

        activeSpawnersList.AddRange(allSpawnersList.FindAll(s => s.areaIndex == areaIndex));
    }

    public void IncreaseEnemiesKilledInWave()
    {
        numberOfEnemiesKilledInWave++;
        totalNumberOfEnemiesKilled++;

        if (numberOfEnemiesKilledInWave >= currentWave.numberOfEnemiesInWave)
            allEnemiesKilledInWave = true;
    }

    //Resetting wave stats when game is ended - scriptable object wasn't doing it automatically.
    public void OnDisable()
    {
        infiniteWaveToStartFrom.numberOfEnemiesSpawned = 0;
        infiniteWaveToStartFrom.numberOfEnemiesInWave = infiniteWaveStartingNumberEnemiesInWave;

        for (int i = 0; i < wavesList.Count; i++)
        {
            wavesList[i].numberOfEnemiesSpawned = 0;
        }
    }

    private void IncreaseDifficultyOfInfiniteWave()
    {
        infiniteWaveToStartFrom.numberOfEnemiesSpawned = 0;
        infiniteWaveToStartFrom.numberOfEnemiesInWave += Random.Range(minNumberOfEnemiesToAddToEachWave, maxNumberOfEnemiesToAddToEachWave);
    }
}