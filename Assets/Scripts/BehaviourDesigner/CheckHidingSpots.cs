using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


/// <summary>
/// Check hiding spots in a radius, can either randomly check spots, check spots the player isnt in, or check the spot that the player is in
/// </summary>
public class CheckHidingSpots : Action
{
    enum CheckMode
    {
        NotPlayer,
        Player
    }

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Speed to move between hiding spots")]
    public float Speed = 2f;
    public int HidingSpotsToCheck = 2;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Distance from hiding spot to check inside")]
    public float CheckDistance = 1f;
    public float CheckRadius = 10f;
    public LayerMask hidingSpotMask;

    private List<HidingSpot> spots;
    private NavMeshAgent agent;

    public override void OnAwake()
    {
        agent = GetComponent<NavMeshAgent>();
        spots = new List<HidingSpot>();
    }
    public override void OnStart()
    {
        agent.speed = Speed;

        // Cast sphere to check for hiding spots
        Collider[] cols = Physics.OverlapSphere(transform.position, CheckRadius, hidingSpotMask, QueryTriggerInteraction.Collide);
        foreach (Collider col in cols)
        {
            //populate hiding spot list
            if (col.GetComponentInParent<HidingSpot>() != null)
            {
                spots.Add(col.GetComponentInParent<HidingSpot>());
            }
            else if (col.GetComponent<HidingSpot>() != null)
            {
                spots.Add(col.GetComponent<HidingSpot>());
            }
        }
    }
    public override TaskStatus OnUpdate()
    {
        
        return TaskStatus.Running;
    }

    private void GoToHidingSpot(HidingSpot spot)
    {
        agent.SetDestination(spot.transform.position);
    }

}
