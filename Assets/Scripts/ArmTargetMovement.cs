using UnityEngine;

public class ArmTargetMovement : MonoBehaviour
{
    public Transform cameraTransform;   // Reference to the camera's transform
    public float rotationSpeed = 1f;    // Speed of rotation
    public float maxOffset = 0.1f;      // Maximum offset for movement

    private Vector3 initialPosition;    // Initial position of the arm target
    private Quaternion initialRotation; // Initial rotation of the arm target

    private void Start()
    {
        // Store the initial position and rotation
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        // Calculate the rotation angle based on the camera's rotation
        Quaternion targetRotation = Quaternion.Euler(0f, cameraTransform.rotation.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Calculate the movement offset based on the camera's position
        Vector3 targetOffset = cameraTransform.position - initialPosition;
        targetOffset.y = 0f; // Ignore any vertical movement

        // Apply the movement offset with clamping
        Vector3 clampedOffset = Vector3.ClampMagnitude(targetOffset, maxOffset);
        transform.position = initialPosition + clampedOffset;
    }
}
