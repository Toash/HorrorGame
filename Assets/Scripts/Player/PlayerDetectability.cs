using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerDetectability : MonoBehaviour
{
    [ReadOnly]
    public bool hiding;

    public delegate void BoolDelegate(bool b);
    public BoolDelegate PlayerHiding;

    public void OnTriggerEnter(Collider other)
    {

        if(other.tag == "HidingSpot")
        {
            ReliableOnTriggerExit.NotifyTriggerEnter(other, gameObject, OnTriggerExit);


            HidingSpot spot = other.GetComponent<HidingSpot>();

            spot.PlayerInHidingSpot = true;

            hiding = true;
            PlayerHiding.Invoke(true);
        }
    }
    public void OnTriggerExit(Collider other)
    {

        if (other.tag == "HidingSpot")
        {
            ReliableOnTriggerExit.NotifyTriggerExit(other, gameObject);

            HidingSpot spot = other.GetComponent<HidingSpot>();

            spot.PlayerInHidingSpot = false;

            hiding = false;
            PlayerHiding.Invoke(false);
        }
    }
}
