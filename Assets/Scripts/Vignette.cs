using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Player;
public class Vignette : MonoBehaviour
{
    public Image image;

    private void Start()
    {
        PlayerSingleton.instance.detectability.PlayerHiding += Fade;
    }
    public void Fade(bool fadeIn)
    {
        if (fadeIn)
        {
            image.DOColor(Color.black, 1);
        }
        else
        {
            image.DOColor(Color.clear, 1);
        }

    }

}
