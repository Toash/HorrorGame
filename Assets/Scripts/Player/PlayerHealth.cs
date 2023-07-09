using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerHealth : MonoBehaviour
{
    public delegate void Action();
    public Action PlayerTookDamage;

    [Button]
    public void Damage()
    {
        Debug.Log("Damage");
        PlayerTookDamage.Invoke();
    }
}
