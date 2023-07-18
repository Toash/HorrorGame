using UnityEngine;

public class Note : Interactable
{
    [Multiline(20)]
    public string noteText;

    public override void Interact()
    {
        NoteManager.instance.ShowNotes(noteText);
    }



}
