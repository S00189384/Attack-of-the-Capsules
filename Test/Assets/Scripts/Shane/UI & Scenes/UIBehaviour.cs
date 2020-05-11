using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Script for handling all UI. Not sure if it was better to make multiple scripts.
//UI has inventory slots, wave number, ammo counter, points counter, interact message etc.
public class UIBehaviour : MonoBehaviour
{
    //Components.
    GameManager gameManager;
    GameObject player;
    PlayerWeaponInventory playerWeaponInventory;

    //General
    public static Color transparentColour = new Color(255, 255, 255, 0);

    //Aim Dot.
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

    [Header("Gun Info")]
    public GameObject gunInfoUI;
    public TextMeshProUGUI tmProWeaponName;
    Coroutine fadeWeaponNameCoroutine;
    public Color weaponNameColour;
    public float weaponNameFadeSpeedAfterEquipping;
    public TextMeshProUGUI tmProGunAmmoCount;
    public Color ammoCountLowColour;
    public Color ammoCountNotLowColour;
    public TextMeshProUGUI tmProGunReserveCount;

    [Header("Throwable UI")]
    public TextMeshProUGUI TMProThrowableCount;
    public GameObject throwableUI;

    [Header("Inventory UI")]
    public InventoryUISlot[] inventoryUISlots;
    private InventoryUISlot lastInventoryUISlotEquipped;

    [Header("Pause Menu")]
    public CanvasGroup pauseMenuCanvasGroup;
    public bool CanPauseGame;
    public bool GameIsPaused;

    [Header("Black Screen")]
    public CanvasGroup blackScreenCanvasGroup;
    public float blackScreenFadeSpeed;

    [Header("Points Counter")]
    public TextMeshProUGUI tmProPointsCounter;
    public Color pointsCounterColourAboveZero;
    public Color pointsCounterColourZero;

