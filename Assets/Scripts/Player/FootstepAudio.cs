using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepAudio : MonoBehaviour
{
    public AudioClip[] footstepClips;
    public float stepInterval = 0.5f;
    public CharacterController characterController;

    private AudioSource audioSource;
    private float stepTimer;

    private bool isMoving = false; // 👈 Controlled by PlayerController

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (characterController == null)
            characterController = GetComponentInParent<CharacterController>();
    }

    public void SetMoving(bool moving) // 👈 Called externally
    {
        isMoving = moving;
    }

    void Update()
    {
        if (characterController.isGrounded && isMoving)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.PlayOneShot(clip);
    }
}