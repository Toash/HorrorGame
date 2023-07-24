using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopKiller : MonoBehaviour
{
    public bool onlyInChase = true;
    public bool onlyInSearch = true;
    public void Stop(float seconds)
    {
        if (onlyInChase)
        {
            if(Killer.instance.state == Killer.State.Chasing)
                Killer.instance.Stop(seconds);
        }
        if (onlyInSearch)
        {
            if (Killer.instance.state == Killer.State.Searching)
                Killer.instance.Stop(seconds);
        }

    }
}
