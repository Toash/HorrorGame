using UnityEngine;


/// <summary>
/// Can be used to freeze player camera / movement
/// </summary>
public class PlayerPauseManager : MonoBehaviour
{
    public delegate void Action();
    public Action Pause;
    public Action Unpause;

    public void Freeze()
    {
        Pause.Invoke();
    }
    public void Unfreeze()
    {
        Unpause.Invoke();
    }
}
