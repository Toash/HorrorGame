using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using RootMotion.FinalIK;
using Audio;
using Player;
using Sirenix.OdinInspector;

/// <summary>
/// this class is an absolute monolith
/// </summary>
public class Killer : MonoBehaviour
{
    public static Killer instance;


    public enum State
    {
        Stop,
        Patrol,
        Chasing,
        Searching,
        Interacting
    }

    public State state = State.Patrol;

    [Title("Stats")]
    public float openingDoorTime = 2;
    public float attackCooldown = 3f;

    [Title("Patrol")]
    public float patrolSpeed = 1.5f;
    [Tooltip("Follows in top to bottom order")]public GameObject[] PatrolPoints;
    [ShowInInspector, ReadOnly] private int currentPatrolIndex = 0;

    [Title("Chase")]
    public float chaseSpeed = 2;

    [Title("Search")]
    public float searchSpeed = 1.75f;

    [Title("Interacting")]
    public LayerMask interactableMask;

    [Title("Field of View")]
    [Range(0, 360)] public float ViewAngle = 160;
    public float ViewDistance = 10;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float obstacleSpherecastRadius = .4f;

    [Title("Hearing")]
    public LayerMask soundMask;

    [Title("Sounds")]
    public AudioSource localSource;
    public AudioSource[] alertSounds;
    public float alertSoundCooldown = 8f;
    
    public AudioSource chaseMusic;
    public float chaseMusicFadeInSpeed = 5f;
    public float chaseMusicFadeOutSpeed = .15f;

    public AudioClip[] scarySounds;
    public float scarySoundsDelay = 3f;
    public bool playScarySounds;

    [Title("References")]

    [Tooltip("Raycasts will start from here")] public Transform eyeTransform;
    public Animator anim;
    public NavMeshAgent agent;
    public LookAtIK lookAtIK;

    [Title("Debug")]
    [ReadOnly] public bool ChasingPlayer = false;


    private float attackTimer = Mathf.Infinity;
    private float scarySoundTimer = Mathf.Infinity;
    private float alertSoundTimer = Mathf.Infinity;

    //Player tracking
    private Transform playerTrans;
    private Vector3 playerLastKnownLocation;

    public bool CanPlayAlertSound()
    {
        return (alertSoundTimer > alertSoundCooldown);
    }
    public bool CanAttack()
    {
        return (attackTimer > attackCooldown);
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        playerTrans = PlayerSingleton.instance.GetComponent<Transform>();
        
    }

    private void Update()
    {
        UpdateTimers();

        ControlChaseMusic();

        ScanForPlayer();
        ListenForSounds();

        StateMachine();
        Animation();
    }

