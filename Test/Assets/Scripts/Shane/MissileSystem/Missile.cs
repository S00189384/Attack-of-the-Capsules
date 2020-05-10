using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void MissileExplodedDelegate();
public class Missile : MonoBehaviour
{
    //Components.
    UIBehaviour uiBehaviour;
    Rigidbody rigidbody;
    MissileComputerCanvas missileComputerCanvas;

    //Events.
    public event MissileExplodedDelegate ExplodedEvent;

    [Header("Missile Control")]
    public bool CanControlMissile;
    public float movementSensitivity;

    [Header("Missile Velocity Downwards")]
    public float initialVelocityDownwards;
    public float boostVelocityDownwards;
    public bool HasAppliedBoost;
    private float horizontalPlayerInput;
    private float verticalPlayerInput;
    private Vector3 playerInputVector;

    [Header("Applying Damage")]
    public LayerMask layersToDealDamageTo;
    public float explosionRadius;
    public float damage;

    [Header("Damage Checking")]
    public LayerMask layersToCheckIfCanDealDamage;
    Vector3 positionOfImpact;
    Ray damageCheckRay;
    RaycastHit hitInfo;

    [Header("Failsafe")]
    public float timeToDestroyAfterSpawning;

    void Start()
    {
        ExplodedEvent += GameObject.FindGameObjectWithTag("MissileScreen").GetComponent<MissileComputerScreen>().RecontrolPlayer;
        missileComputerCanvas = GameObject.FindGameObjectWithTag("MissileScreenCanvas").GetComponent<MissileComputerCanvas>();
        ExplodedEvent += missileComputerCanvas.SwitchToCooldown;
        ExplodedEvent += missileComputerCanvas.PlayAudioWhenMissileHitsGround;

        uiBehaviour = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBehaviour>();
        uiBehaviour.ShowPlayerInteractMessage("Press space to boost", true,Color.white);

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(Vector3.down * initialVelocityDownwards, ForceMode.Impulse);

        Destroy(gameObject, timeToDestroyAfterSpawning);
    }

    void Update()
    {
        if(CanControlMissile)
        {
            horizontalPlayerInput = Input.GetAxis("Mouse X");
            verticalPlayerInput = Input.GetAxis("Mouse Y");

            playerInputVector = new Vector3(verticalPlayerInput * movementSensitivity, 0, -horizontalPlayerInput * movementSensitivity);
            rigidbody.AddForce(playerInputVector);

            if(Input.GetKeyDown(KeyCode.Space) && !HasAppliedBoost)
            {
                HasAppliedBoost = true;
                rigidbody.AddForce(Vector3.down * boostVelocityDownwards,ForceMode.Impulse);
                uiBehaviour.HidePlayerInteractMessage();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        positionOfImpact = collision.GetContact(0).point + new Vector3(0, 1, 0);
        Collider[] collidersHit = Physics.OverlapSphere(positionOfImpact, explosionRadius, layersToDealDamageTo, QueryTriggerInteraction.Ignore);

        //Go through all enemies in explosion radius and check if wall is in the way of applying damage.
        for (int i = 0; i < collidersHit.Length; i++)
        {
            damageCheckRay = new Ray(positionOfImpact, collidersHit[i].gameObject.transform.position - transform.position);
            Debug.DrawRay(damageCheckRay.origin, damageCheckRay.direction * explosionRadius, Color.green, 200);
            if (Physics.Raycast(damageCheckRay, out hitInfo, explosionRadius, layersToCheckIfCanDealDamage, QueryTriggerInteraction.Ignore))
            {
                if (((1 << hitInfo.collider.gameObject.layer) & layersToDealDamageTo) != 0)
                {
                    collidersHit[i].GetComponent<HealthComponent>().ApplyDamage(damage);
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        ExplodedEvent();
    }
}
