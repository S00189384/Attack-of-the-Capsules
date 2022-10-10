using System;
using System.Collections;
using TMPro;
using UnityEngine;


//CCTV Screen on control panel displays the current time and location of where the camera is showing.
public class CCTVScreen : MonoBehaviour
{
    [Header("CCTV Info")]
    public string CCTVLocation;
    public int CCTVIndex;

    [Header("Text Mesh Pro's")]
    public TextMeshPro dateTimeText;
    public TextMeshPro cameraLocationText;

    private void Start()
    {
        cameraLocationText.text = "Cam " + CCTVIndex + " - " + CCTVLocation;
        StartCoroutine(UpdateDateTime());
    }

    IEnumerator UpdateDateTime()
    {
        while (true)
        {
            dateTimeText.text = DateTime.UtcNow.ToString();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
