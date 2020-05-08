using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/*  Enemy is assigned a "defence point" at spawn. If the enemy is in the first row at the defence point they attack the barricade or the player if the player gets too close.
 *  If there are enemies waiting not in the front row and another enemy in the front row dies, a random enemy in the back row is chosen to move up and attack the barricade / player.
 *  Once the defence point has no barricades the enemies chase the player - their navmeshmask changes so that once they enter the building they cant leave.
 *  When chasing they check if there is obstacles between them and the player which prevents them from attacking through walls.
 *  Their attack is a dash towards the player after a certain amount of time.  
 */

public delegate void LeftDefencePoint(BreakInEnemy enemy);
public class BreakInEnemy : NavMeshEnemy
{
    System.Random rng = new System.Random();
    
    //Components.
    WaveController waveController;
    DefencePointEnemyBehaviour DefencePointEnemyBehaviour;
    DefencePointPlayerBehaviour DefencePointPlayerBehaviour;
    HealthComponent healthComponent;
    AudioSource audioSource;

    //Events.
    public event LeftDefencePoint LeftDefencePointEvent;

    //Variables.
    [Header("Defence Point Info")]
    public DefencePoint defencePoint;
    public int DefencePointIndex;

    [Header("Status Info")]
    public bool IsAtDefencePoint;
    public bool IsAtDefencePointFrontRow;
    public bool ChasingPlayer;

    [Header("Attack Info")]
    GameObject player;
    public GameObject attackTarget;
    public bool CanAttack;
    public float distanceToStopFromPlayer = 2;
    public float distanceToAttack = 3;

    [Header("Dash Attack")]
    public float damage = 20;
    public float attackSpeed = 2;
    public float attackTimer = 0;
    public float timeBetweenAttacks = 2;
    [Tooltip("How much the enemy should be offset from the target. 0 = exactly targets position / 0.5 slightly away from the target")]
    [Range(0,0.5f)]
    public float dashAttackOffset;
    public bool CheckForPlayerCollision;
    public AudioClip attackSound;

    [Header("Dash Attack Checking if possible")]
    [Tooltip("Which objects if in between enemy and player prevent the enemy from doing dash attack")]
    public LayerMask layersToCheckIfDashAttackPossible;
    RaycastHit preventDashAttackHitCheck;

    [Header("Appearance")]
    public Material materialOfEnemy;
    public List<Material> possibleMaterials;

    [Header("Death")]
    public ParticleSystem deathParticleEffect;
    public Vector3 directionRaycastHit;

    [Header("Chasing Player")]
    [Tooltip("Name of area mask that's inside building that the enemy will switch to once inside so it can't leave")]
    public string navMeshAreaMaskInsideBuilding;
    int areaIndexMaskInsideBuilding;

    //Start.
    public override void Awake()
    {
        base.Awake();       
    }
    public override void Start ()
    {
        base.Start();

        //Components.
        healthComponent = GetComponent<HealthComponent>();
        audioSource = GetComponent<AudioSource>();
        DefencePointEnemyBehaviour = defencePoint.GetComponentInChildren<DefencePointEnemyBehaviour>();
        DefencePointPlayerBehaviour = defencePoint.GetComponentInChildren<DefencePointPlayerBehaviour>();

        player = GameObject.FindGameObjectWithTag("Player");
        waveController = GameObject.FindGameObjectWithTag("WaveController").GetComponent<WaveController>();

        //Subscribing events.
        player.GetComponent<HealthComponent>().OnDeathEvent += DisableAttack;
        healthComponent.OnDeathEvent += OnDeath;
        ReachedDestinationEvent += CheckIfReachedFrontRow;

        if(defencePoint.IsBarricaded())
        {
            DefencePointPlayerBehaviour.PlayerEnteredDefencePointArea += TargetPlayer;
            DefencePointPlayerBehaviour.PlayerLeftDefencePointArea += TargetBarricade;
        }

        //Assigning material of enemy.
        materialOfEnemy = possibleMaterials[rng.Next(0, possibleMaterials.Count)];
        GetComponent<MeshRenderer>().material = materialOfEnemy;
    }	

