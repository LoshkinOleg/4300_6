using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffectController : MonoBehaviour
{
    [HideInInspector] public GameObject target = null;
    [HideInInspector] public float lifetime = 0.5f;
    [SerializeField] float verticalOffset = 0.6f;

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
