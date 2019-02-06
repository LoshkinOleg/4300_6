using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTester : MonoBehaviour
{
    private void Update()
    {
        if (Input.anyKey)
        {
            Debug.Log("Input detected.");
        }
    }
}
