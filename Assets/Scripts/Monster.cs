using UnityEngine;
using RootMotion.FinalIK;
using BehaviorDesigner.Runtime;
using Audio;
using Player;

public class Monster : MonoBehaviour
{
    public LookAtIK lookAtIK;
    public Animator anim;
    public bool brainless = false;
    public bool ChasingPlayer = false;
    public BehaviorTree behaviorTree;
    public AudioSource localSource;
    public AudioClip chaseMusic;
    public AudioClip patrolMusic;
    public AudioClip searchingMusic;

    public AudioClip[] scarySounds;
    public float scarySoundsDelay = 3f;
    public bool playScarySounds;
    private float scarySoundTimer = 0f;

    public float attackCooldown = 3f;
    private float attackTimer = 0f;

    public bool CanAttack()
    {
        return (attackTimer > attackCooldown);
    }

    private void OnEnable()
    {
        behaviorTree.RegisterEvent("Chase", ChaseStart);
        behaviorTree.RegisterEvent("Search", SearchStart);
        behaviorTree.RegisterEvent("Patrol", PatrolStart);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        //Look at player camera
        lookAtIK.solver.IKPosition = PlayerSingleton.instance.cam.cam.transform.position;

        scarySoundTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        if (ChasingPlayer)
        {
            lookAtIK.solver.IKPositionWeight = 1f;
        }
        else
        {
            //Dont look at player
            lookAtIK.solver.IKPositionWeight = 0f;
        }

        if (scarySoundsDelay < scarySoundTimer && playScarySounds)
        {
            PlayRandomScarySound();
            scarySoundTimer = 0;
        }
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
}
