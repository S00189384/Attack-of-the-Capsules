using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour
{
    //General
    Color transparentColour = new Color(255, 255, 255, 0);

    public Image _imgPlayerAimDot;
    public Color aimDotOriginalColour;

    [Header("Canvas Groups")]
    public CanvasGroup entireUICanvasGroup;
    public CanvasGroup allUIExceptInteractMessageCanvasGroup;
    public float canvasFadeSpeed;

    [Header("Interact Message For Player")]
    public TextMeshProUGUI tmProInteractMessage;
    public float interactMessageFadeSpeed = 1f;
    Coroutine FadeInteractMessageCoroutine;

    [Header("Wave Number")]
    public TextMeshProUGUI tmProWaveNumber;
    public Color waveNumberColour;
    public Color waveNumberColourDuringRoundTransition;
    public float startOfRoundFadeSpeed;
    public float roundTransitionFadeSpeed;
    public float roundTransitionFadeDuration;
    public float delayAfterRoundFadingEnds;
    Coroutine startOfRoundFadeCoroutine;
    Coroutine roundTransitionCoroutine;

    private void Start()
    {
        aimDotOriginalColour = _imgPlayerAimDot.color;
        GameObject.FindGameObjectWithTag("Player").GetComponent<HealthComponent>().OnDeathEvent += DisableUI;
    }

    //Interact Message.
    public void ShowPlayerInteractMessage(string message, bool FadeUIElement)
    {
        tmProInteractMessage.text = message;

        if (FadeUIElement)
        {
            if(FadeInteractMessageCoroutine == null)
                FadeInteractMessageCoroutine = StartCoroutine(FadeTMProFromClearToVisibleContinuously(tmProInteractMessage,interactMessageFadeSpeed));
        }
    }
    public void HidePlayerInteractMessage()
    {
        tmProInteractMessage.text = "";
        tmProInteractMessage.color = Color.white;
        if (FadeInteractMessageCoroutine != null)
        {
            StopCoroutine(FadeInteractMessageCoroutine);
            FadeInteractMessageCoroutine = null;
        }
    }
    public bool IsNotificationMessageOnScreen()
    {
        return tmProInteractMessage.text != "";
    }

    //Aim dot.
    public void ResetAimDotColour()
    {
        _imgPlayerAimDot.color = aimDotOriginalColour;
    }
    public void FadeAimDotColour(float fadeSpeed,Color targetColour)
    {
        _imgPlayerAimDot.color = Color.Lerp(_imgPlayerAimDot.color, targetColour, fadeSpeed * Time.deltaTime);
    }

    //Overall UI.
    public IEnumerator FadeUI()
    {
        float fadeTimer = 0;

        while (fadeTimer < 1)
        {
            fadeTimer += Time.deltaTime;
            entireUICanvasGroup.alpha = Mathf.Lerp(entireUICanvasGroup.alpha, 0, fadeTimer * canvasFadeSpeed); 
            yield return null;
        }

        DisableUI();
    }
    public void DisableUI()
    {
        entireUICanvasGroup.alpha = 0;
    }
    public void EnableUI()
    {
        entireUICanvasGroup.alpha = 1;
    }

    public void DisableUIExceptInteractMessage()
    {
        allUIExceptInteractMessageCanvasGroup.alpha = 0;
    }
    public void EnableUIExceptInteractMessage()
    {
        allUIExceptInteractMessageCanvasGroup.alpha = 1;
    }

    //General tmpro methods.
    IEnumerator FadeTMProColourFromTo(TextMeshProUGUI tmProElement, float fadeSpeed,Color currentColour,Color colourToFadeTo)
    {
        float percentageComplete = 0;
        while (percentageComplete <= 1)
        {
            percentageComplete += Time.deltaTime * fadeSpeed;
            tmProElement.color = Color.Lerp(currentColour, colourToFadeTo, percentageComplete);
            yield return null;
        }
    }
    IEnumerator FadeTMProFromClearToVisibleContinuously(TextMeshProUGUI tmProElement, float fadeSpeed)
    {
        Color tmProOriginalColour = tmProElement.color;
        float fadeTimer = 0;

        while (true)
        {
            fadeTimer += Time.deltaTime * fadeSpeed;
            tmProElement.color = Color.Lerp(tmProOriginalColour, transparentColour, Mathf.PingPong(fadeTimer, 1));
            yield return null;
        }
    }
    IEnumerator FadeTMProFromClearToVisibleOverTime(TextMeshProUGUI tmProElement, float fadeSpeed, float durationOfFade,float alphaToSetAtEnd)
    {
        Color tmProOriginalColour = tmProElement.color;
        float timer = 0;
        while(timer <= durationOfFade)
        {
            timer += Time.deltaTime;
            tmProElement.color = Color.Lerp(tmProOriginalColour,transparentColour,Mathf.PingPong(timer, 1));
            yield return null;
        }

        tmProElement.color = new Color(tmProOriginalColour.r, tmProOriginalColour.g, tmProOriginalColour.b, alphaToSetAtEnd);
    }

    //Wave UI.
    public void FadeRoundNumberOnRoundStart()
    {
        startOfRoundFadeCoroutine = StartCoroutine(FadeTMProColourFromTo(tmProWaveNumber, startOfRoundFadeSpeed, tmProWaveNumber.color, waveNumberColour));
    }

    public IEnumerator UpdateRoundNumberOnRoundTransition(int roundNumber)
    {
        tmProWaveNumber.color = waveNumberColourDuringRoundTransition;
        yield return roundTransitionCoroutine = StartCoroutine(FadeTMProFromClearToVisibleOverTime(tmProWaveNumber, roundTransitionFadeSpeed, roundTransitionFadeDuration,0));
        yield return new WaitForSeconds(delayAfterRoundFadingEnds);
        tmProWaveNumber.text = roundNumber.ToString();
        FadeRoundNumberOnRoundStart();
    }
}
