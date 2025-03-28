using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Core Components")]
    public CharacterController characterController;
    public PlayerHealth playerHealth;
    public PlayerArmor playerArmor;
    public PlayerStagger playerStagger;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Look Settings")]
    public float lookSensitivity = 2f;
    public Transform cameraTransform;
    private float xRotation = 0f;

    [Header("UI References")]
    public DeathScreenController deathScreenController;
    public DamageScreenController damageScreenController;
    [Header("Audio")]
    public FootstepAudio footstepAudio;

    public float GetCurrentHealth() => playerHealth.currentHealth;
    public float GetMaxHealth() => playerHealth.maxHealth;

    public float GetCurrentArmor() => playerArmor.currentArmor;
    public float GetMaxArmor() => playerArmor.maxArmor;

    private Vector3 velocity;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        playerHealth.damageScreenController = damageScreenController;
        playerHealth.deathScreenController = deathScreenController;
    }

    public void TakeDamage(float damage)
    {
        int remainingDamage = playerArmor.AbsorbDamage((int)damage);
        playerHealth.TakeDamage(remainingDamage);

        if (!playerHealth.IsDead())
        {
            playerStagger.Stagger();
        }
    }
    
    public void ModifyMovementSpeed(float speedMultiplier, float duration)
    {
        StartCoroutine(ModifySpeedCoroutine(speedMultiplier, duration));
    }

    private IEnumerator ModifySpeedCoroutine(float speedMultiplier, float duration)
    {
        float originalSpeed = moveSpeed;
        moveSpeed *= speedMultiplier;

        yield return new WaitForSeconds(duration);

        moveSpeed = originalSpeed;
    }
    public void AddArmor(float armorAmount)
    {
        playerArmor.AddArmor((int)armorAmount);
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        HandleJump();

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // Tell footstep system if we're moving
        bool isMoving = input.magnitude > 0.1f && characterController.isGrounded;
        if (footstepAudio != null)
            footstepAudio.SetMoving(isMoving);
    }

    void HandleLook()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        xRotation -= lookInput.y * lookSensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * lookInput.x * lookSensitivity);
    }

    void HandleJump()
    {
        if (characterController.isGrounded)
        {
            velocity.y = -2f;

            if (jumpAction.triggered)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
    }

    public void RestoreHealth(float amount)
    {
        playerHealth.Heal((int)amount);
    }
}
