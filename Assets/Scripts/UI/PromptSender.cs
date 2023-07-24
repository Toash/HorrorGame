using UnityEngine;
using Sirenix.OdinInspector;


public class PromptSender : MonoBehaviour
{
    [TextArea]
    public string prompt;
    public float duration;

    [Button]
    public void SendPrompt()
    {
        PromptManager.instance.ShowPrompt(prompt, duration);
    }
}
