using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour
{
    public Image _imgPlayerAimDot;
    public Color aimDotOriginalColour;

    [Header("Overall Canvas")]
    public CanvasGroup canvasGroup;
    public float canvasFadeSpeed;

    [Header("Interact Message For Player")]
    public TextMeshProUGUI _txtInteractMessage;
    public float interactMessageFadeSpeed = 1f;
    Coroutine FadeInteractMessage;

    private void Start()
    {
        aimDotOriginalColour = _imgPlayerAimDot.color;
        GameObject.FindGameObjectWithTag("Player").GetComponent<HealthComponent>().OnDeathEvent += DisableUI;
    }

    //Interact Message.
    public void ShowPlayerInteractMessage(string message, bool FadeUIElement)
    {
        _txtInteractMessage.text = message;

        if (FadeUIElement)
        {
            //if (FadeInteractMessage != null)
            //    StopCoroutine(FadeInteractMessage);

            if(FadeInteractMessage == null)
                FadeInteractMessage = StartCoroutine(FadePlayerInteractMessage());
        }
    }
    public void HidePlayerInteractMessage()
    {
        _txtInteractMessage.text = "";
        _txtInteractMessage.color = Color.white;
        if (FadeInteractMessage != null)
        {
            StopCoroutine(FadeInteractMessage);
            FadeInteractMessage = null;
        }
    }
    IEnumerator FadePlayerInteractMessage()
    {
        Color transparentColour = new Color(255, 255, 255, 0);
        float fadeTimer = 0;

        while (true)
        {
            fadeTimer += Time.deltaTime;
            _txtInteractMessage.color = Color.Lerp(aimDotOriginalColour, transparentColour, Mathf.PingPong(fadeTimer * interactMessageFadeSpeed, 1));
            yield return null;
        }
    }
    public bool IsNotificationMessageOnScreen()
    {
        return _txtInteractMessage.text != "";
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
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, fadeTimer * canvasFadeSpeed); 
            yield return null;
        }

        DisableUI();
    }
    public void DisableUI()
    {
        canvasGroup.gameObject.SetActive(false);
    }
    public void EnableUI()
    {
        canvasGroup.gameObject.SetActive(true);
    }
}