    private void StateMachine()
    {
        switch (state)
        {
            case State.Stop:
                return;
            case State.Patrol:
                agent.speed = patrolSpeed;

                GoToCurrentPatrolPoint();
                // Check if reach destination, if so, go to next destination
                bool agentReachedPatrolPoint = agent.remainingDistance < .5f;
                if (ReachedDestination())
                {
                    SetNextPatrolPoint();
                }
                break;
            case State.Chasing:
                
                agent.speed = chaseSpeed;

                // Go towards player, if break los go to last seen position.
                // maybe cast a circle, if player in it keep chasing regardless of los break.
                if (HasLOSWithPlayer())
                {
                    agent.SetDestination(playerTrans.position);
                }
                else
                {
                    //Go to last player pos.
                    playerLastKnownLocation = playerTrans.position;
                    state = State.Searching;
                }

                break;

            // set last known player pos before calling this
            case State.Searching:
                agent.speed = searchSpeed;
                GoToLastKnownPlayerPos();

                if (ReachedDestination())
                {
                    anim.SetTrigger("LookAround");
                    Stop(4);
                    //Should cast a range to see if player is in it, if so, chase the player.
                    state = State.Patrol;
                }

                break;
            case State.Interacting:
                // Cast collider then get nearest interactable. interact with that.
                // Monster can only interact with certain interactables?
                break;
        }
    }
    public void Stop(float seconds)
    {
        StartCoroutine(IEStop(seconds));
    }
    private IEnumerator IEStop(float seconds)
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(seconds);
        agent.isStopped = false;
    }
    private Vector3 DirToPlayer()
    {
        return (playerTrans.position - eyeTransform.position).normalized;
    }
    private Vector3 DirToPlayerNoY()
    {
        return (new Vector3(playerTrans.position.x, eyeTransform.position.y, playerTrans.position.z) - eyeTransform.position).normalized;
    }
    private float DstToPlayer()
    {
        return Vector3.Distance(eyeTransform.position, playerTrans.position);
    }

    private void GoToCurrentPatrolPoint()
    {
        agent.SetDestination(PatrolPoints[currentPatrolIndex].transform.position);
    }
    private void SetNextPatrolPoint()
    {
        bool reachedEnd = currentPatrolIndex >= (PatrolPoints.Length - 1);
        if (reachedEnd)
        {
            currentPatrolIndex = 0;
        }
        else
        {
            currentPatrolIndex += 1;
        }
    }


    private void Animation()
    {
        SetIKToPlayerCam();
        anim.SetFloat("Speed", agent.velocity.magnitude);
        float dest;
        
        if (state == State.Chasing)
        {
            dest = 1f;
        }
        else
        {
            //Dont look at player
            dest = 0f;
        }
        lookAtIK.solver.IKPositionWeight = Mathf.Lerp(lookAtIK.solver.IKPositionWeight, dest, Time.deltaTime*5);
    }

    private void UpdateTimers()
    {
        scarySoundTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;
        alertSoundTimer += Time.deltaTime;
    }
    
    //TODO: Optimize
    [Button,PropertyOrder(-1)]
    public void OpenNearestDoors()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 3, interactableMask, QueryTriggerInteraction.Collide);
        foreach(Collider col in cols)
        {
            if(col.GetComponentInParent<DraggableDoor>() != null)
            {
                col.GetComponentInParent<DraggableDoor>().OpenDoor();
            }
        }
    }

    //Field of view
    //------------------------------------------------------------------
    //returns normalized dir from angle
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            //angle is local
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    //-------------------------------------------AWARENESS-----------------------------------------------------------------
    //------------------------------------------------------------------------------------------------------------
    //Check for player so can begin chase.
    private void ScanForPlayer()
    {
        if (state != State.Chasing)
        {

            //Player in range
            if (Physics.Raycast(eyeTransform.position, DirToPlayer(), ViewDistance, playerMask))
            {
                //Check angle, should not care about y axis
                if (Vector3.Angle(eyeTransform.forward, DirToPlayerNoY()) < ViewAngle / 2)
                {
                    //Check obstacles
                    //TODO: Spherecast
                    Ray ray = new Ray(eyeTransform.position, DirToPlayer());
                    if (!Physics.SphereCast(ray,obstacleSpherecastRadius, DstToPlayer(), obstacleMask,QueryTriggerInteraction.Ignore))
                    {
                        //Debug.Log("Can see player!");
                        //PlayRandomAlertSound();
                        state = State.Chasing;
                    }
                }
            }
        }
    }

    private void ListenForSounds()
    {
        if(state != State.Chasing)
        {
            if (Physics.CheckSphere(eyeTransform.position, .5f, soundMask, QueryTriggerInteraction.Collide))
            {
                Debug.Log("Killer heard a noise");
                if (state == State.Patrol || state == State.Stop)
                    PlayRandomAlertSound();
                playerLastKnownLocation = playerTrans.position;
                state = State.Searching;
            }
        }

    }

    //Check if line of sight is possible while chasing the player
    private bool HasLOSWithPlayer()
    {
        if (Physics.Raycast(eyeTransform.position, DirToPlayer(), ViewDistance, playerMask))
        {
            if (!Physics.Raycast(eyeTransform.position, DirToPlayer(), DstToPlayer(), obstacleMask))
            {
                //Player in range and no obstacle in way
                return true;
            }
        }
        return false;
    }
    //----------------------------------------------------------------------------------------------

    private void GoToLastKnownPlayerPos()
    {
        agent.SetDestination(playerLastKnownLocation);
    }


    private void SetIKToPlayerCam()
    {
        lookAtIK.solver.IKPosition = PlayerSingleton.instance.cam.cam.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (attackTimer > attackCooldown)
            {
                //Damage
                //Debug.Log("Monster attacking");
                PlayerHealth health = other.GetComponent<PlayerHealth>();
                anim.SetTrigger("Attack");
                attackTimer = 0;
            }
        }
    }

    private void PlayRandomScarySound()
    {
        int size = scarySounds.Length;
        int index = Random.Range(0, size);
        localSource.clip = scarySounds[index];
        localSource.Play();
    }

    //States
    // -----------------------------------------------------------
    
    private void Wander(float distancePerTry, int tries)
    {
        float currentDistance = distancePerTry;

        //Get random direction and distance

        var x = Random.Range(-1f, 1f);
        var z = Random.Range(-1f, 1f);
        Vector3 randomDir = new Vector3(x, 0, z).normalized;
        RaycastHit hit;
        if(Physics.Raycast(eyeTransform.position,randomDir,out hit, currentDistance, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            //hit wall
            // go to spot near wall
            Debug.DrawLine(eyeTransform.position, hit.point,Color.green,5);
            agent.SetDestination(hit.point);
        }
        else
        {
            //hit Nothing
            //just go there then
            Vector3 point = new Ray(eyeTransform.position, randomDir).GetPoint(currentDistance);
            Debug.DrawLine(eyeTransform.position, point, Color.green, 5);
            agent.SetDestination(point);
        }

        //Go there
        //wait for some time
        //repeat (if multiple tries)
        
    }
    [Button]
    public void EnterChaseState()
    {
        state = State.Chasing;
    }
    //------------------------------------------------------------------
    public void OnDrawGizmos()
    {
        Vector3 viewAngleA = this.DirFromAngle(-this.ViewAngle / 2, false);
        Vector3 viewAngleB = this.DirFromAngle(this.ViewAngle / 2, false);

        Gizmos.color = Color.red;
        GizmosExtensions.DrawWireArc(this.eyeTransform.position, eyeTransform.forward, ViewAngle, ViewDistance);
        if (playerTrans != null)
            Gizmos.DrawLine(eyeTransform.position, new Vector3(playerTrans.position.x, eyeTransform.position.y, playerTrans.position.z));

        
    }

    private bool ReachedDestination()
    {
        // Check if we've reached the destination
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //Audio

    [Button]
    private void PlayRandomAlertSound()
    {
        if (CanPlayAlertSound())
        {
            int i = Random.Range(0, alertSounds.Length);
            alertSounds[i].Play();
            alertSoundTimer = 0;
        }
    }

    private void ControlChaseMusic()
    {
        if (state == State.Chasing)
        {
            chaseMusic.volume = Mathf.Lerp(chaseMusic.volume, 1, Time.deltaTime * chaseMusicFadeInSpeed);
        }
        else
        {
            chaseMusic.volume = Mathf.Lerp(chaseMusic.volume, 0, Time.deltaTime * chaseMusicFadeOutSpeed);
        }
    }
}
