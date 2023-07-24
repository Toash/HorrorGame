using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class LighterController : MonoBehaviour
{
    [Title("Events")]
    public UnityEvent OnEvent;
    public UnityEvent OffEvent;

    [Title("Stats")]
    public bool hasFluid = true;
    public float hasFluidRange = 6f;
    public float hasFluidIntensity = .6f;
    public float noFluidRange = 4;
    public float noFluidIntensity = .45f;

    [Title("Flicker")]
    public float intensityRange;
    public float flickerSpeed = 5;
    private float targetIntensity;
    private float minIntensity;
    private float maxIntensity;

    [Title("Other")]
    public bool hasLighter;
    public bool startEnabled = false;
    public KeyCode flashLightKey = KeyCode.R;
    new public Light light;

    private void Start()
    {
        RandomLightIntensity();


        targetIntensity = light.intensity;
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
        if (hasLighter)
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
        if(light.enabled == true)
        {
            if (hasFluid)
            {
                light.range = hasFluidRange;
                minIntensity = hasFluidIntensity - intensityRange;
                maxIntensity = hasFluidIntensity + intensityRange;
            }
            else
            {
                light.range = noFluidRange;
                minIntensity = noFluidIntensity - intensityRange;
                maxIntensity = noFluidIntensity + intensityRange;
            }
        }
    }
    private void LateUpdate()
    {
        //minIntensity = light.intensity - intensityRange;
        //maxIntensity = light.intensity + intensityRange;
        FlickerLight();
    }

    public void ReceiveFlashlight()
    {
        hasLighter = true;
    }


    private void On()
    {
        OnEvent.Invoke();
        light.enabled = true;
        
    }
    private void Off()
    {
        OffEvent.Invoke();
        light.enabled = false;
        
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