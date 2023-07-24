using System.Collections;
using UnityEngine;


public class Button : Interactable
{

    public delegate void Action();
    public Action Activate;

    public override void Update()
    {
        base.Update();
    }

    public override void Interact()
    {
        base.Interact();
        Activate.Invoke();
    }
}
