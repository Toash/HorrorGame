using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Interactable : MonoBehaviour
{
    public string InteractText;
    private bool touching;


    [Header("IK")]
    [Space]
    public Transform interactPoint;



    public virtual void Update()
    {

    }
    public virtual void Interact()
    {
        Debug.Log("interacting");
    }

    public void Touching()
    {
        touching = true;
    }
    public void Nottouching()
    {
        touching = false;
    }
}
