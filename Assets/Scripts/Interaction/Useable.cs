using UnityEngine;
using UnityEngine.Events;


//Use this for pickup as well as in hand
public class Useable : MonoBehaviour
{
    
    public UnityEvent UseEvent;
    public int uses = 1;

    public void Use()
    {
        UseEvent.Invoke();
        uses -= 1;
        if(uses <= 0)
        {
            Destroy(this.gameObject);
        }
           
    }

}
