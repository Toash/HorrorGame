using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Functionality for simple door
/// </summary>
public class Door : Interactable
{
    [Space]
    public Animator anim;
    [Tooltip("To turn off when swinging")]
    public Collider col;
    public Collider enemyBlockCol;

    public AudioSource source;
    public AudioClip openSound;
    public AudioClip closedSound;

    public UnityEvent openEvent;
    public UnityEvent closeEvent;
    

    private bool closed = true;





    public override void Update()
    {
        base.Update();
        //Door is swinging
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            col.enabled = false;
        }
        else
        {
            col.enabled = true;
        }
        if (closed)
        {
            this.InteractText = "Open";
        }
        else
        {
            this.InteractText = "Close";
        }
    }
    public override void Interact()
    {
        if (closed)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
    public bool isOpen()
    {
        return !closed;
    }
    public void Open()
    {
        openEvent.Invoke();
        anim.SetBool("Open", true);
        PlayClip(openSound);
        closed = false;
        enemyBlockCol.enabled = false;
        
    }

    public void Close()
    {
        closeEvent.Invoke();
        anim.SetBool("Open", false);
        PlayClip(closedSound);
        closed = true;
        enemyBlockCol.enabled = true;
    }

    public void PlayClip(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }
}
