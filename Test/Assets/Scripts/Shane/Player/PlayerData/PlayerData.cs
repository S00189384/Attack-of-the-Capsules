using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    //Components.
    UIBehaviour uiBehaviour;

    public int points;

    void Start()
    {
        uiBehaviour = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBehaviour>();
    }

    public void AddPoints(int pointsToAdd)
    {
        points += pointsToAdd;
        //Update UI.
    }
    public void RemovePoints(int pointsToRemove)
    {
        points -= pointsToRemove;
        //Update UI.
    }
}
