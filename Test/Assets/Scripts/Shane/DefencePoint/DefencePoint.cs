using UnityEngine;

public class DefencePoint : MonoBehaviour
{
    public DefencePointPlayerBehaviour defencePointPlayerBehaviour;
    UIBehaviour uiBehaviour;
    AudioSource audioSource;

    public AudioClip repairSound;

    [Header("Barricade Numbers")]
    public int TotalNumberOfBarricades;
    public int barricadeIndex = 0;

    [Header("Barricade Objects")]
    public Barricade[] ActiveBarricades;
    public Barricade barricadePrefab;
    public float barricadeDissapearTime = 2;

    [Header("Barricade Movement")]
    public float[] barricadeTargetRotations;
    public Transform[] barricadeMovePositions;
    public Transform barricadeSpawnPosition;
    public float barricadeMoveSpeed = 2;

    private void Start()
    {
        defencePointPlayerBehaviour = GetComponentInChildren<DefencePointPlayerBehaviour>();
        uiBehaviour = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBehaviour>();
        audioSource = GetComponent<AudioSource>();

        //Every window will be fully barricaded at start.
        //-1 if no barricades -> one less than amount of barricades if barricaded at start.
        barricadeIndex = TotalNumberOfBarricades - 1;
    }

    //Testing
    //private void Update()
    //{
    //    // Simulate enemy breaking down a barricade.
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        ActiveBarricades[barricadeIndex].GetComponent<HealthComponent>().ApplyDamage(20);
    //    }
    //}

    public void SpawnBarricade()
    {
        barricadeIndex++;
        Barricade barricadeToSpawn = Instantiate(barricadePrefab, barricadeSpawnPosition.position, Quaternion.identity);
        barricadeToSpawn.defencePoint = GetComponent<DefencePoint>();
        barricadeToSpawn.index = barricadeIndex;

        //To clean up hierarchy a bit by putting barricade spawns inside an empty GO thats a child of the defence point.
        barricadeToSpawn.transform.SetParent(transform.GetChild(transform.childCount - 1),true);

        ActiveBarricades[barricadeIndex] = barricadeToSpawn;
        StartCoroutine(barricadeToSpawn.BarricadeDefencePoint(barricadeMovePositions[barricadeIndex], barricadeTargetRotations[barricadeIndex], barricadeMoveSpeed));
        AudioSource.PlayClipAtPoint(repairSound, barricadeToSpawn.transform.position);
    }

    //Defence Point Status.
    public bool IsFullyBarricaded()
    {
        return barricadeIndex == TotalNumberOfBarricades - 1;
    }
    public bool IsBarricaded()
    {
        return barricadeIndex >= 0;
    }

    //Barricade.
    public Barricade GetFirstBarricade()
    {
        print(ActiveBarricades[TotalNumberOfBarricades - barricadeIndex - 1].name);
        return ActiveBarricades[TotalNumberOfBarricades - barricadeIndex - 1];
    }
    public Barricade GetLastBarricade()
    {
        if(IsBarricaded())
            return ActiveBarricades[barricadeIndex];
        else
            return null;
    }
}
