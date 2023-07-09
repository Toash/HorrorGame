using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectability : MonoBehaviour
{
    public bool hiding;

    public delegate void BoolDelegate(bool b);
    public BoolDelegate PlayerHiding;

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "HidingSpot")
        {
            hiding = true;
            PlayerHiding.Invoke(true);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "HidingSpot")
        {
            hiding = false;
            PlayerHiding.Invoke(false);
        }
    }
}
