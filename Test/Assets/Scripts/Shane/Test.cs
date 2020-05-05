using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Test : MonoBehaviour
{
    //private void Start()
    //{
    //    StaticEditorFlags gg = StaticEditorFlags.BatchingStatic;
    //    GameObjectUtility.SetStaticEditorFlags(gameObject, gg);
    //}

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            StaticEditorFlags gg = StaticEditorFlags.BatchingStatic;
            GameObjectUtility.SetStaticEditorFlags(gameObject, gg);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StaticEditorFlags gg = StaticEditorFlags.NavigationStatic;
            GameObjectUtility.SetStaticEditorFlags(gameObject, gg);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            NavMeshObstacle navMeshObstacle = GetComponent<NavMeshObstacle>();
            navMeshObstacle.carving = false;
        }

    }

}