    //If can attack, attack target.
	public override void Update ()
    {
        base.Update();

        //if(Input.GetKeyDown(KeyCode.L))
        //{
        //    navMeshAgent.areaMask = 1 << 5;
        //}

        if(CanAttack)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer >= timeBetweenAttacks)
            {
                if(!ChasingPlayer)
                    DetermineAttackTargetAtDefencePoint();

                StartCoroutine(AttackTarget());
                attackTimer = 0;
            }
        }
    }

    //Reaching defence point front row.
    public void CheckIfReachedFrontRow()
    {
        if (IsAtDefencePoint && DefencePointIndex >= 0 && DefencePointIndex < DefencePointEnemyBehaviour.numberOfEnemiesInFirstRow)
        {
            IsAtDefencePointFrontRow = true;
            ReachedDestinationEvent -= CheckIfReachedFrontRow; //Unsubscribing as once the enemy reaches front row he's not going to go back to back row. 

            CanAttack = true;
            CheckIfCanChasePlayer();

            if(!ChasingPlayer)
                DetermineAttackTargetAtDefencePoint();
        }
    }

    //Switch attack target.
    public void DetermineAttackTargetAtDefencePoint()
    {
        if (defencePoint.defencePointPlayerBehaviour.PlayerInRange)
            TargetPlayer();

        else if (defencePoint.IsBarricaded())
            TargetBarricade();
    }
    public void TargetPlayer()
    {
        attackTarget = player;
    }
    public void TargetBarricade()
    {
        attackTarget = defencePoint.GetLastBarricade().gameObject;
    }
    public void DisableAttack()
    {
        CanAttack = false;
    }

    //General Attack.
    IEnumerator AttackTarget()
    {
        if (CanAttack)
        {
            StopMoving();

            //Lerps to target - melee attack.
            //After attack -check if barricade if fully destroyed.If it is target player. 

            //Check if barricade still exists as other enemy might have destroyed it.
            if (attackTarget != null)
            {
                CheckForPlayerCollision = true;
                bool HasAppliedDamage = false;
                Vector3 startingPosition = transform.position;
                Vector3 directionToTarget = (attackTarget.transform.position - transform.position).normalized;
                Vector3 positionToDashTo = attackTarget.transform.position - (directionToTarget * dashAttackOffset);

                float percentageComplete = 0;
                while (percentageComplete <= 1)
                {
                    percentageComplete += Time.deltaTime * attackSpeed;
                    float interpolation = (-(percentageComplete * percentageComplete) + percentageComplete) * 4;
                    transform.position = Vector3.Lerp(startingPosition, positionToDashTo, interpolation);

                    if (interpolation >= 0.5f & !HasAppliedDamage)
                    {
                        audioSource.PlayOneShot(attackSound);

                        HasAppliedDamage = true;
                        if (attackTarget.gameObject != null && attackTarget.gameObject.tag == "Barricade")
                        {
                            if(defencePoint.IsBarricaded())
                            {
                                HealthComponent healthComponentOfBarricade = defencePoint.GetLastBarricade().GetComponent<HealthComponent>();
                                if (healthComponentOfBarricade != null)
                                    healthComponentOfBarricade.ApplyDamage(damage);
                            }
                        }
                    }

                    yield return null;
                }
            }

            CheckIfCanChasePlayer();
            ResumeMoving();
        }       
    }

    //Chasing & Attacking Player Behaviour.
    private void CheckIfCanChasePlayer()
    {
        if (IsAtDefencePointFrontRow && !defencePoint.IsBarricaded())
        {
            StartCoroutine(ChasePlayer());
        }
    }
    private bool ObjectInWayOfEnemyAndPlayer()
    {
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out preventDashAttackHitCheck, distanceToAttack,layersToCheckIfDashAttackPossible,QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(transform.position, (player.transform.position - transform.position) * 20, Color.red, 20);
            if (preventDashAttackHitCheck.collider.tag != "Player")
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator ChasePlayer()
    {
        areaIndexMaskInsideBuilding =  NavMesh.GetAreaFromName(navMeshAreaMaskInsideBuilding);
        navMeshAgent.areaMask = 1 << areaIndexMaskInsideBuilding;

        //Unsubscribe events.
        UnsubscribeAttackTargetEvents();

        //Switch attention to chasing player.
        ChasingPlayer = true;
        TargetPlayer();

        //Leave defence point.
        LeftDefencePointEvent(this);
        IsAtDefencePoint = false;
        IsAtDefencePointFrontRow = false;

        //Can't attack temporarily but will check distance to player soon to see if it can attack.
        CanAttack = false;

        //Move to player and determine if can attack.
        navMeshAgent.stoppingDistance = distanceToStopFromPlayer;
        float refreshTime = 0.2f;

        while (true)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= distanceToAttack && !ObjectInWayOfEnemyAndPlayer())
                CanAttack = true;

            else
                CanAttack = false;

            MoveTo(player.transform.position);
            yield return new WaitForSeconds(refreshTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag =="Player" && CheckForPlayerCollision)
        {
            HealthComponent healthComponentOfPlayer = other.gameObject.GetComponent<HealthComponent>();

            if(healthComponentOfPlayer.IsAlive)
                healthComponentOfPlayer.ApplyDamage(damage);

            CheckForPlayerCollision = false;
        }
    }

    //Death & Removal.
    private void UnsubscribeAttackTargetEvents()
    {
        DefencePointPlayerBehaviour.PlayerEnteredDefencePointArea -= TargetPlayer;
        DefencePointPlayerBehaviour.PlayerLeftDefencePointArea -= TargetBarricade;
    }
    public void OnDeath()
    {
        //Particle effect - spawning, setting colour, destroying.
        ParticleSystem particleSystem = Instantiate(deathParticleEffect,transform.position,Quaternion.FromToRotation(Vector3.forward,healthComponent.directionOfHit));
        ParticleSystem.MainModule psMainModule = particleSystem.main;
        psMainModule.startColor = materialOfEnemy.color;
        Destroy(particleSystem.gameObject, particleSystem.main.startLifetimeMultiplier);


        waveController.IncreaseEnemiesKilledInWave();
        UnsubscribeAttackTargetEvents();

        if (!ChasingPlayer)
            LeftDefencePointEvent(this);
    }
}
