using UnityEngine;
using UnityEngine.UI;
using Player;

public class StaminaUI : MonoBehaviour
{
    public Slider slider;
    public Image Fill;

    public float fadeoutSpeed = 5;

    private float opacityDest;

    private void Update()
    {
        slider.value = PlayerSingleton.instance.movement.currentStamina / PlayerSingleton.instance.movement.maxStamina;
        if (PlayerSingleton.instance.movement.sprinting)
        {
            opacityDest = 1;

        }
        else
        {
            //Player not moving
            opacityDest = 0;
        }
        Fill.color = new Color(1, 1, 1, Mathf.Lerp(Fill.color.a, opacityDest, Time.deltaTime * fadeoutSpeed));
    }
}
