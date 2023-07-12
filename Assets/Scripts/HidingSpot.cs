using UnityEngine;
using UnityEngine.Events;

public class HidingSpot : MonoBehaviour
{
    public Collider col;
    public bool PlayerInHidingSpot;
    public UnityEvent HidingSpotChecked;
    
    public void CheckHidingSpot()
    {
        if (PlayerInHidingSpot) Debug.Log("Player got caught!");
        HidingSpotChecked.Invoke();
    }

    public void EnableHidingSpot()
    {
        col.enabled = true;
    }

    public void DisableHidingSpot()
    {
        col.enabled = false;
    }
}
