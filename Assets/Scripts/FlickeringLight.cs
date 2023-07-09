using UnityEngine;
using System.Collections;

public class FlickeringLight : MonoBehaviour
{
    public float minIntensity = 0.2f;    // Minimum light intensity
    public float maxIntensity = 1f;      // Maximum light intensity
    public float flickerSpeed = 5f;      // Speed of flickering

    private Light lightSource;
    private float targetIntensity;

    private void Start()
    {
        lightSource = GetComponent<Light>();
        targetIntensity = Random.Range(minIntensity, maxIntensity);
        StartCoroutine(FlickerLight());
    }

    private IEnumerator FlickerLight()
    {
        while (true)
        {
            float currentIntensity = lightSource.intensity;
            float newIntensity = Mathf.Lerp(currentIntensity, targetIntensity, flickerSpeed * Time.deltaTime);
            lightSource.intensity = newIntensity;

            if (Mathf.Abs(currentIntensity - targetIntensity) <= 0.05f)
            {
                targetIntensity = Random.Range(minIntensity, maxIntensity);
            }

            yield return null;
        }
    }
}