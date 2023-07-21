using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerHealth : MonoBehaviour
{
    public ScreenDamage screenDamage;
    public int StartingHealth = 100;
    [ShowInInspector,ReadOnly]
    private int currentHealth;

    public delegate void Action();
    public Action PlayerTookDamage;
    public Action PlayerDied;

    public AudioSource playerHurtSound;

    private void Start()
    {
        currentHealth = StartingHealth;
    }

    [Button]
    public void Damage(int amount)
    {
        playerHurtSound.Play();

        PlayerTookDamage.Invoke();
        currentHealth -= amount;
        screenDamage.CurrentHealth = currentHealth;

        if(currentHealth <= 0)
        {
            PlayerDied.Invoke();
        }
    }
}
