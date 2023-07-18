
using UnityEngine;

using Sirenix.OdinInspector;

public class CuttableFence : Interactable
{

    [ReadOnly]
    public bool cut;
    private string initialText;
    private void Start()
    {
        initialText = InteractText;
    }
    public override void Interact()
    {
        base.Interact();
        if (PlayerHasItem())
        {
            interactEvent.Invoke();
        }
    }

    public override void Update()
    {
        base.Update();

        if (PlayerHasItem())
        {
            this.InteractText = "Cut";
        }
        else
        {
            this.InteractText = initialText;
        }
    }


}
