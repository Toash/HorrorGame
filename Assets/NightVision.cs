using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class NightVision : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip nvgOn;
    public float nvgOnVolume = .25f;
    public AudioClip nvgOff;
    public float nvgOffVolume = 1;
    public KeyCode ToggleKey = KeyCode.G;
    public Color defaultLightColor;
    public Color NVGLightColor;
    public float defaultFogDensity = .25f;
    public float NVGFogDensity = .15f;
    public PostProcessVolume nightVisionVolume;

    private bool nvgEnabled = false;
    
    void Start()
    {
        DisableNVGEffect();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(ToggleKey))
        {
            if (nvgEnabled)
            {
                nvgEnabled = false;
                DisableNVGEffect();

            }
            else
            {
                nvgEnabled = true;
                EnableNVGEffect();
            }
        }
    }
    private void EnableNVGEffect()
    {
        RenderSettings.ambientLight = NVGLightColor;
        RenderSettings.fogDensity = NVGFogDensity;
        nightVisionVolume.weight = 1;
        audioSource.clip = nvgOn;
        audioSource.volume = nvgOnVolume;
        audioSource.Play();
    }
    private void DisableNVGEffect()
    {
        RenderSettings.ambientLight = defaultLightColor;
        RenderSettings.fogDensity = defaultFogDensity;
        nightVisionVolume.weight = 0;
        audioSource.clip = nvgOff;
        audioSource.volume = nvgOffVolume;
        audioSource.Play();
    }
}
