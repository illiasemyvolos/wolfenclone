using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    [Header("Door Reference")]
    public AutomaticDoor door;  // Reference to the actual door object

    [Header("Delay Settings")]
    public float closeDelay = 2f;  // Delay before closing the door

    private Coroutine closeDoorCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (door != null)
            {
                if (closeDoorCoroutine != null) 
                {
                    StopCoroutine(closeDoorCoroutine); // Cancel closing if already scheduled
                }

                door.OpenDoor();
                Debug.Log("✅ Player detected — Opening door.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (door != null)
            {
                closeDoorCoroutine = StartCoroutine(DelayedClose());
            }
        }
    }

    // Coroutine to delay closing the door
    private IEnumerator DelayedClose()
    {
        Debug.Log($"⏳ Waiting {closeDelay} seconds before closing the door...");
        yield return new WaitForSeconds(closeDelay);

        if (door != null)
        {
            door.CloseDoor();
            Debug.Log("❌ Door closed after delay.");
        }
    }
}
