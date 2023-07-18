using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using DG.Tweening;

public class Glimmer : MonoBehaviour
{
    public float showDistance = 2;
    public GameObject model;
    public float quadRotationSpeed = 30f;
    public Vector3 startScale = new Vector3(1f, 1f, 1f);
    public Vector3 endScale = new Vector3(2f, 2f, 2f);
    public float duration = 1f;
    private void Start()
    {
        transform.localScale = startScale;
        ScaleObject();
    }
    void Update()
    {
        transform.LookAt(PlayerSingleton.instance.cam.cam.transform);
        model.transform.Rotate(Vector3.forward, quadRotationSpeed * Time.deltaTime);
        if(Vector3.Distance(PlayerSingleton.instance.transform.position,transform.position) < showDistance)
        {
            model.SetActive(true);
        }
        else
        {
            model.SetActive(false);
        }
    }
    private void ScaleObject()
    {
        // Tween the object's scale from startScale to endScale and back
        transform.DOScale(endScale, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // Reverse the animation by swapping startScale and endScale
                Vector3 temp = startScale;
                startScale = endScale;
                endScale = temp;

                // Restart the animation
                ScaleObject();
            });
    }
}
