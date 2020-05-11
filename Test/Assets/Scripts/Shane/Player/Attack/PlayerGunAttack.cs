using UnityEngine;

//If gun is equipped, checks if player fires gun and checks if player aims down sights.
public class PlayerGunAttack : MonoBehaviour
{
    //Components.
    GameManager gameManager;
    PlayerMovement playerMovement;
    PlayerWeaponInventory PlayerWeaponInventory;
    PlayerInteractRaycast playerInteractRaycast;

    [Header("Weapon Aiming")]
    public bool IsAiming;
    public Camera playerCamera;
    public float aimSpeed;
    public float cameraFOVWhileAiming;
    public AudioClip aimSound;
    float defaultCameraFOV;
    AudioSource audioSource;

    //Fading aim dot image;
    UIBehaviour UIBehaviour;
    Color transparentColour = new Color(255, 255, 255, 0);

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        UIBehaviour = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBehaviour>();
        PlayerWeaponInventory = GetComponent<PlayerWeaponInventory>();
        playerInteractRaycast = GetComponentInParent<PlayerInteractRaycast>();
        audioSource = GetComponent<AudioSource>();
        defaultCameraFOV = playerCamera.fieldOfView;
    }

    private void Update()
    {
        if(gameManager.CanControlPlayer)
        {
            if (PlayerWeaponInventory.activeGun)
            {
                if (Input.GetKeyDown(KeyCode.R) && !playerMovement.IsSprinting)
                {
                    PlayerWeaponInventory.activeGun.Reload();
                }

                CheckIfPlayerAims();
                CheckIfPlayerShoots();
            }
        }
    }

    private void CheckIfPlayerShoots()
    {
        if (!PlayerWeaponInventory.activeGun.IsReloading)
        {
            if (PlayerWeaponInventory.activeGun.magazine > 0)
            {
                //Gun can shoot. Check if automatic or not.
                if (PlayerWeaponInventory.activeGun.IsAutomatic)
                {
                    if (Input.GetButton("Fire1"))
                    {
                        //Fire weapon.
                        if (Time.time - PlayerWeaponInventory.activeGun.lastFired > 1 / PlayerWeaponInventory.activeGun.automaticRateOfFire)
                        {
                            FireActiveGun();
                        }
                    }
                }
                else
                {
                    if(Input.GetButtonDown("Fire1"))
                    {
                        if (Time.time - PlayerWeaponInventory.activeGun.lastFired > 1 / PlayerWeaponInventory.activeGun.singleRateOfFire)
                        {
                            FireActiveGun();
                        }
                    }
                }
            }                   
        }
    }

    private void FireActiveGun()
    {
        PlayerWeaponInventory.activeGun.lastFired = Time.time;
        PlayerWeaponInventory.activeGun.Fire(transform.position, transform.forward);
    }


    //Cannot aim when running - checked in playermovement script.
    private void CheckIfPlayerAims()
    {
        if (Input.GetMouseButton(1) && !PlayerWeaponInventory.activeGun.IsReloading)
        {
            if(!PlayerWeaponInventory.activeGun.animator.GetBool("Aiming"))
                PlayerWeaponInventory.activeGun.animator.SetBool("Aiming", true);

            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView,
                cameraFOVWhileAiming, aimSpeed * Time.deltaTime);

            PlayerWeaponInventory.activeGun.transform.localPosition
                = Vector3.Lerp(PlayerWeaponInventory.activeGun.transform.localPosition, PlayerWeaponInventory.activeGun.weaponPosAiming, aimSpeed * Time.deltaTime);

            if (!playerInteractRaycast.IsLookingAtInteractableObject)
                UIBehaviour.FadeAimDotColour(aimSpeed, transparentColour);

            IsAiming = true;
        }
        else
        {
            if(playerCamera.fieldOfView != defaultCameraFOV)
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView,defaultCameraFOV, aimSpeed * Time.deltaTime);

            if(PlayerWeaponInventory.activeGun.transform.localPosition != PlayerWeaponInventory.activeGun.defaultWeaponPosition)
                PlayerWeaponInventory.activeGun.transform.localPosition = Vector3.Lerp(PlayerWeaponInventory.activeGun.transform.localPosition, PlayerWeaponInventory.activeGun.defaultWeaponPosition, aimSpeed * Time.deltaTime);

          
            if (!playerInteractRaycast.IsLookingAtInteractableObject)
                UIBehaviour.FadeAimDotColour(aimSpeed, UIBehaviour.aimDotOriginalColour);

            if(PlayerWeaponInventory.activeGun.animator.GetBool("Aiming"))
                PlayerWeaponInventory.activeGun.animator.SetBool("Aiming", false);


            IsAiming = false;
            audioSource.Play();
        }

    }

    public void ResetAimingDownWeapon()
    {
        if(IsAiming)
        {
            PlayerWeaponInventory.activeGun.transform.localPosition = PlayerWeaponInventory.activeGun.defaultWeaponPosition;
            playerCamera.fieldOfView = defaultCameraFOV;
            PlayerWeaponInventory.activeGun.animator.SetBool("Aiming", false);
        }
    }
}
