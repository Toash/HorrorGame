using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using Player;


public class Pickup : Interactable
{
    [Title("Pickup")]
    public PickupSO SO;
    public AudioSource thud;
    public Rigidbody rb;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit");
        thud?.Play();
    }
    public override void Interact()
    {
        base.Interact();
        PickUp();
        Destroy(gameObject);
    }
    public virtual void PickUp()
    {
        // Give pickup to player
        PlayerSingleton.instance.interact.EquipPickup(SO);
    }
}
