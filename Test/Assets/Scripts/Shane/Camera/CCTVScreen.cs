using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CCTVScreen : MonoBehaviour
{
    public string CCTVLocation;
    public int CCTVIndex;
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
