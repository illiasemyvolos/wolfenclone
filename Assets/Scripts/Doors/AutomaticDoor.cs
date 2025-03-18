using UnityEngine;
using UnityEngine.AI;
using System.Collections; // Required for Coroutine support

public class AutomaticDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public Vector3 openPositionOffset = new Vector3(0f, 3f, 0f);
    public float doorSpeed = 3f;
    public float closeDelay = 3f;  // Time before the door closes automatically

    [Header("Obstacle Control")]
    public NavMeshObstacle navMeshObstacle;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpening = false;
    private bool isOpen = false;   // Tracks if door is already open

    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + openPositionOffset;

        if (navMeshObstacle == null)
        {
            navMeshObstacle = GetComponent<NavMeshObstacle>();
        }
    }

    private void Update()
    {
        HandleDoorMovement();

        if (navMeshObstacle != null)
        {
            navMeshObstacle.enabled = !isOpening;
        }
    }

    private void HandleDoorMovement()
    {
        Vector3 targetPosition = isOpening ? openPosition : closedPosition;

        if (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, doorSpeed * Time.deltaTime);
        }
        else
        {
            isOpen = isOpening;  // Track if the door has finished opening/closing
        }
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpening = true;
            isOpen = true;  // Track open state
            StopAllCoroutines();
            StartCoroutine(AutoCloseDoor());  // Start closing countdown
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            isOpening = false;
            isOpen = false;  // Track closed state
        }
    }

    private IEnumerator AutoCloseDoor()
    {
        yield return new WaitForSeconds(closeDelay);

        if (isOpen)  // Ensure the door is still open before closing
        {
            CloseDoor();
        }
    }
}
