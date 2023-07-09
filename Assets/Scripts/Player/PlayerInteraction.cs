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
    [ReadOnly]
    public bool CarryingSomething;
    public Camera cam;
    public float interactDistance = 3f;
    public LayerMask interactMask;

    public Transform rightHandTarget;
    [ReadOnly]
    public Interactable currentInteractable;

    private Vector3 rightHandInitialPos;

    public void Start()
    {
        rightHandInitialPos = rightHandTarget.transform.localPosition;
        
    }

    public void Update()
    {
        RaycastHit hit;
       if(Physics.Raycast(cam.transform.position,cam.transform.forward,out hit,interactDistance, interactMask, QueryTriggerInteraction.Collide))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            if(interactable != null)
            {
                currentInteractable = interactable;
                currentInteractable.Touching();
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (interactable.interactPoint != null)
                    {
                        StartCoroutine(moveRightHandTarget(interactable.interactPoint.position));
                    }
                    interactable.Interact();

                }
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

    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(cam.transform.position, cam.transform.forward * interactDistance);
    }

    private IEnumerator moveRightHandTarget(Vector3 pos)
    {
        rightHandTarget.DOMove(pos, 1, false);
        yield return new WaitForSeconds(.6f);
        rightHandTarget.DOLocalMove(rightHandInitialPos, 1, false);
    }

    private void HadInteractableNowDont()
    {
        currentInteractable.Nottouching();
        currentInteractable = null;
    }
}
