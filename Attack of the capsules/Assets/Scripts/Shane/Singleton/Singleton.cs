﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Object doesn't destroy itself on loading scene and ensures there's only one.
//Didn't use it in the project yet.
public class Singleton : MonoBehaviour
{
    void Start()
    {
        SetUpSingleton();
    }
    private void SetUpSingleton()
    {
        if(FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
