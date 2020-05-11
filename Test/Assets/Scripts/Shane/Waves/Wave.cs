using UnityEngine;

//Scriptable object that represents a wave. 
//Wave controller has a list of this object.
[CreateAssetMenu(fileName = "Wave", menuName = "Scriptable Object- Wave")]
public class Wave : ScriptableObject
{
    [Header("Prefab/s to Spawn")]
    public GameObject prefabToSpawn;

    [Header("Number of Enemies")]
    public int numberOfEnemiesInWave;
    public int numberOfEnemiesSpawned;

    [Header("Spawn Timings")]
    public float minTimeBetweenEnemySpawns;
    public float maxTimeBetweenEnemySpawns;
    public float GetNextSpawnTime()
    {
        return Random.Range(minTimeBetweenEnemySpawns, maxTimeBetweenEnemySpawns);
    }
}
