using UnityEngine;
using Player;
using Sirenix.OdinInspector;

public abstract class Interactable : MonoBehaviour
{
    
    public enum InteractType
    {
        Click,
        Hold,
        Persistent //handle unselecting manually
    }
    public InteractType interactType;
    public string InteractText;
    private bool touching;


    [Header("IK")]
    [Space]
    public Transform interactPoint;



    public virtual void Update()
    {
        if(interactType == InteractType.Persistent)
        {
            if (Input.GetKeyUp(PlayerSingleton.instance.interact.interactKey))
            {
                PlayerSingleton.instance.interact.RemoveCurrentInteractable();
            }
        }

    }
    public abstract void Interact();


    public virtual void LookingAt()
    {
        touching = true;
    }
    public virtual void NotLookingAt()
    {
        touching = false;
    }
}
