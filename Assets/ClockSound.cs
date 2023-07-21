using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockSound : MonoBehaviour
{
    public float interval = 1;
    public AudioSource tickSound;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > interval)
        {
            tickSound.Play();
            timer = 0;
        }
    }
}
