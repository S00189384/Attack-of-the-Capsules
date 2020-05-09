using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodedBarrelDebris : MonoBehaviour
{
    [Header("Fade Over Time")]
    public float timeToDestroy;
    //public float objectFadeSpeed;

    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }

    //IEnumerator FadeObject()
    //{
    //    Material objectMaterial = GetComponent<MeshRenderer>().material;
    //    Color originalColour = objectMaterial.color;
    //    Color targetColour = new Color(originalColour.r, originalColour.g, originalColour.b, 0);
    //    float percentageComplete = 0;

    //    while (percentageComplete <= 1)
    //    {
    //        percentageComplete += Time.deltaTime * objectFadeSpeed;
    //        objectMaterial.color = Color.Lerp(originalColour, targetColour, percentageComplete);
    //        yield return null;
    //    }

    //    Destroy(gameObject);
    //}
}
