using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public bool startEnabled = false;
    public GameObject flashLightTarget;
    public KeyCode flashLightKey = KeyCode.R;
    new public Light light;
    public Transform cameraTransform;
    public float rotationSpeed = 2f;
    public AudioSource source;
    public AudioClip onSound;
    public AudioClip offSound;

    private Quaternion targetRotation;

    private void Start()
    {
        targetRotation = cameraTransform.rotation;
        if (startEnabled)
        {
            On();
        }
        else
        {
            Off();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(flashLightKey))
        {
            if (light.enabled)
            {
                Off();
            }
            else
            {
                On();
            }
        }
    }
    private void LateUpdate()
    {
        targetRotation = Quaternion.Lerp(targetRotation, cameraTransform.rotation, rotationSpeed * Time.deltaTime);

        transform.rotation = targetRotation;
    }

    private void PlaySound(AudioClip sound) 
    {
        source.clip = sound;
        source.Play();
    }

    private void On()
    {
        light.enabled = true;
        PlaySound(onSound);
    }
    private void Off()
    {
        light.enabled = false;
        PlaySound(offSound);
    }


}