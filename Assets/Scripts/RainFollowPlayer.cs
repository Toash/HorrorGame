using UnityEngine;
using Player;

public class RainFollowPlayer : MonoBehaviour
{
    private float initialY;
    // Start is called before the first frame update
    void Start()
    {
        initialY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = (new Vector3(PlayerSingleton.instance.transform.position.x, initialY, PlayerSingleton.instance.transform.position.z));
    }
}
