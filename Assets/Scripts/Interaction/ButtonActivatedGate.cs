using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonActivatedGate : MonoBehaviour
{
    public UnityEvent Activate;
    public Button connectedButton;

    private void OnEnable()
    {
        connectedButton.Activate += Open;
    }
    private void OnDisable()
    {
        connectedButton.Activate -= Open;
    }
    private void Open()
    {
        Activate.Invoke();
        print("Open gate");
    }

}
