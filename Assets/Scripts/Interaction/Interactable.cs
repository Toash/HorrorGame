using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Player;

public abstract class Interactable : MonoBehaviour
{
    
    public enum InteractType
    {
        Click,
        Hold,
        Persistent //handle unselecting manually
    }
    public UnityEvent interactEvent;
    [Title("Interactable")]
    public InteractType interactType;
    public string InteractText;

    public bool RequiresItem = false;
    [ShowIf("RequiresItem")]
    public PickupSO ItemRequirement;

    [ShowIf("interactType",InteractType.Persistent)]
    public float bailOutRange;

    private bool touching;


    [Header("Optional IK")]
    [Space]
    public Transform interactPoint;

    protected bool PlayerHasItem()
    {
        return PlayerSingleton.instance.interact.PickupInHand == ItemRequirement;
    }



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
    public virtual void Interact()
    {
        if (PlayerSingleton.instance.interact.PickupInHand != ItemRequirement) return;
        interactEvent?.Invoke();
    }


    public virtual void LookingAt()
    {
        touching = true;
    }
    public virtual void NotLookingAt()
    {
        touching = false;
    }
}
