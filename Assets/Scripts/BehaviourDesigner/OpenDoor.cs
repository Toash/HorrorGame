using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class OpenDoor : Action
{
    public Monster monster;
    public Door nearestDoor;

    public float DoorSearchRadius = 3;
    public float OpenDelay = 2f;
    public LayerMask doorMask;
    private float timer = 0;
    private NavMeshAgent agent;

    //Run only once
    public override void OnAwake()
    {
        base.OnAwake();
        agent = GetComponent<NavMeshAgent>();
    }
    public override void OnStart()
    {
        base.OnStart();
        nearestDoor = lookForDoor();
        agent.isStopped = true;
        timer = 0;
    }
    public override TaskStatus OnUpdate()
    {
        if (nearestDoor == null)
        {
            Debug.LogError("Monsters nearest door is null");
            return TaskStatus.Failure;
        }

        timer += Time.deltaTime;
        if (timer > OpenDelay)
        {
            //open door.
            Debug.Log("Monster opening door");
            nearestDoor.Open();
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private Door lookForDoor()
    {
        Debug.Log("Monster trying to look for door");
        RaycastHit hit;
        //cast sphere
        Collider[] cols = Physics.OverlapSphere(transform.position, DoorSearchRadius, doorMask, QueryTriggerInteraction.Collide);
        {
            foreach (Collider col in cols)
            {
                if (col.GetComponent<Door>() != null)
                {
                    return col.GetComponent<Door>();
                }
                else if ((col.GetComponentInParent<Door>() != null))
                {
                    return col.GetComponentInParent<Door>();
                }
            }
            return null;
        }

    }
}