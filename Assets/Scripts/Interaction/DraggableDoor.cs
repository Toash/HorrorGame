using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Player;

public class DraggableDoor : Interactable
{
    public enum DoorState
    {
        Open, //Enemy can see past door
        Peeking, //Enemy cant see past door
        Closed, // Enemy cant see past door
        Locked
    }
    [Title("Events")]
    public UnityEvent Open;
    public UnityEvent Close;
    public UnityEvent Unlock;
    public UnityEvent Locked;

    [Title("Stats")]
    public bool useLock;
    [ShowIf("useLock")]
    public Lock _lock;
    [ShowIf("useLock")]
    public float lockTryDelay = 2;

    [Space]
    public float dragPointDistance = 2;
    public float speedMultiplier = 60000;
    //public float speedExponentialMultiplier = 3;

    public float moveTolerance = 0.1f;
    [Tooltip("Z axis is forward")]
    public Transform forwardVector;
    public float doorCloseCooldown = .2f;
    public float doorOpenCooldown = .2f;

    [Title("Angles")]
    public float doorCloseAngleFromZero = 1;
    [Tooltip("Anything past this angle and the door is considered open.")]
    public float doorPeakingAngle = 25;

    [Title("Physics")]
    [Tooltip("pivot should be at center for this model")]
    public GameObject DoorModel;
    public HingeJoint joint;


    [Title("Debug")]
    [ShowInInspector,ReadOnly]
    private DoorState state;

    private JointMotor motor;
    private Camera cam;
    private GameObject dragPoint;


    private float doorCloseTimer = 0;
    private float doorOpenTimer = 0;
    private float lockTryTimer = 0;

    public override void Interact()
    {
        if (!useLock)
        {
            SwingLogic();
        }
        else if (useLock && !_lock.locked)
        {
            SwingLogic();
        }

    }
    [Button]
    public void OpenDoor()
    {
        //Get dir where door swings open
        // apply a large force in that direction
        // profit
        Vector3 openingDir = -forwardVector.forward;

        joint.useMotor = true;
        motor.targetVelocity = -speedMultiplier;
        motor.force = 1000;

        joint.motor = motor;
        motor.targetVelocity = 0;

    }

    private IEnumerator ApplyOpenDoorForce()
    {
        yield return null;
    }
    private void SwingLogic()
    {
        joint.useMotor = true;
        
        // Create drag point object 
        if (dragPoint == null)
        {
            dragPoint = new GameObject("Drag point");
            dragPoint.transform.parent = DoorModel.transform;
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        //dragPoint.transform.position = ray.GetPoint(Vector3.Distance(dragPoint.transform.position, cam.transform.position));
        dragPoint.transform.position = ray.GetPoint(dragPointDistance);
        //dragpoint is on th e same plane as th e bottom of door
        dragPoint.transform.position = new Vector3(dragPoint.transform.position.x, forwardVector.position.y, dragPoint.transform.position.z);
        dragPoint.transform.rotation = DoorModel.transform.rotation;

        //Speed up when player mouse moves farther away
        //float delta = Mathf.Pow(Vector3.Distance(dragPoint.transform.position, forwardVector.transform.position), speedExponentialMultiplier);



        Vector3 dirFromForwardVecToDragPoint = (dragPoint.transform.position - forwardVector.position).normalized;
        Ray horizRayThroughDoor = new Ray(forwardVector.position, forwardVector.right);
        float distanceFromRayToDragPoint = Vector3.Cross(horizRayThroughDoor.direction, dragPoint.transform.position - horizRayThroughDoor.origin).magnitude;
        //Debug.Log(distanceFromRayToDragPoint);
        //Apply velocity to door motor
        if (distanceFromRayToDragPoint > moveTolerance)
        {
            if (Vector3.Dot(forwardVector.forward, dirFromForwardVecToDragPoint) > 0)
            {
                //Drag point on the side of the forward vector
                motor.targetVelocity = speedMultiplier  * Time.deltaTime;
            }
            else
            {
                //Drag point on the other side of forward vector
                motor.targetVelocity = -speedMultiplier  * Time.deltaTime;
            }
        }
        else
        {
            motor.targetVelocity = 0;
        }


        //apply the motor
        joint.motor = motor;
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = PlayerSingleton.instance.cam.cam;
        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        doorOpenTimer += Time.deltaTime;
        doorCloseTimer += Time.deltaTime;
        lockTryTimer += Time.deltaTime;

        if(doorOpenTimer> doorOpenCooldown)
        {
            bool doorInClosingAngle = joint.angle >= (0 - doorCloseAngleFromZero) && joint.angle <= (0 + doorCloseAngleFromZero);
            if (doorInClosingAngle)
            {
                if (state != DoorState.Closed)
                {
                    DoorCloseBehaviour();
                    doorCloseTimer = 0;
                }


            }
        }
        if(doorCloseTimer > doorCloseCooldown)
        {
            bool doorPastClosingAndPeekingAngle = joint.angle <= (0 - doorCloseAngleFromZero) || joint.angle >= (0 + doorCloseAngleFromZero);
            if (doorPastClosingAndPeekingAngle)
            {
                if (state != DoorState.Open)
                {
                    DoorOpenBehaviour();
                    doorOpenTimer = 0;
                }

            }
        }
    }

    private void DoorOpenBehaviour()
    {
        state = DoorState.Open;
        Open.Invoke();
    }
    private void DoorPeekBehaviour()
    {
        //Debug.Log("Door peeking");
        state = DoorState.Peeking;
    }
    private void DoorCloseBehaviour()
    {
        state = DoorState.Closed;
        Close.Invoke();
    }
    public override void LookingAt()
    {
        base.LookingAt();
        motor = joint.motor;
        
    }
    public override void NotLookingAt()
    {
        base.NotLookingAt();
        motor.targetVelocity = 0;
        joint.motor = motor;
        joint.useMotor = false;
    }
    public void OnDrawGizmos()
    {
        if(dragPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(dragPoint.transform.position, .25f);
        }

    }
}
