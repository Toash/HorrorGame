using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteManager : MonoBehaviour
{
    public KeyCode exitKey = KeyCode.Escape;
    public static NoteManager instance;
    public GameObject noteObject;
    public TMP_Text noteText;
    public AudioSource audioSource;
    public AudioClip nodeOpenSound;
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
            
            //audioSource.clip = nodeCloseSound;
            //audioSource.Play();
        }
        notesShowing = false;

    }
    public void ShowNotes(string note)
    {
        if (!notesShowing)
        {
            noteText.text = note;
            noteObject.SetActive(true);
            audioSource.clip = nodeOpenSound;
            audioSource.Play();
        }
        notesShowing = true;
    }


}