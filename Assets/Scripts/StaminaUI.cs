using UnityEngine;
using UnityEngine.UI;
using Player;

public class StaminaUI : MonoBehaviour
{
    public Slider slider;

    private void Update()
    {
        slider.value = PlayerSingleton.instance.movement.currentStamina / PlayerSingleton.instance.movement.maxStamina;
    }
}
