using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteManager : MonoBehaviour
{
    public KeyCode exitKey = KeyCode.Escape;
    public static NoteManager instance;
    public GameObject noteObject;
    public TMP_Text noteText;
    public AudioSource openSound;
    //public AudioClip nodeCloseSound;

    private bool notesShowing = false;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        HideNotes();
    }

    private void Update()
    {
        if (Input.GetKeyDown(exitKey))
        {
            HideNotes();
        }
    }

    public void HideNotes()
    {

        noteObject.SetActive(false);
        
        if (notesShowing)
        {
            Player.PlayerSingleton.instance.movement.Resume();
            //audioSource.clip = nodeCloseSound;
            //audioSource.Play();
        }
        notesShowing = false;

    }
    public void ShowNotes(string note)
    {
        if (!notesShowing)
        {
            Player.PlayerSingleton.instance.movement.Stop();
            noteText.text = note;
            noteObject.SetActive(true);
            openSound.Play();
        }
        notesShowing = true;
    }


}
