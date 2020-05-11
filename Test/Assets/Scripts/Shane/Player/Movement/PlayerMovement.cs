using System.Collections;
using UnityEngine;

//Player Movement - player can jump and sprint(sprint has cooldown). 
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    //Components.
    GameManager gameManager;
    HealthComponent playerHealthComponent;
    PlayerWeaponInventory playerWeaponInventory;
    PlayerGunAttack playerAttack;
    CharacterController characterController;
    AudioSource audioSource;

    [Header("Movement")]
    public float movementSpeed = 0f;
    float xMovementInput;
    float zMovementInput;
    Vector3 movementThisFrame;
    Vector3 velocity;

    [Header("Walking")]
    public float walkSpeed = 12f;
    public AudioClip walkSound;

    [Header("Sprinting")]
    public bool IsSprinting;
    public float sprintSpeed = 24f;
    public float sprintTimer = 0;
    public float maxSprintTime = 3;

    public bool SprintIsInCooldown;
    public float sprintCooldownTimer = 0;
    public float sprintCooldownTime = 4;
    public AudioClip sprintSound;

    [Header("Jumping")]
    public float gravity = -9.81f;
    public float jumpheight = 5f;

    [Header("Crouching")]
    public bool IsCrouching;
    public float NormalHeight;
    public float CrouchHeight;
    public float CrouchSpeed = 2;
    public float CrouchMovementSpeed = 2;

    [Header("Ground Checking")]
    public Transform groundcheck;
    public LayerMask groundmask;
    public float grounddistance = 0.4f;
    public bool IsGrounded;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        playerHealthComponent = GetComponent<HealthComponent>();
        playerHealthComponent.OnDeathEvent += ResetAudioPlayer;
        playerWeaponInventory = GetComponentInChildren<PlayerWeaponInventory>();
        characterController = GetComponent<CharacterController>();
        playerAttack = GetComponentInChildren<PlayerGunAttack>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = walkSound;
        audioSource.loop = true;
        NormalHeight = characterController.height;
        CrouchHeight = characterController.height / 2;
    }

    void Update()
    {
        if(gameManager.CanControlPlayer)
        {
            CheckIfPlayerIsGrounded();
            MoveOnPlayerInput();
            PlayFootstepSounds();
            JumpOnPlayerInput();
            CrouchOnPlayerInput();
        }
    }

    private void JumpOnPlayerInput()
    {
        //setting velocity to near zero
        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        //jumping
        if (Input.GetKeyDown("space") && IsGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpheight * -2f * gravity);
        }

        //gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    private void MoveOnPlayerInput()
    {
        xMovementInput = Input.GetAxisRaw("Horizontal");
        zMovementInput = Input.GetAxisRaw("Vertical");

        //Check if player tries to sprint.
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && sprintTimer <= maxSprintTime && !SprintIsInCooldown && !IsCrouching && !playerAttack.IsAiming)
        {
            IsSprinting = true;
            movementSpeed = sprintSpeed;
            sprintTimer += Time.deltaTime;
            if (sprintTimer >= maxSprintTime)
                SprintIsInCooldown = true;
        }
        else
        {
            IsSprinting = false;
            if(!IsCrouching)
                movementSpeed = walkSpeed;

            if (SprintIsInCooldown)
            {
                sprintCooldownTimer += Time.deltaTime;
                if (sprintCooldownTimer >= sprintCooldownTime)
                {
                    SprintIsInCooldown = false;
                    sprintCooldownTimer = 0;
                }
            }

            if (sprintTimer > 0)
            {
                sprintTimer -= Time.deltaTime;
                if (sprintTimer <= 0)
                    sprintTimer = 0;
            }
        }

        //Moving.
        movementThisFrame = transform.right * xMovementInput + transform.forward * zMovementInput;
        characterController.Move(movementThisFrame * movementSpeed * Time.deltaTime);
    }
    private void PlayFootstepSounds()
    {
        if (IsGrounded && characterController.velocity!= Vector3.zero && !IsCrouching)
        {
            audioSource.clip = IsSprinting ? sprintSound : walkSound;
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
        }
    }
    private void CheckIfPlayerIsGrounded()
    {
        IsGrounded = Physics.CheckSphere(groundcheck.position, grounddistance, groundmask);
    }
    private void CrouchOnPlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && IsGrounded && !IsSprinting)
        {
            if(!IsCrouching)
            {
                IsCrouching = true;
                movementSpeed = CrouchMovementSpeed;
                StartCoroutine(AdjustHeight(CrouchHeight));
            }
            else
            {
                IsCrouching = false;
                StartCoroutine(AdjustHeight(NormalHeight));
            }
        }
    }

    private IEnumerator AdjustHeight(float targetHeight)
    {
        float percentageComplete = 0;

        while (percentageComplete <= 1)
        {
            characterController.height = Mathf.Lerp(characterController.height, targetHeight, percentageComplete);
            percentageComplete += CrouchSpeed * Time.deltaTime;
            yield return null;
        }
    }

    public void ResetAudioPlayer()
    {
        audioSource.clip = null;
    }
}
