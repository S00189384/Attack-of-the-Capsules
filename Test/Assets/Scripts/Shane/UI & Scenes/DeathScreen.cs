using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    [Header("Death Screen")]
    public Image _imgDeathScreen;
    public float deathScreenFadeSpeed = 1f;

    //Death Screen.
    IEnumerator FadeToBlack()
    {
        Color startingColor = _imgDeathScreen.color;

        float percentageComplete = 0;
        while (percentageComplete < 1)
        {
            print(percentageComplete);
            percentageComplete += deathScreenFadeSpeed * Time.deltaTime;
            _imgDeathScreen.color = Color.Lerp(startingColor, Color.black, percentageComplete);
            yield return null;
        }

        SceneManager.LoadScene(1);
    }
    public void StartFadeToDeathScreen()
    {
        StartCoroutine(FadeToBlack());
    }

}
