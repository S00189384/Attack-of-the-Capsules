using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/* Firing missile ui screen has  -  
    A status message which flashes informing player whether missile is available or not to fire - missileStatus.
    A console command like flashing "_" to show that player can "type" something into the computer - missileInputField.
    When command is entered - fade to black and start controlling the missile sequence.   
*/

public delegate void CooldownDelegate();
public class MissileComputerCanvas : MonoBehaviour
{
    //Components.
    [Header("Components")]
    AudioSource audioSource;
    public MissileComputerScreen computerScreen;
    //public BlackScreen blackScreen;

    //Events.
    public CooldownDelegate CooldownEndedEvent;
    public CooldownDelegate CooldownStartedEvent;

    //UI elements.
    [Header("On Screen UI")]
    public TextMeshProUGUI tmProMissileStatus;
    public TextMeshProUGUI tmProMissileInputField;
    public TextMeshProUGUI tmProCooldown;

    [Header("Canvas Groups")]
    public CanvasGroup missileAvailableCanvasGroup;
    public CanvasGroup missileNotAvailableCanvasGroup;
    public CanvasGroup blackScreen;

    Coroutine fadeMissileStatusCoroutine;
    Coroutine flashInputFieldCoroutine;
    Coroutine typeCommandCoroutine;
    Coroutine cooldownCoroutine;

    [Header("Fading To Black")]
    public float blackScreenFadeSpeed;

    //Missile Status
    [Header("Missile Status")]
    public float missileFadeSpeed;
    private string missileAvailableMessage = "Missile Available";
    private string missileNotAvailableMessage = "Missile Not Available";

    [Header("Missile Status Colour")]
    public Color missileStatusAvailableColour;
    public Color missileStatusNotAvailableColour;
    public Color missileStatusCurrentColour;

    [Header("Missile Input Field")]
    public float timeBetweenTypedLetters = 0.2f;
    public float fadeToBlackTimeAfterCommandTyped;
    public float missileInputFieldFlashtime;
    private string fireMissileInputMessage = "FireMissile();";
    private string missileInputDefaultText = "_";

    [Header("Missile")]
    public Missile missilePrefab;
    public Transform missileStartPosition;

    [Header("Audio Clips")]
    public AudioClip typingSounds;
    public AudioClip missileDrop;
    public AudioClip missileExplosion;

    [Header("Cooldown")]
    public float cooldownTime = 30;
    public float cooldownTimer;

    void Start()
    {
        //Components.

        //Audio
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = typingSounds;

        //When player interacts, trigger 
        computerScreen.PlayerLookedAtScreenEvent += TypeMissileInput;
        computerScreen.PlayerLookedAwayFromScreenEvent += SwitchToCooldown;

        MakeComputerAvailable();
        ResetCooldownTimer();
    }

    //Status Message.
    public void SetMissileStatusText(string textToAppear)
    {
        tmProMissileStatus.text = textToAppear;
    }
    public void ResetMissileStatusFade()
    {
        StopCoroutine(fadeMissileStatusCoroutine);
        fadeMissileStatusCoroutine = null;
    }
    public void SetMissileStatusColour(Color colourToApply)
    {
        tmProMissileStatus.color = colourToApply;
        missileStatusCurrentColour = tmProMissileStatus.color;
    }
    public IEnumerator FadeTextMeshProElement(TextMeshProUGUI textMeshPro)
    {
        Color transparentColour = new Color(missileStatusCurrentColour.r, missileStatusCurrentColour.g, missileStatusCurrentColour.b, 0);
        float fadeTimer = 0;

        while (true)
        {
            fadeTimer += Time.deltaTime;
            tmProMissileStatus.color = Color.Lerp(missileStatusCurrentColour, transparentColour, Mathf.PingPong(fadeTimer * missileFadeSpeed, 1));
            yield return null;
        }
    }
    public void FadeMissileStatusMessage()
    {
        if(fadeMissileStatusCoroutine == null)
            fadeMissileStatusCoroutine = StartCoroutine(FadeTextMeshProElement(tmProMissileInputField));
    }

