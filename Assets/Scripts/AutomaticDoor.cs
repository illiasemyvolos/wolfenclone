using UnityEngine;
using UnityEngine.AI;

public class AutomaticDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public Vector3 openPositionOffset = new Vector3(0f, 3f, 0f);
    public float doorSpeed = 3f;

    [Header("Obstacle Control")]
    public NavMeshObstacle navMeshObstacle;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpening = false;

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
        transform.position = Vector3.Lerp(transform.position, targetPosition, doorSpeed * Time.deltaTime);
    }

    public void OpenDoor() => isOpening = true;

    public void CloseDoor() => isOpening = false;
}
