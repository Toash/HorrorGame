using UnityEngine;
using Player;
using TMPro;
public class InteractTextUI : MonoBehaviour
{
    public TMP_Text text;

    private void Start()
    {
        text.text = "";
    }

    private void Update()
    {
        Interactable currentInteractable = PlayerSingleton.instance.interact.currentInteractable;
        string interactText;
        if (currentInteractable)
        {
            interactText = currentInteractable.InteractText;
            text.text = interactText;
        }
        else
        {
            text.text = "";
        }

    }
}
