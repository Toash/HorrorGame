using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using RootMotion.FinalIK;
using BehaviorDesigner.Runtime;
using Audio;
using Player;
using Sirenix.OdinInspector;

/// <summary>
/// this class is an absolute monolith
/// </summary>
public class Monster : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Chasing,
        Searching,
        Interacting
    }

    public State state = State.Chasing;

    [Title("Patrol")]
    public GameObject[] PatrolPoints;
    [ReadOnly]
    public GameObject NextPatrolPoint;

    [Title("Stats")]
    [Title("Field of View")]
    [Range(0,360)]
    public float ViewAngle = 160;
    public float ViewDistance = 10;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    public float OpeningDoorTime = 2;
    public float attackCooldown = 3f;
    public bool brainless = false;

    [Title("Sounds")]
    public AudioSource localSource;
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
    
    private Transform playerTrans;
    private float attackTimer = 0f;
    private float scarySoundTimer = 0f;
    private Vector3 dirToPlayerNoY;




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
        CheckIfLOSMaintained();

        StateMachine();
        Animation();
    }

    private void StateMachine()
    {
        switch (state)
        {
            case State.Patrol:
                break;
            case State.Chasing:
                // Go towards player, if break los go to last seen position.

                agent.SetDestination(playerTrans.position);
                break;
            case State.Searching:
                // Wander randomly around a radius.
                // Pick and random point around enemy, go there, keep doing x amount of times.
                break;
            case State.Interacting:
                // Cast collider then get nearest interactable. interact with that.
                // Monster can only interact with certain interactables?
                break;
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


    //Check for player so can begin chase.
    private void ScanForPlayer()
    {
        if (state != State.Chasing)
        {
            Vector3 dirToPlayer = (playerTrans.position - eyeTransform.position).normalized;
            this.dirToPlayerNoY = (new Vector3(playerTrans.position.x, eyeTransform.position.y, playerTrans.position.z) - eyeTransform.position).normalized;
            float dstToPlayer = Vector3.Distance(eyeTransform.position, playerTrans.position);

            //Player in range
            if (Physics.Raycast(eyeTransform.position, dirToPlayer, ViewDistance, playerMask))
            {
                //Check angle, should not care about y axis
                if (Vector3.Angle(eyeTransform.forward, dirToPlayerNoY) < ViewAngle / 2)
                {
                    //Check obstacles
                    if (!Physics.Raycast(eyeTransform.position, dirToPlayer, dstToPlayer, obstacleMask))
                    {
                        state = State.Chasing;
                    }
                }
            }
        }
    }
    //Check if line of sight is possible while chasing the player
    private void CheckIfLOSMaintained()
    {
        if(state == State.Chasing)
        {

        }
    }


    //------------------------------------------------------------------
    private void SetIKToPlayerCam()
    {
        lookAtIK.solver.IKPosition = PlayerSingleton.instance.cam.cam.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (attackTimer > attackCooldown)
            {
                //Damage
                Debug.Log("Monster attacking");
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
    public void OnDrawGizmos()
    {

        Vector3 viewAngleA = this.DirFromAngle(-this.ViewAngle / 2, false);
        Vector3 viewAngleB = this.DirFromAngle(this.ViewAngle / 2, false);

        Gizmos.color = Color.red;
        GizmosExtensions.DrawWireArc(this.eyeTransform.position, eyeTransform.forward, ViewAngle, ViewDistance);
        if(playerTrans!=null)
            Gizmos.DrawLine(eyeTransform.position, new Vector3(playerTrans.position.x, eyeTransform.position.y, playerTrans.position.z));
        

    }
}