    //Input text
    private void StartInputTextFlashing()
    {
        if(flashInputFieldCoroutine == null)
            flashInputFieldCoroutine = StartCoroutine(FlashInputText());
    }
    private void StopInputTextFlashing()
    {
        StopCoroutine(flashInputFieldCoroutine);
        flashInputFieldCoroutine = null;
    }
    IEnumerator FlashInputText()
    {
        while (true)
        {
            if (tmProMissileInputField.enabled)
                tmProMissileInputField.enabled = false;
            else
                tmProMissileInputField.enabled = true;

            yield return new WaitForSeconds(missileInputFieldFlashtime);
        }      
    }

    //Typing.
    public void TypeMissileInput()
    {
        if(typeCommandCoroutine == null)
            typeCommandCoroutine = StartCoroutine(TypeCommandToFire());
    }
    IEnumerator TypeCommandToFire()
    {
        ResetMissileStatusFade();
        SetMissileStatusColour(missileStatusAvailableColour);
        StopInputTextFlashing();
        tmProMissileInputField.text = "";
        tmProMissileInputField.enabled = true;

        //Type message on screen.
        audioSource.clip = typingSounds;
        audioSource.Play();

        foreach (char letter in fireMissileInputMessage)
        {
            tmProMissileInputField.text += letter;
            if (letter.ToString() != " ")
                yield return new WaitForSeconds(timeBetweenTypedLetters);
        }

        audioSource.Stop();

        Instantiate(missilePrefab, missileStartPosition.position,Quaternion.Euler(0,0,-90));
        yield return StartCoroutine(UIBehaviour.FadeCavasGroupAlphaFromTo(blackScreen,blackScreen.alpha,1,blackScreenFadeSpeed));
        audioSource.clip = missileDrop;
        audioSource.Play();
        computerScreen.computerScreenCamera.enabled = false;
        StartCoroutine(UIBehaviour.FadeCavasGroupAlphaFromTo(blackScreen, blackScreen.alpha, 0, blackScreenFadeSpeed));

        StopCoroutine(typeCommandCoroutine);
        typeCommandCoroutine = null;
    }
    private void ResetInputText()
    {
        tmProMissileInputField.text = missileInputDefaultText;
    }

    //Cooldown.
    public void SwitchToCooldown()
    {
        HideCanvasGroup(missileAvailableCanvasGroup);
        ShowCanvasGroup(missileNotAvailableCanvasGroup);

        SetMissileStatusText(missileNotAvailableMessage);
        SetMissileStatusColour(missileStatusNotAvailableColour);
        FadeMissileStatusMessage();

        if (cooldownCoroutine == null)
            cooldownCoroutine = StartCoroutine(StartCooldown());
    }
    public void SetCooldownTimer(string textToShow)
    {
        tmProCooldown.text = textToShow;
    }
    IEnumerator StartCooldown()
    {
        CooldownStartedEvent();

        while (cooldownTimer >= 0)
        {
            cooldownTimer -= Time.deltaTime;
            SetCooldownTimer(Mathf.CeilToInt(cooldownTimer).ToString());
            yield return null;
        }

        //Cooldown is over. Event changes the canvas & computer updates.
        CooldownEndedEvent();
        ResetCooldownTimer();
        ResetComputerAfterCooldown();

        StopCoroutine(cooldownCoroutine);
        cooldownCoroutine = null;
    }
    private void ResetCooldownTimer()
    {
        cooldownTimer = cooldownTime;
    }

    //After cooldown.
    public void ResetComputerAfterCooldown()
    {
        tmProCooldown.text = "";
        ResetMissileStatusFade();
        MakeComputerAvailable();   
    }
    public void MakeComputerAvailable()
    {
        //Status text setting up.
        SetMissileStatusColour(missileStatusAvailableColour);
        SetMissileStatusText(missileAvailableMessage);
        ResetInputText();

        HideCanvasGroup(missileNotAvailableCanvasGroup);
        ShowCanvasGroup(missileAvailableCanvasGroup);

        FadeMissileStatusMessage();
        StartInputTextFlashing();
    }

    //Canvas Groups.
    public void HideCanvasGroup(CanvasGroup canvasGroupToHide)
    {
        canvasGroupToHide.alpha = 0;
    }
    public void ShowCanvasGroup(CanvasGroup canvasGroupToShow)
    {
        canvasGroupToShow.alpha = 1;
    }
    public void PlayAudioWhenMissileHitsGround()
    {
        audioSource.Stop();
        audioSource.clip = missileExplosion;
        audioSource.Play();
    }
}
