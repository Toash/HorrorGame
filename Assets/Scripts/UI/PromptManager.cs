using UnityEngine;
using System.Collections;
using TMPro;

public class PromptManager : MonoBehaviour
{
    public static PromptManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    public TMP_Text promptText;
    private void Start()
    {
        promptText.text = "";
    }

    public void ShowPrompt(string prompt, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(IEShowPrompt(prompt, duration));
    }

    public IEnumerator IEShowPrompt(string prompt, float duration)
    {
        promptText.text = prompt;
        yield return new WaitForSeconds(duration);
        promptText.text = "";
    }
}
