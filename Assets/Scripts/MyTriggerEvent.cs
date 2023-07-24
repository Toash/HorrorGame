using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class MyTriggerEvent : MonoBehaviour
{
    public bool playerCanEnter = false;
    public bool killerCanEnter = false;
    public bool disableColOnEnter = true;

    public UnityEvent EnteredTrigger;
    public UnityEvent ExitTrigger;

    private Collider col;
    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerCanEnter)
        {
            if (other.tag == "Player")
            {
                EnteredTrigger.Invoke();
                if (disableColOnEnter)
                {
                    col.enabled = false;
                }
            }
                
        }
        if(killerCanEnter)
        {
            if (other.tag == "Killer")
            {
                EnteredTrigger.Invoke();
                if (disableColOnEnter)
                {
                    col.enabled = false;
                }
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (playerCanEnter)
        {
            if(other.tag == "Player")
                ExitTrigger.Invoke();
        }
        if(killerCanEnter)
        {
            if(other.tag == "Killer")
                ExitTrigger.Invoke();
        }
    }
}
