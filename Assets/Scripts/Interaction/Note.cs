using UnityEngine;

public class Note : Interactable
{
    public string noteText;

    public override void Interact()
    {
        NoteManager.instance.ShowNotes(noteText);
    }
}
