using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TestDirector : MonoBehaviour
{
    public PlayableDirector director;

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            director.Play();
            Destroy(this.gameObject);
        }
    }
}
