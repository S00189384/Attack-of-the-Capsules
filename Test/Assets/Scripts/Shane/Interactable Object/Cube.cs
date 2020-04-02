using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : PlayerInteractableObject
{
    public override void PlayerInteracted()
    {
        GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * 5, ForceMode.Impulse);
    }
}
