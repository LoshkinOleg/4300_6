using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    Rigidbody2D crateRigidBody2D = null;

    private void Start()
    {
        crateRigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            GameManager.instance.player1.SetMovementMode(Player1.MovementMode.GROUND);
        }
        else if (collision.gameObject.tag == "Player2")
        {
            GameManager.instance.player2.SetMovementMode(Player2.MovementMode.GROUND);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            GameManager.instance.player1.SetMovementMode(Player1.MovementMode.AIRBORNE);
        }
        else if (collision.gameObject.tag == "Player2")
        {
            GameManager.instance.player2.SetMovementMode(Player2.MovementMode.AIRBORNE);
        }
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
        if (crateRigidBody2D.velocity.y < -CrateManager.instance.speedLimit)
        {
            crateRigidBody2D.velocity = new Vector2(0, -CrateManager.instance.speedLimit);
        }
    }
}
