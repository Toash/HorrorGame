using UnityEngine;

public class CameraLeaning : MonoBehaviour
{
    public float leanAngle = 30f;  // Angle for camera leaning
    public float leanSpeed = 5f;   // Speed of camera leaning

    private Quaternion originalRotation;
    private float leanAmount;
    float leanInput;

    private void Start()
    {
        originalRotation = transform.localRotation;
    }

    private void Update()
    {
        // Get input for leaning (assuming you use "Q" and "E" keys)

        if (Input.GetKey(KeyCode.E))
        {
            Debug.Log("e");
            leanInput = 1f;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            leanInput = -1f;
        }
        else
        {
            leanInput = 0;
        }

        // Calculate the new lean amount based on input
        float targetLeanAmount = leanInput * leanAngle;
        leanAmount = Mathf.Lerp(leanAmount, targetLeanAmount, Time.deltaTime * leanSpeed);

        // Apply the rotation to the camera
        transform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, leanAmount);
    }
}