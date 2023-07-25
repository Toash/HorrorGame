using System.Collections;
using UnityEngine;


public class Button : Interactable
{
    public bool locked = false;
    public delegate void Action();
    public Action Activate;

    public override void Update()
    {
        base.Update();
    }
    public void Unlock()
    {
        locked = false;
    }

    public override void Interact()
    {
        base.Interact();
        if (locked) return;
        Activate.Invoke();
    }
}
