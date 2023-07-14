using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTargetSway : MonoBehaviour
{
    public float SwayAmount = 10;
    public float SwaySpeed = 10;
    private Vector3 initialPos;
    private Vector3 offsetPos;
    private void Awake()
    {
        initialPos = transform.localPosition;
    }

    void Update()
    {
        offsetPos.x = Input.GetAxis("Mouse X") * SwayAmount;
        offsetPos.y = Input.GetAxis("Mouse Y") * SwayAmount;
        
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPos + offsetPos, SwaySpeed * Time.deltaTime);
    }
}
