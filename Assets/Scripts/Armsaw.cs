using UnityEngine;
using Sirenix.OdinInspector;

public class Armsaw : MonoBehaviour
{
    [Tooltip("part to rotate")]
    public Transform armsawRoot;
    public Vector3 armsawSpinAxis;
    public float rotateSpeed = 50;



    private void Update()
    {
        armsawRoot.Rotate(armsawSpinAxis.normalized, rotateSpeed * Time.deltaTime, Space.Self);
    }


}
