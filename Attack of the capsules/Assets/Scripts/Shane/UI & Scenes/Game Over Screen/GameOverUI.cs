using System.Collections;
using TMPro;
using UnityEngine;

//This scene is loaded when player wins or dies (decided to just have one scene for this, only thing different is the gameover text).
//Game over text is different depending on if the player died or survived all waves.
public class GameOverUI : MonoBehaviour
{
    public static string gameOverText;
    public static string wavesSurvivedText;
    public static string numEnemiesKilledText;

    [Header("GameOver Text")]
    public TextMeshProUGUI TMProGameOverHeader;
    public TextMeshProUGUI TMProRoundsSurvived;
    public TextMeshProUGUI TMProNumEnemiesKilled;
    public Color gameOverColour;
    public float gameOverTextFadeInSpeed;
    public float delayToDisplayMenu;

    [Header("Menu")]
    public GameObject menuUI;

    IEnumerator Start()
    {
        Color youDiedTextColour = TMProGameOverHeader.color;

        UpdateUIText();

        yield return StartCoroutine(UIBehaviour.FadeTMProColourFromTo(TMProGameOverHeader, gameOverTextFadeInSpeed, youDiedTextColour, gameOverColour));
        yield return new WaitForSeconds(delayToDisplayMenu);

        Cursor.lockState = CursorLockMode.None;    
        Cursor.visible = true;

        menuUI.gameObject.SetActive(true);
    }

    public static void SetGameOverText(string mainText,int numWavesSurvived,int numEnemiesKilled)
    {
        gameOverText = mainText;
        wavesSurvivedText = $"Waves survived - {numWavesSurvived}";
        numEnemiesKilledText = $"Enemies killed - {numEnemiesKilled}";
    }

    private void UpdateUIText()
    {
        TMProGameOverHeader.text = gameOverText;
        TMProRoundsSurvived.text = wavesSurvivedText;
        TMProNumEnemiesKilled.text = numEnemiesKilledText;
    }
}