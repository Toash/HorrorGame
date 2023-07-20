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
    public enum State
    {
        Stop,
        Patrol,
        Chasing,
        Searching,
        Interacting
    }

    public State state = State.Patrol;

    [Title("Patrol")]
    [Tooltip("Follows in top to bottom order")]
    public GameObject[] PatrolPoints;
    [ShowInInspector, ReadOnly] private int currentPatrolIndex = 0;

    [Title("Stats")]
    [Title("Field of View")]
    [Range(0, 360)] public float ViewAngle = 160;
    public float ViewDistance = 10;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    [Title("Hearing")]
    public LayerMask soundMask;

    public float OpeningDoorTime = 2;
    public float attackCooldown = 3f;

    [Title("Sounds")]
    public AudioSource localSource;
    public AudioSource[] alertSounds;


    public AudioClip chaseMusic;
    public AudioClip patrolMusic;
    public AudioClip searchingMusic;

    public AudioClip[] scarySounds;
    public float scarySoundsDelay = 3f;
    public bool playScarySounds;

    [Title("References")]
    [Tooltip("Raycasts will start from here")]
    public Transform eyeTransform;
    public Animator anim;
    public NavMeshAgent agent;
    public LookAtIK lookAtIK;

    [Title("Debug")]
    [ReadOnly]
    public bool ChasingPlayer = false;

    private float attackTimer = 0f;
    private float scarySoundTimer = 0f;

    //Player tracking
    private Transform playerTrans;
    private Vector3 playerLastKnownLocation;

    public bool CanAttack()
    {
        return (attackTimer > attackCooldown);
    }

    private void Start()
    {
        playerTrans = PlayerSingleton.instance.GetComponent<Transform>();
    }

    private void Update()
    {
        UpdateTimers();

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
                GoToCurrentPatrolPoint();
                // Check if reach destination, if so, go to next destination
                bool agentReachedPatrolPoint = agent.remainingDistance < .5f;
                if (ReachedDestination())
                {
                    SetNextPatrolPoint();
                }
                break;
            case State.Chasing:
                // Go towards player, if break los go to last seen position.

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

                GoToLastKnownPlayerPos();
                bool reachedLastKnownPlayerPos = agent.remainingDistance < .1f;
                if (ReachedDestination())
                {
                    Debug.Log("Stopping");
                    StartCoroutine(Stop(2));
                    state = State.Patrol;
                }

                break;
            case State.Interacting:
                // Cast collider then get nearest interactable. interact with that.
                // Monster can only interact with certain interactables?
                break;
        }
    }
    private IEnumerator Stop(float seconds)
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
        if (state == State.Chasing)
        {
            lookAtIK.solver.IKPositionWeight = 1f;
        }
        else
        {
            //Dont look at player
            lookAtIK.solver.IKPositionWeight = 0f;
        }
    }

    private void UpdateTimers()
    {
        scarySoundTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;
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
                    if (!Physics.Raycast(eyeTransform.position, DirToPlayer(), DstToPlayer(), obstacleMask))
                    {
                        //Debug.Log("Can see player!");
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
    private void PatrolStart()
    {
        AudioManager.instance.FadeOutChaseAudio();
        ChasingPlayer = false;
        playScarySounds = false;
    }
    private void ChaseStart()
    {
        AudioManager.instance.FadeInChaseAudio();
        ChasingPlayer = true;
        playScarySounds = true;
    }
    private void SearchStart()
    {
        AudioManager.instance.FadeOutChaseAudio();
        ChasingPlayer = false;
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
        int i = Random.Range(0, alertSounds.Length);
        alertSounds[i].Play();
    }
}
