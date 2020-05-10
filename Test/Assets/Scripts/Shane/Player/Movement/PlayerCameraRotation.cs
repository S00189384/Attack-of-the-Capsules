using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraRotation : MonoBehaviour
{
    //Components.
    GameManager gameManager;
    HealthComponent playerHealthComponent;

    public Transform PlayerBody;
    public Camera playerCamera;

    //Rotation
    [Header("Rotation Speed")]
    public int defaultMouseSensitivity;

    [Header("Player Input")]
    public float horizontalRotationInput;
    public float verticalRotationInput;

    //Vertical rotation.
    [Header("Looking Up and Down")]
    public float VerticalRotation = 0;
    public float MinXAxisClampValue = -90;
    public float MaxXAxisClampValue = 90;

    void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera = Camera.main;
        playerHealthComponent = GetComponentInParent<HealthComponent>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }
	
	void Update ()
    {
        RotateOnPlayerInput();
	}

    private void RotateOnPlayerInput()
    {
        if(gameManager.CanControlPlayer)
        {
            //Get user entered input for rotation.
            horizontalRotationInput = Input.GetAxisRaw("Mouse X");
            verticalRotationInput = Input.GetAxisRaw("Mouse Y");

            //Get rotation amount for looking down and up and clamp it.
            VerticalRotation += verticalRotationInput * Settings.playerCameraSensitivity;
            ClampVerticalRotation();

            //Rotate player camera and player.
            transform.localRotation = Quaternion.Euler(-VerticalRotation, 0f, 0f);
            PlayerBody.Rotate(Vector3.up * horizontalRotationInput * Settings.playerCameraSensitivity);
        }
    }
    private void ClampVerticalRotation()
    {
        VerticalRotation = Mathf.Clamp(VerticalRotation, MinXAxisClampValue, MaxXAxisClampValue);

        if (VerticalRotation >= MaxXAxisClampValue || VerticalRotation <= MinXAxisClampValue)
            verticalRotationInput = 0;
    }
}
