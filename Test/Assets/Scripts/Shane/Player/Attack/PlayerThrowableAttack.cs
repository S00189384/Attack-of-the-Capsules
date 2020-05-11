using UnityEngine;

//Check for throwing weapon if they have a throwable equipped.
public class PlayerThrowableAttack : MonoBehaviour
{
    //Components.
    GameManager gameManager;
    PlayerWeaponInventory PlayerWeaponInventory;

    [Header("Throw Info")]
    public float throwCooldownTime = 2;
    public float nextTimeCanThrow;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        PlayerWeaponInventory = GetComponent<PlayerWeaponInventory>();
    }

    private void Update()
    {
        if(gameManager.CanControlPlayer)
        {
            if (PlayerWeaponInventory.activeThrowableWeapon)
            {
                if (Input.GetMouseButtonDown(0) && Time.time > nextTimeCanThrow)
                {
                    nextTimeCanThrow = Time.time + throwCooldownTime;
                    PlayerWeaponInventory.activeThrowableWeapon.animator.SetBool("Throw", true);
                }
            }
        }
    }

}
