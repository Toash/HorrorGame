using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class FlashlightPickup : Pickup
{
    public override void Interact()
    {
        base.Interact();
        PlayerSingleton.instance.flashlight.ReceiveFlashlight();
    }

    public override void PickUp()
    {
        
    }
}
