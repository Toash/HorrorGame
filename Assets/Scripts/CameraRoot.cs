using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

/// <summary>
/// Contains methdos to move the root camera for things such as crouching
/// </summary>
public class CameraRoot : MonoBehaviour
{
    public float moveBackTime = 1;

    [ReadOnly]
    public Vector3 initialCameraLocalRootPos;

    private void Awake()
    {
        initialCameraLocalRootPos = transform.localPosition;
    }



    void Update()
    {
        
    }

    public void MoveRootTowardsPos(Vector3 pos, float time)
    {
        transform.DOLocalMove(pos, time);
    }


}
