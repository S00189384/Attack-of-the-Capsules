using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public delegate void ReachedDestinationDelegate();
[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMover : MonoBehaviour
{
    NavMeshHit navMeshHit; 

    public event ReachedDestinationDelegate ReachedDestinationEvent;
    protected NavMeshAgent navMeshAgent;

    [Header("NavMeshInfo")]
    public Vector3 target;
    public bool CanMove;
    public float rangeThreshold = 0.1f;

    public virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    public virtual void Start()
    {

    }

    public virtual void MoveTo(Vector3 targetPosition)
    {
        target = targetPosition;
        navMeshAgent.SetDestination(targetPosition);
    }
    public virtual void MoveTo(GameObject targetObject)
    {
        target = targetObject.transform.position;
        navMeshAgent.SetDestination(targetObject.transform.position);
    }
    public virtual void StopMoving()
    {
        CanMove = false;
        navMeshAgent.isStopped = true;
    }
    public virtual void ResumeMoving()
    {
        CanMove = true;
        navMeshAgent.isStopped = false;
    }
    public void CheckIfReachedDestination()
    {
        if(Vector3.Distance(transform.position,navMeshAgent.destination) <= rangeThreshold)
        {
            ReachedDestinationEvent?.Invoke();
        }
    }
    public virtual void Update()
    {
        if(navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete || navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial)
            CheckIfReachedDestination();
    }
}
