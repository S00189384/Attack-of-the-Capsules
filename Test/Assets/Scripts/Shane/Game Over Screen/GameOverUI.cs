using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public Button btnMainMenu;
    public Button btnTryAgain;

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

    //Menu. Button Presses.
    public void LoadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

}
