using UnityEngine;
using UnityEngine.SceneManagement;

//Used in menu scenes to load other scenes on button click.
public class SceneLoader : MonoBehaviour
{    
    public void LoadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
