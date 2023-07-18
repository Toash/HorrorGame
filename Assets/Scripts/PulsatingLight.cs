using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PulsatingLight : MonoBehaviour
{
    public Light theLight;
    public float LightChangeamount = .5f;
    public float ChangeDuration = .5f;


    private void Start()
    {
        DOTween.To(() => theLight.intensity, x => theLight.intensity = x, theLight.intensity + LightChangeamount, ChangeDuration)
       .SetEase(Ease.InOutSine)
       .SetLoops(-1, LoopType.Yoyo);
    }
    private void Update()
    {
        
    }

}
