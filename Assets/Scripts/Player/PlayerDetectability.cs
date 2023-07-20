using UnityEngine;
using Sirenix.OdinInspector;
using Player;

public class PlayerDetectability : MonoBehaviour
{
    [Title("Sound")]
    public float CrouchSoundRadus = .5f;
    public float WalkSoundRadius = 3;
    public float RunSoundRadius = 7;
    public SphereCollider soundCollider;


    [ReadOnly]
    public bool hiding;

    public delegate void BoolDelegate(bool b);
    public BoolDelegate PlayerHiding;

    private void Update()
    {
        if (PlayerSingleton.instance.movement.moving)
        {
            soundCollider.enabled = true;
            soundCollider.radius = WalkSoundRadius;
            if (PlayerSingleton.instance.movement.sprinting)
            {
                soundCollider.radius = RunSoundRadius;
            }
            if (PlayerSingleton.instance.movement.crouching)
            {
                soundCollider.radius = CrouchSoundRadus;
            }
        }
        else
        {
            soundCollider.enabled = false;
            soundCollider.radius = WalkSoundRadius;
        }
    }

    public void OnTriggerEnter(Collider other)
    {

        if(other.tag == "HidingSpot")
        {
            ReliableOnTriggerExit.NotifyTriggerEnter(other, gameObject, OnTriggerExit);


            HidingSpot spot = other.GetComponent<HidingSpot>();

            spot.PlayerInHidingSpot = true;

            hiding = true;
            PlayerHiding.Invoke(true);
        }
    }
    public void OnTriggerExit(Collider other)
    {

        if (other.tag == "HidingSpot")
        {
            ReliableOnTriggerExit.NotifyTriggerExit(other, gameObject);

            HidingSpot spot = other.GetComponent<HidingSpot>();

            spot.PlayerInHidingSpot = false;

            hiding = false;
            PlayerHiding.Invoke(false);
        }
    }
}
