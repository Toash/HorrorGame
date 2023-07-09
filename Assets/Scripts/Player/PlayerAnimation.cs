using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        public Animator camAnim;

        private void Start()
        {
            PlayerSingleton.instance.health.PlayerTookDamage += camHit;
        }

        private void OnDisable()
        {
            PlayerSingleton.instance.health.PlayerTookDamage -= camHit;
        }

        private void camHit()
        {
            camAnim.SetTrigger("Hit");
        }
    }
}

