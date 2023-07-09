using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    /// <summary>
    /// Fades in and out audio
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;
        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
        }

        public AudioMixer mixer;
        public AudioMixerGroup monster;
        public AudioMixerGroup ambience;

        public void FadeInChaseAudio()
        {
            FadeMixerGroup.StartFade(mixer, "ChaseVolume", .5f, 1);
        }
        public void FadeOutChaseAudio()
        {
            FadeMixerGroup.StartFade(mixer, "ChaseVolume", 2, 0);
        }
    }
}


