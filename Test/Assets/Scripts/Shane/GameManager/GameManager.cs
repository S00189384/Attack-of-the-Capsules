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

    //Player Controlling.
    public void DisablePlayerMovement()
    {
        CanControlPlayer = false;
        player.GetComponent<AudioSource>().Stop();
    }
    public IEnumerator DisablePlayerMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisablePlayerMovement();
    }
    public void EnablePlayerMovement()
    {
        CanControlPlayer = true;
    }
}
