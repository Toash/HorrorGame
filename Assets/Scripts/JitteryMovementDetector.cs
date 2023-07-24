using UnityEngine;

public class JitteryMovementDetector : MonoBehaviour
{
    public float movementThreshold = 0.1f; // Adjust this value to control sensitivity of jitter detection

    private Vector3 originalPosition;
    private Vector3 previousPosition;
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalPosition = transform.position;
        previousPosition = originalPosition;
    }

    private void Update()
    {
        if (IsJitteryMovement())
        {
            TeleportToOriginalPosition();
        }

        // Update the previous position at the end of each frame
        previousPosition = transform.position;
    }

    /*
    private bool IsJitteryMovement()
    {
        // Calculate the distance between the previous position and the current position
        float distanceToPreviousPosition = Vector3.Distance(previousPosition, transform.position);

        // If the distance is greater than the movement threshold, consider it jittery movement
        if (distanceToPreviousPosition > movementThreshold)
        {
            // Update the original position to the current position
            originalPosition = transform.position;
            return true;
        }

        return false;
    }
    */
    private bool IsJitteryMovement()
    {
        // Calculate the distance between the previous position and the current position
        float distanceToPreviousPosition = Vector3.Distance(previousPosition, transform.position);

        // Calculate the threshold distance for the current fixed frame based on movementThreshold and fixedDeltaTime
        float frameThreshold = movementThreshold * Time.fixedDeltaTime;

        // If the distance is greater than the frame threshold, consider it jittery movement
        // Update the original position to the current position
        originalPosition = transform.position;
        return distanceToPreviousPosition > frameThreshold;
    }

    private void TeleportToOriginalPosition()
    {
        characterController.enabled = false;
        //transform.position = originalPosition;
        transform.position = new Vector3(originalPosition.x, 1, originalPosition.z);
        characterController.enabled = true;
    }
}
