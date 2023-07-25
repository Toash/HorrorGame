using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Player;

public class Lock : Interactable
{
    public UnityEvent UnlockEvent;
    public UnityEvent TryUnlock;
    public bool locked = true;
    public Rigidbody rb;
    
    //public PickupSO KeyToUnlock;
    //public float lockTryDelay;
    //private float lockTryTimer = 0;

    public override void Interact()
    {
        base.Interact();
        Unlock();
    }
    public void Unlock()
    {
        Debug.Log("Unlocking");
        UnlockEvent.Invoke();
        rb.isKinematic = false;
        rb.useGravity = true;
        locked = false;
    }
}
