using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;
    public bool CanControlPlayer = true;

    private void Start()
    {
        player.GetComponent<HealthComponent>().OnDeathEvent += DisablePlayerMovement;
    }

    public void DisablePlayerMovement()
    {
        CanControlPlayer = false;
    }

    public void EnablePlayerMovement()
    {
        CanControlPlayer = true;
    }

    //Ending the game.
    public IEnumerator FadeToBlackAndLoadScene(float delayBeforeStartingFade,float fadeSpeed,int sceneIndex)
    {
        yield return new WaitForSeconds(delayBeforeStartingFade);

        DisablePlayerMovement();
        player.GetComponent<AudioSource>().Stop();

        yield return GameObject.FindGameObjectWithTag("BlackScreen").GetComponent<BlackScreen>().FadeToBlack(fadeSpeed);

        SceneManager.LoadScene(sceneIndex);
    }
}
