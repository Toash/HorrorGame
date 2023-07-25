using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Fusebox : Interactable
{
    public UnityEvent AllFusesOn;
    public UnityEvent FirstFuse;
    public UnityEvent SecondFuse;
    public UnityEvent ThirdFuse;
    public UnityEvent FourthFuse;

    int currentFuses = 0;
    


    //Required item is fuse
    public override void Update()
    {
        base.Update();
    }

    public override void Interact()
    {
        base.Interact();
        // add fuse
        currentFuses += 1;
        switch (currentFuses)
        {
            case 1:
                FirstFuse.Invoke();
                break;
            case 2:
                SecondFuse.Invoke();
                break;
            case 3:
                ThirdFuse.Invoke();
                break;
            case 4:
                FourthFuse.Invoke();
                AllFusesOn.Invoke();
                break;
            default:
                return;
        }
    }

}
