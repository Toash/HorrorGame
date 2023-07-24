using UnityEngine;
using UnityEngine.Events;
using System.Collections;


public class KillerOpenNearestDoor : MonoBehaviour
{
    public UnityEvent KillerOpenDoor;
    public void OpenNearestDoor()
    {
        Killer.instance.OpenNearestDoors();
        KillerOpenDoor.Invoke();
    }

    public void OpenNearestDoorDelay(float seconds)
    {
        StartCoroutine(OpenNearestDoorDelayRoutine(seconds));
    }
    private IEnumerator OpenNearestDoorDelayRoutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        OpenNearestDoor();
    }
}
