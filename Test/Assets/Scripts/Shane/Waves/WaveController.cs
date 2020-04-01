using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class WaveController : MonoBehaviour
{
    System.Random rng = new System.Random();

    //Components.
    AudioSource audioSource;

    [Header("Waves")]
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

    [Header("Spawners")]
    public List<Spawner> activeSpawnersList = new List<Spawner>();
    public List<Spawner> allSpawnersList = new List<Spawner>();

    IEnumerator Start()
    {
        audioSource = GetComponent<AudioSource>();

        currentWave = wavesList[currentWaveIndex];

        //Play audio at start - when it ends start to spawn waves.
        yield return BeforeWaveTransition(startOfGameAudio);
        StartCoroutine(SpawnAllWaves());
    }

    IEnumerator SpawnAllWaves()
    {
        while(currentWaveIndex < wavesList.Count)
        {
            yield return SpawnAllEnemiesInWave();
        }
    }

    IEnumerator SpawnAllEnemiesInWave()
    {
        //Spawn all enemies in wave.
        while (currentWave.numberOfEnemiesSpawned < currentWave.numberOfEnemiesInWave)
        {
            //If the chosen random spawner already has max no. of enemies dont spawn anything & wait until next spawn time.
            Spawner randomSpawner = GetRandomSpawner();
            if(!randomSpawner.DefencePointEnemyBehaviour.HasMaxNumberOfEnemies())
            {
                randomSpawner.SpawnEnemy(currentWave.prefabToSpawn);
                currentWave.numberOfEnemiesSpawned++;
            }

            yield return new WaitForSeconds(currentWave.GetNextSpawnTime());
        }

        //Wait until all the enemies are killed.
        while(allEnemiesKilledInWave == false)
        {
            yield return null;
        }

        //Transition to next wave if there is one.
        currentWaveIndex++;
        if (currentWaveIndex < wavesList.Count)
        {
            currentWave = wavesList[currentWaveIndex];
            numberOfEnemiesKilledInWave = 0;
            allEnemiesKilledInWave = false;

            yield return BeforeWaveTransition(transitionBetweenWavesAudio);
        }
        else
        {
            //Fade to black - game / level ends.
            StartCoroutine(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().FadeToBlackAndLoadScene(endGameFadeToBlackDelay,endGameFadeToBlackSpeed,1));
        }
    }

    IEnumerator BeforeWaveTransition(AudioClip transitionAudio)
    {
        //Play audio.
        audioSource.clip = transitionAudio;
        audioSource.Play();

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
        activeSpawnersList.AddRange(allSpawnersList.FindAll(s => s.areaIndex == areaIndex));
    }

    public void IncreaseEnemiesKilledInWave()
    {
        numberOfEnemiesKilledInWave++;
        print("No. of enemies killed = " + numberOfEnemiesKilledInWave);

        if (numberOfEnemiesKilledInWave >= currentWave.numberOfEnemiesInWave)
            allEnemiesKilledInWave = true;
    }


    //Resetting wave stats when game is ended - scriptable object wasn't doing it automatically.
    public void OnDisable()
    {
        for (int i = 0; i < wavesList.Count; i++)
        {
            wavesList[i].numberOfEnemiesSpawned = 0;
        }
    }
}
