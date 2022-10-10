using System.Collections;
using UnityEngine;

//A script that mainly deals with enabling / disabling player movement. Could have put that in player movement (probably better) but ended up putting it here.
//When they load in from menu it makes the cursor not visible.
public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;
    public bool CanControlPlayer = true;

    private void Start()
    {
        player.GetComponent<HealthComponent>().OnDeathEvent += DisablePlayerMovement;
        Cursor.visible = false;
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
