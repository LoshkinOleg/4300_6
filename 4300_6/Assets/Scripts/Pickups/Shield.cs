using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] int hitCounter = 3;

    public GameObject target = null; // Needs to be set up upon shield instantiation.

    public void Hit()
    {
        hitCounter--;
    }

    // Inherited methods
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            Hit();
        }
    }

    private void FixedUpdate()
    {
        transform.position = target.transform.position;
    }

    private void Update()
    {
        if (hitCounter < 1)
        {
            Destroy(gameObject);
        }
    }
}
