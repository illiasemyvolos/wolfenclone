using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Look Settings")]
    public float lookSensitivity = 2f;
    public Transform cameraTransform;
    private float xRotation = 0f;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Stagger Settings")]
    public float staggerDuration = 1f;  
    public float staggerSpeedMultiplier = 0.3f;

    [Header("UI References")]
    public DeathScreenController deathScreenController; // Public reference for death screen
    public DamageScreenController damageScreenController;

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;

    private CharacterController controller;
    private Vector3 velocity;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;

    private bool isStaggered = false; 

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        currentHealth = maxHealth;

        // Auto-assign Death Screen if not set manually
        if (deathScreenController == null)
        {
            deathScreenController = FindObjectOfType<DeathScreenController>();
        }

        if (damageScreenController == null)
        {
            damageScreenController = FindObjectOfType<DamageScreenController>();
        }
    }

    

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0f);
        Debug.Log($"Player took {damage} damage. Current HP: {currentHealth}");

        if (damageScreenController != null)
        {
            damageScreenController.ShowDamageEffect();
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Player has died!");
            
            if (deathScreenController != null)
            {
                deathScreenController.ShowDeathScreen();
            }
        }
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
        controller.Move(move * moveSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
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
        if (controller.isGrounded)
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
        if (currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            Debug.Log($"💉 Player healed for {amount} HP. Current HP: {currentHealth}");
        }
    }

    public void Stagger()
    {
        if (!isStaggered)
        {
            StartCoroutine(StaggerCoroutine());
        }
    }

    private IEnumerator StaggerCoroutine()
    {
        isStaggered = true;
        Debug.Log("💥 Player staggered!");

        float originalSpeed = moveSpeed;
        moveSpeed *= staggerSpeedMultiplier;

        yield return new WaitForSeconds(staggerDuration);

        moveSpeed = originalSpeed;
        isStaggered = false;

        Debug.Log("✅ Player recovered from stagger.");
    }
}
