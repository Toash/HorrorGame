using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDamageTrigger : MonoBehaviour
{
    public Killer monster;
    public string parameterName = "RightHandEnabler";
    public float HitCooldown = .4f;

    public Animator anim;

    private float hitCooldownTimer = 0;

    private bool hitCooldownOver()
    {
        return hitCooldownTimer > HitCooldown;
    }
    private void Update()
    {
        hitCooldownTimer += Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && anim.GetFloat(parameterName) > 0.9f && hitCooldownOver())
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            health.Damage(40);
            Debug.Log("Apply damage!");
            hitCooldownTimer = 0;
        }
    }
}
