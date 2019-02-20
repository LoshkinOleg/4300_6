using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoLeftTextController : MonoBehaviour
{
    [SerializeField] float lifetime = 1f;

    IEnumerator SelfDestroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(SelfDestroy(lifetime));
    }
}
