using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using TMPro;
using Sirenix.OdinInspector;
using DG.Tweening;

/// <summary>
/// Interaction and hands
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Title("Keybinds")]
    public KeyCode dropKey = KeyCode.G;
    public KeyCode useKey = KeyCode.F;
    [Title("Pickups")]
    public UnityEvent CarryingSomethingButTryingToPickSomethingElseUp;

    [ReadOnly]
    public PickupSO PickupInHand = null;
    public Transform HoldPoint;
    public Transform DropPoint;

    [ReadOnly,ShowInInspector]
    public bool CarryingSomething { get { return PickupInHand != null; } }


    [Title("Interaction")]
    public KeyCode interactKey = KeyCode.Mouse0;

    public Camera cam;
    public float interactDistance = 3f;
    public LayerMask interactMask;


    //Hand graphics
    //-----------------------------------------------
    public Transform rightHandTarget;
    public float handInteractionGoToSpeed = .5f;
    public float handInteractTime = .5f;
    public float handInteractGoBackSpeed = .35f;
    //-----------------------------------------------


    [ReadOnly]
    public Interactable currentInteractable;
    [ReadOnly]
    public bool interacting;

    private Vector3 rightHandInitialPos;
    private bool handMoving = false;

    private bool isClick()
    {
        return currentInteractable.interactType == Interactable.InteractType.Click;
    }
    private bool isHold()
    {
        return currentInteractable.interactType == Interactable.InteractType.Hold;
    }
    private bool isPersistent()
    {
        return currentInteractable.interactType == Interactable.InteractType.Persistent;
    }
    private bool InteractableInPersistentRange()
    {
        return (currentInteractable.transform.position - transform.position).magnitude < currentInteractable.bailOutRange;
    }

    public void Start()
    {
        rightHandInitialPos = rightHandTarget.transform.localPosition;
        
    }

    public void Update()
    {
        Interaction();
        if(Input.GetKeyDown(dropKey) && CarryingSomething)
        {
            //Debug.Log("Dropping item");
            DropPickup();
        }
        if(CarryingSomething && PickupInHand.PickupObject.GetComponent<Useable>() != null)
        {
            if (Input.GetKeyDown(useKey))
            {
                //use it
                PickupInHand.PickupObject.GetComponent<Useable>().Use();
                //destroy the pickup in hand

            }
        }

    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(cam.transform.position, cam.transform.forward * interactDistance);
    }
    public void EquipPickup(PickupSO pickup)
    {
        if (!CarryingSomething)
        {
            GameObject model = Instantiate(pickup.EquipModel, HoldPoint.position, HoldPoint.rotation, HoldPoint);
            PickupInHand = pickup;
        }
        else
        {
            CarryingSomethingButTryingToPickSomethingElseUp.Invoke();
        }
    }
    public void DropPickup()
    {
        if (!CarryingSomething) return;

        Transform equipModel = HoldPoint.GetChild(0);
        Destroy(equipModel.gameObject);

        GameObject pickUp = Instantiate(PickupInHand.PickupObject,DropPoint.position,Quaternion.identity);
        PickupInHand = null;

    }
    public void RemoveCurrentInteractable()
    {
        if(currentInteractable != null)
        {
            currentInteractable.NotLookingAt();
            currentInteractable = null;
            interacting = false;
        }

    }
    private void Interaction()
    {

        if (currentInteractable == null && !handMoving) 
        {
            moveRightHandTargetBack();
        }
        
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactDistance, interactMask, QueryTriggerInteraction.Collide))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            if (interactable == null) interactable = hit.transform.GetComponentInParent<Interactable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                currentInteractable.LookingAt();
            }
            //Hitting something but not an interactable
            else if (currentInteractable != null)
            {
                HadInteractableNowDont();
            }
        }
        //Not hitting anythign but still have interactable
        else if (currentInteractable != null)
        {
            HadInteractableNowDont();
        }

        if (currentInteractable != null)
        {
            if (isClick())
            {
                if (Input.GetKeyDown(interactKey))
                {
                    if (currentInteractable.interactPoint != null)
                    {
                        StartCoroutine(moveRightHandTarget(currentInteractable.interactPoint.position));
                    }
                    currentInteractable.Interact();
                    interacting = true;
                }
                return;
            }
            else if (isHold() || isPersistent())
            {
                if (Input.GetKey(interactKey))
                {
                    if (currentInteractable.interactPoint != null)
                    {

                        moveRightHandTargetToPos(currentInteractable.interactPoint.position);
                    }
                    currentInteractable.Interact();
                    interacting = true;
                }
                else
                {
                    moveRightHandTargetBack();
                }

            }
        }
    }

    private IEnumerator moveRightHandTarget(Vector3 pos)
    {
        rightHandTarget.DOMove(pos, handInteractionGoToSpeed, false);
        handMoving = true;
        yield return new WaitForSeconds(handInteractTime);
        rightHandTarget.DOLocalMove(rightHandInitialPos, handInteractGoBackSpeed, false);
        handMoving = false ;
    }

    private void moveRightHandTargetToPos(Vector3 pos)
    {
        rightHandTarget.DOMove(pos, handInteractionGoToSpeed, false);
    }
    private void moveRightHandTargetBack()
    {
        rightHandTarget.DOLocalMove(rightHandInitialPos, handInteractGoBackSpeed, false);
    }

    private void HadInteractableNowDont()
    {
        if (currentInteractable.interactType == Interactable.InteractType.Persistent && InteractableInPersistentRange() && interacting) return;
        RemoveCurrentInteractable();
    }


}
