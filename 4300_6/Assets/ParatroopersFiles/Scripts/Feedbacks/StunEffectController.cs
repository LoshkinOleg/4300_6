using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffectController : MonoBehaviour
{
    // Private variables
    Transform target = null;
    float lifetime;
    float verticalOffset;

    // Public methods
    public void Init(Transform target, float lifetime, float verticalOffset)
    {
        this.target = target;
        this.lifetime = lifetime;
        this.verticalOffset = verticalOffset;
    }

    // Private methods
    IEnumerator SelfDestroyAfterSeconds()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    // Inherited methods
    private void Start()
    {
        StartCoroutine(SelfDestroyAfterSeconds());
    }
    private void FixedUpdate()
    {
        transform.position = target.transform.position + new Vector3(0,verticalOffset,0);
    }
}
