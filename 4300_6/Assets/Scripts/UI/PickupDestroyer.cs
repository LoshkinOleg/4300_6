using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupDestroyer : MonoBehaviour
{
    private void FixedUpdate()
    {
        Destroy(GameObject.FindGameObjectWithTag("Health_Pickup"));
        Destroy(GameObject.FindGameObjectWithTag("Shield_Pickup"));
        Destroy(GameObject.FindGameObjectWithTag("Speedup_Pickup"));
        Destroy(GameObject.FindGameObjectWithTag("Slowdown_Pickup"));
        Destroy(GameObject.FindGameObjectWithTag("Storm_Pickup"));
        GameObject[] lightningBolts = GameObject.FindGameObjectsWithTag("LightningBolt");
        foreach (var item in lightningBolts)
        {
            Destroy(item);
        }
    }
}
