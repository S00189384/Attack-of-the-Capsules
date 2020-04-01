using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour
{
    [Header("Death Screen")]
    public Image _imgBlackScreen;
    public float blackScreenFadeSpeed = 1f;

    public IEnumerator FadeToBlack(float fadeSpeed)
    {
        Color startingColor = _imgBlackScreen.color;

        float percentageComplete = 0;
        while (percentageComplete < 1)
        {
            percentageComplete += fadeSpeed * Time.deltaTime;
            _imgBlackScreen.color = Color.Lerp(startingColor, Color.black, percentageComplete);
            yield return null;
        }

    }
}
