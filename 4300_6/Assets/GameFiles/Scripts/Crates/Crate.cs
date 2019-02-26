using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Limits crate speed and destroy crate when it hits the DestructorGO
public class Crate : MonoBehaviour
{
    Rigidbody2D crateRigidBody2D = null;

    private void Start()
    {
        crateRigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Destructor")
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (crateRigidBody2D.velocity.y < -CrateManager.Instance.speedLimit)
        {
            crateRigidBody2D.velocity = new Vector2(0, -CrateManager.Instance.speedLimit);
        }
    }
}
