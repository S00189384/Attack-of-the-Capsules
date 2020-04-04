using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissileComputerCanvas : MonoBehaviour
{
    public MissileComputerScreen computerScreen;

    [Header("On Screen UI")]
    public TextMeshProUGUI missileStatusText;
    public bool FadeMissileStatusText;
    public float missileFadeSpeed;
    public Color missileStatusAvailableColour;
    public Color missileStatusNotAvailableColour;
    private Color missileStatusCurrentColour;


    void Start()
    {
        missileStatusText.color = missileStatusAvailableColour;
        missileStatusCurrentColour = missileStatusText.color;

        StartCoroutine(FadeUIText());
    }


    void Update()
    {

    }

    //Status Text.
    public void ResetMissileStatusFade()
    {
        FadeMissileStatusText = false;
        StopCoroutine(FadeUIText());
    }

    public void SetMissileStatusColour(Color colourToApply)
    {
        missileStatusText.color = colourToApply;
    }

    public IEnumerator FadeUIText()
    {
        Color transparentColour = new Color(missileStatusCurrentColour.r, missileStatusCurrentColour.g, missileStatusCurrentColour.b, 0);
        float fadeTimer = 0;

        while (FadeMissileStatusText)
        {
            fadeTimer += Time.deltaTime;
            missileStatusText.color = Color.Lerp(missileStatusCurrentColour, transparentColour, Mathf.PingPong(fadeTimer * missileFadeSpeed, 1));
            yield return null;
        }
    }


}
