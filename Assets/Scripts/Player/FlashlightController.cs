using UnityEngine;
using Sirenix.OdinInspector;

public class FlashlightController : MonoBehaviour
{
    public bool hasFlashlight;
    [Title("Flicker")]
    public float intensityRange;
    public float flickerSpeed = 5;
    private float targetIntensity;
    private float minIntensity;
    private float maxIntensity;

    [Title("Other")]
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
        RandomLightIntensity();
        targetRotation = cameraTransform.rotation;
        minIntensity = light.intensity - intensityRange;
        maxIntensity = light.intensity + intensityRange;
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
        if (hasFlashlight)
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
            FlickerLight();
        }

    }
    private void LateUpdate()
    {
        targetRotation = Quaternion.Lerp(targetRotation, cameraTransform.rotation, rotationSpeed * Time.deltaTime);

        transform.rotation = targetRotation;
    }

    public void ReceiveFlashlight()
    {
        hasFlashlight = true;
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

    private void FlickerLight()
    {
        if (light.enabled)
        {
            light.intensity = Mathf.Lerp(light.intensity, targetIntensity, flickerSpeed * Time.deltaTime);

            bool reachedTargetIntensity = Mathf.Abs(light.intensity - targetIntensity) <= 0.05f;
            if (reachedTargetIntensity)
            {
                RandomLightIntensity();
            }
        }

    }
    private void RandomLightIntensity()
    {
        targetIntensity = Random.Range(minIntensity, maxIntensity);
    }
}