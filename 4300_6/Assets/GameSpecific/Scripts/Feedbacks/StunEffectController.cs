using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffectController : MonoBehaviour
{
    [HideInInspector] public Transform target = null;
    [HideInInspector] public float lifetime = 0.5f;
    [HideInInspector] public float verticalOffset = 0.6f;

    public void Init(Transform target, float lifetime, float verticalOffset)
    {
        this.target = target;
        this.lifetime = lifetime;
        this.verticalOffset = verticalOffset;
    }

    IEnumerator SelfDestroyAfterSeconds()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(SelfDestroyAfterSeconds());
    }

    private void FixedUpdate()
    {
        transform.position = target.transform.position + new Vector3(0,verticalOffset,0);
    }
}
