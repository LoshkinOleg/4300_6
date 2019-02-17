using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate_Top : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            GameManager.instance.player1.currentMovementMode = PlayerMovementController.MovementMode.GROUND;
        }
        else if (collision.gameObject.tag == "Player2")
        {
            GameManager.instance.player2.currentMovementMode = PlayerMovementController.MovementMode.GROUND;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            GameManager.instance.player1.currentMovementMode = PlayerMovementController.MovementMode.AIRBORNE;
        }
        else if (collision.gameObject.tag == "Player2")
        {
            GameManager.instance.player2.currentMovementMode = PlayerMovementController.MovementMode.AIRBORNE;
        }
    }
}
