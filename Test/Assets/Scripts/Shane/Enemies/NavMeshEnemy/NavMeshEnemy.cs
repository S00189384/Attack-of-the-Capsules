using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
