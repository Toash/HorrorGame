using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class DraggableDoor : Interactable
{
    public float speedMultiplier = 60000;
    public float moveTolerance = 0.1f;
    [Tooltip("Z axis is forward")]
    public Transform forwardVector;
    [Tooltip("pivot should be at center for this model")]
    public GameObject DoorModel;
    public HingeJoint joint;
    private JointMotor motor;
    
    
    private Camera cam;
    private GameObject dragPoint;
    

    public override void Interact()
    {
        joint.useMotor = true;
        // Create drag point object 
        if (dragPoint == null)
        {
            dragPoint = new GameObject("Drag point");
            dragPoint.transform.parent = DoorModel.transform;
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        dragPoint.transform.position = ray.GetPoint(Vector3.Distance(dragPoint.transform.position, cam.transform.position));
        //dragpoint is on th e same plane as th e bottom of door
        dragPoint.transform.position = new Vector3(dragPoint.transform.position.x, forwardVector.position.y, dragPoint.transform.position.z);
        dragPoint.transform.rotation = DoorModel.transform.rotation;

        //Speed up when player mouse moves farther away
        float delta = Mathf.Pow(Vector3.Distance(dragPoint.transform.position, DoorModel.transform.position), 3);

        

        Vector3 dirFromForwardVecToDragPoint = (dragPoint.transform.position - forwardVector.position).normalized;
        Ray horizRayThroughDoor = new Ray(forwardVector.position, forwardVector.right);
        float distanceFromRayToDragPoint = Vector3.Cross(horizRayThroughDoor.direction,dragPoint.transform.position - horizRayThroughDoor.origin).magnitude;
        Debug.Log(distanceFromRayToDragPoint);
        //Apply velocity to door motor
        if (distanceFromRayToDragPoint > moveTolerance)
        {
            if (Vector3.Dot(forwardVector.forward, dirFromForwardVecToDragPoint) > 0)
            {
                //Drag point on the side of the forward vector
                motor.targetVelocity = delta * speedMultiplier * Time.deltaTime;
            }
            else
            {
                //Drag point on the other side of forward vector
                motor.targetVelocity = -delta * speedMultiplier * Time.deltaTime;
            }
        }
        else
        {
            motor.targetVelocity = 0;
        }


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
}
