using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Base script for enemy that uses navmesh.
[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshEnemy : NavMeshMover
{
    public override void Start ()
    {
        base.Start();
    }
    public override void Awake()
    {
        base.Awake();
    }
}
