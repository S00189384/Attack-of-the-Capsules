using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base script for enemy that doesn't use navmesh - none in this project so far.
[RequireComponent(typeof(HealthComponent))]
public class Enemy : MonoBehaviour
{
    //Components.
    HealthComponent healthComponent;

	public virtual void Start ()
    {
        healthComponent = GetComponent<HealthComponent>();
	}
	
}
