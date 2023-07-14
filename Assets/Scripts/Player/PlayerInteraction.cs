using UnityEngine;
using System.Collections;
using TMPro;
using Sirenix.OdinInspector;
using DG.Tweening;

/// <summary>
/// Interaction and hands
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.Mouse0;
    [ReadOnly]
    public bool CarryingSomething;
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

    private Vector3 rightHandInitialPos;

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
    private bool InteractableInRange()
    {
        return (currentInteractable.transform.position - transform.position).magnitude < interactDistance;
    }

    public void Start()
    {
        rightHandInitialPos = rightHandTarget.transform.localPosition;
        
    }

    public void Update()
    {
        if(currentInteractable == null)
        {
            moveRightHandTargetBack();
        }
        RaycastHit hit;
       if(Physics.Raycast(cam.transform.position,cam.transform.forward,out hit,interactDistance, interactMask, QueryTriggerInteraction.Collide))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            if (interactable == null) interactable = hit.transform.GetComponentInParent<Interactable>();
            if(interactable != null)
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

       if(currentInteractable != null)
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
                }
                return;
            }
            if (isHold() || isPersistent())
            {
                if (Input.GetKey(interactKey))
                {
                    if (currentInteractable.interactPoint != null)
                    {
                        moveRightHandTargetToPos(currentInteractable.interactPoint.position);
                    }
                    currentInteractable.Interact();
                }
                else
                {
                    moveRightHandTargetBack();
                }

            }
        }

    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(cam.transform.position, cam.transform.forward * interactDistance);
    }
    public void RemoveCurrentInteractable()
    {
        if(currentInteractable != null)
        {
            currentInteractable.NotLookingAt();
            currentInteractable = null;
        }

    }

    private IEnumerator moveRightHandTarget(Vector3 pos)
    {
        rightHandTarget.DOMove(pos, handInteractionGoToSpeed, false);
        yield return new WaitForSeconds(handInteractTime);
        rightHandTarget.DOLocalMove(rightHandInitialPos, handInteractGoBackSpeed, false);
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
        if (currentInteractable.interactType == Interactable.InteractType.Persistent && InteractableInRange()) return;
        RemoveCurrentInteractable();
    }
}
