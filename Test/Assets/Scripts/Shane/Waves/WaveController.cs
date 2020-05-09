using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class WaveController : MonoBehaviour
{
    System.Random rng = new System.Random();

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
        currentWave = wavesList[currentWaveIndex];

        //Play audio at start - when it ends start to spawn waves.
        uiBehaviour.FadeRoundNumberOnRoundStart();

        //Start of game - fade number UI from transparent to its colour.
        audioSource.clip = startOfGameAudio;
        audioSource.Play();
        yield return new WaitForSeconds(startOfGameAudio.length * transitionAudioWaveStarterOffset);

        StartCoroutine(SpawnAllWaves());
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
        {
            IncreaseDifficultyOfInfiniteWave();
            //wavesList.Clear();
            //wavesList.Add(GenerateWave());
            //currentWave = wavesList[currentWaveIndex];
        }
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
            GameOverUI.gameOverText = "You Win!";

            yield return new WaitForSeconds(endGameFadeToBlackDelay);

            uiBehaviour.FadeToBlackAndLoadScene(endGameFadeToBlackSpeed, 1);
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
        return activeSpawnersList[rng.Next(0, activeSpawnersList.Count)];
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

        if (numberOfEnemiesKilledInWave >= currentWave.numberOfEnemiesInWave)
            allEnemiesKilledInWave = true;

        print("Killed: " + numberOfEnemiesKilledInWave + " ---- Target: " + currentWave.numberOfEnemiesInWave);
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
