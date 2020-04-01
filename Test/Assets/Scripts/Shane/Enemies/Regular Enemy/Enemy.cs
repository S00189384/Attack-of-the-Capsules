using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class Enemy : MonoBehaviour
{
    HealthComponent healthComponent;

	// Use this for initialization
	public virtual void Start ()
    {
        healthComponent = GetComponent<HealthComponent>();
	}
	
}
