using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnBubbleController : MonoBehaviour
{
    Transform target = null;

    public void Init(Transform target, float lifetime)
    {
        this.target = target;
        StartCoroutine(SelfDestroy(lifetime));
    }

    IEnumerator SelfDestroy(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        transform.position = target.position;
    }
}