    private void Start()
    {
        aimDotOriginalColour = _imgPlayerAimDot.color;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerWeaponInventory = player.GetComponentInChildren<PlayerWeaponInventory>();
        HealthComponent playerHealthComponent = player.GetComponent<HealthComponent>();

        playerHealthComponent.OnDeathEvent += UpdateGameOverTextFailure;
        playerHealthComponent.OnDeathEvent += DisableUI;

        //Pausing stuff.
        if (Time.timeScale != 1)
            Time.timeScale = 1;   

        CanPauseGame = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameManager.CanControlPlayer && !GameIsPaused)
            {
                EnablePauseMenu();
            }
            else if(GameIsPaused)
            {
                DisablePauseMenu();
            }
        }
    }

    //Interact Message.
    public void ShowPlayerInteractMessage(string message, bool FadeUIElement,Color colourToApply)
    {
        tmProInteractMessage.text = message;
        tmProInteractMessage.color = colourToApply;

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

    //General tmpro methods (used in other UI - game over screen etc.)
    public static IEnumerator FadeCavasGroupAlphaFromTo(CanvasGroup canvasGroup,float from,float to,float fadeSpeed)
    {
        float initialAlpha = from;
        float percentageComplete = 0;

        while (percentageComplete < 1)
        {
            percentageComplete += fadeSpeed * Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(initialAlpha, to, percentageComplete);
            yield return null;
        }
    }
    public static IEnumerator FadeTMProColourFromTo(TextMeshProUGUI tmProElement, float fadeSpeed,Color currentColour,Color colourToFadeTo)
    {
        float percentageComplete = 0;
        while (percentageComplete <= 1)
        {
            percentageComplete += Time.deltaTime * fadeSpeed;
            tmProElement.color = Color.Lerp(currentColour, colourToFadeTo, percentageComplete);
            yield return null;
        }
    }
    public static IEnumerator FadeTMProFromClearToVisibleContinuously(TextMeshProUGUI tmProElement, float fadeSpeed)
    {
        Color tmProOriginalColour = tmProElement.color;
        Color transparentColour = new Color(tmProOriginalColour.r, tmProOriginalColour.g, tmProOriginalColour.b, 0);
        float fadeTimer = 0;

        while (true)
        {
            fadeTimer += Time.deltaTime * fadeSpeed;
            tmProElement.color = Color.Lerp(tmProOriginalColour, transparentColour, Mathf.PingPong(fadeTimer, 1));
            yield return null;
        }
    }
    public static IEnumerator FadeTMProFromClearToVisibleOverTime(TextMeshProUGUI tmProElement, float fadeSpeed, float durationOfFade,float alphaToSetAtEnd)
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
    public void UpdateGameOverTextFailure()
    {
        GameOverUI.gameOverText = "You Died";
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

    //Gun UI.
    public void UpdateWeaponUIOnSwitchingWeapon(Weapon weaponToEquip)
    {
        UpdateWeaponName(weaponToEquip.nameOfWeapon);

        if(weaponToEquip.CompareTag("Gun"))
        {
            DisableThrowableUI();
            EnableGunInfoUI();
            Gun gun = weaponToEquip.GetComponent<Gun>();
            UpdateAmmoCount(gun);
            UpdateReserveCount(gun);
        }
        else
        {
            DisableGunInfoUI();
        }

        if(weaponToEquip.CompareTag("Grenade"))
        {         
            EnableThrowableUI();
            UpdateThrowableRemaining(playerWeaponInventory.numberOfGrenadesInInventory);
        }
        else
        {
            DisableThrowableUI();
        }
    }
    public void UpdateWeaponName(string nameOfWeapon)
    {
        if (fadeWeaponNameCoroutine != null)
            StopCoroutine(fadeWeaponNameCoroutine);

        tmProWeaponName.color = weaponNameColour;
        tmProWeaponName.text = nameOfWeapon;
        fadeWeaponNameCoroutine = StartCoroutine(FadeTMProColourFromTo(tmProWeaponName, weaponNameFadeSpeedAfterEquipping, weaponNameColour,new Color(weaponNameColour.r,weaponNameColour.g,weaponNameColour.b,0)));
    }
    public void UpdateAmmoCount(Gun gunToUpdate)
    {
        tmProGunAmmoCount.text = gunToUpdate.magazine.ToString();
        if(gunToUpdate.magazine <= gunToUpdate.maxMagazine * gunToUpdate.lowOnAmmoPercentage)
            tmProGunAmmoCount.color = ammoCountLowColour;
        else
            tmProGunAmmoCount.color = ammoCountNotLowColour;
    }
    public void UpdateReserveCount(Gun gunToUpdate)
    {
        tmProGunReserveCount.text = gunToUpdate.reserves.ToString();
    }
    public void EnableGunInfoUI()
    {
        gunInfoUI.gameObject.SetActive(true);
    }
    public void DisableGunInfoUI()
    {
        gunInfoUI.gameObject.SetActive(false);
    }

    //Throwable UI.
    public void UpdateThrowableRemaining(int numberOfThrowableInInventory)
    {
        TMProThrowableCount.text = numberOfThrowableInInventory.ToString();
    }
    public void EnableThrowableUI()
    {
        throwableUI.gameObject.SetActive(true);
    }
    public void DisableThrowableUI()
    {
        throwableUI.gameObject.SetActive(false);
    }

    //Inventory.
    public void EnableInventoryItemSlot(int indexToEnable)
    {
        if (lastInventoryUISlotEquipped != null)
            lastInventoryUISlotEquipped.ChangeBackgroundColourToUnequipped();

        inventoryUISlots[indexToEnable].ChangeBackgroundColourToEquipped();
        lastInventoryUISlotEquipped = inventoryUISlots[indexToEnable];
    }        

    //Pause Menu.
    private void EnablePauseMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseMenuCanvasGroup.gameObject.SetActive(true);
        DisableUIExceptInteractMessage();
        gameManager.DisablePlayerMovement();
        HidePlayerInteractMessage();
        GameIsPaused = true;
        Time.timeScale = 0;
    }
    public void DisablePauseMenu()
    {
        pauseMenuCanvasGroup.gameObject.SetActive(false);
        EnableUIExceptInteractMessage();
        ShowPlayerInteractMessage("", false,Color.white);
        gameManager.EnablePlayerMovement();
        GameIsPaused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //Black Screen.
    public IEnumerator FadeAndLoadScene(float fadeSpeed,int sceneIndexToLoad)
    {
        float percentageComplete = 0;
        while (percentageComplete < 1)
        {
            percentageComplete += fadeSpeed * Time.deltaTime;
            blackScreenCanvasGroup.alpha = Mathf.Lerp(0, 1, percentageComplete);
            yield return null;
        }

        GetComponent<SceneLoader>().LoadScene(sceneIndexToLoad);
    }
    public void FadeToBlackAndLoadScene(float fadeSpeed,int sceneIndexToLoad)
    {
        StartCoroutine(FadeAndLoadScene(fadeSpeed,sceneIndexToLoad));
    }

    //Points Counter.
    public void UpdatePointsCounter(int pointsToDisplay)
    {
        tmProPointsCounter.text = pointsToDisplay.ToString();

        if (pointsToDisplay <= 0)
            tmProPointsCounter.color = pointsCounterColourZero;
        else
            tmProPointsCounter.color = pointsCounterColourAboveZero;
    }
}
