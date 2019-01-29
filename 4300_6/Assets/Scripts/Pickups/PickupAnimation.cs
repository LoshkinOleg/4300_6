using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAnimation : MonoBehaviour
{
    float magnitude = 25;
    float frequency = 3;

    private void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(0,0,1) * Mathf.Sin(Time.time * frequency) * magnitude;
    }
}
