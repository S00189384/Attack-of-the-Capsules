using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceBuildingCCTV : MonoBehaviour
{
    [Header("Camera Rotates Or Not")]
    public bool Rotates;

    [Header("Target Angle")]
    public float targetYAngle;
    private float amountToRotate;

    [Header("Rotation Movement")]
    public float rotationSpeed;
    public float rotationDelay = 2f;

    void Start()
    {
        if (Rotates)
            StartCoroutine(Rotate());
    }

    //Bad way of doing rotation but I ran out of time and had to move on.
    IEnumerator Rotate()
    {
        amountToRotate = targetYAngle - transform.localEulerAngles.y;

        //Always rotate.
        while (Rotates)
        {
            while (Mathf.Abs(Mathf.DeltaAngle(transform.localEulerAngles.y, targetYAngle)) >= 0.1f)
            {
                transform.localEulerAngles += new Vector3(0, rotationSpeed, 0) * Time.deltaTime;
                yield return null;
            }

            amountToRotate = -amountToRotate;
            targetYAngle += amountToRotate;
            rotationSpeed = -rotationSpeed;

            yield return new WaitForSeconds(rotationDelay);
            yield return null;
        }
    }
}
