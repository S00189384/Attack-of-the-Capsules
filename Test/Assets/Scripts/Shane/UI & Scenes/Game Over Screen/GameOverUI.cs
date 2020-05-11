using System.Collections;
using TMPro;
using UnityEngine;

//This scene is loaded when player wins or dies (decided to just have one scene for this, only thing different is the gameover text).
//Game over text is different depending on if the player died or survived all waves.
public class GameOverUI : MonoBehaviour
{
    public static string gameOverText;

    [Header("GameOver Text")]
    public TextMeshProUGUI TMProGameOver;
    public Color gameOverColour;
    public float gameOverTextFadeInSpeed;
    public float delayToDisplayMenu;

    [Header("Menu")]
    public GameObject menuUI;

    IEnumerator Start()
    {
        Color youDiedTextColour = TMProGameOver.color;
        TMProGameOver.text = gameOverText;
        yield return StartCoroutine(UIBehaviour.FadeTMProColourFromTo(TMProGameOver, gameOverTextFadeInSpeed, youDiedTextColour, gameOverColour));
        yield return new WaitForSeconds(delayToDisplayMenu);

        Cursor.lockState = CursorLockMode.None;    
        Cursor.visible = true;

        menuUI.gameObject.SetActive(true);
    }
}
