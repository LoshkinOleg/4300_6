using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatridgeDestroyer : MonoBehaviour
{
    [SerializeField] float lifetime = 1f;

    IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(SelfDestroy());
    }
}
