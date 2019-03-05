using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Switches player's movement mode between GOUND and AIRBORNE when the player interacts with the top trigger.

public class Crate_Top : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            GameManager.Instance.Players[0].CurrentMovementMode = PlayerMovementController.MovementMode.GROUND;
        }
        else if (collision.gameObject.tag == "Player2")
        {
            GameManager.Instance.Players[1].CurrentMovementMode = PlayerMovementController.MovementMode.GROUND;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            GameManager.Instance.Players[0].CurrentMovementMode = PlayerMovementController.MovementMode.AIRBORNE;
        }
        else if (collision.gameObject.tag == "Player2")
        {
            GameManager.Instance.Players[1].CurrentMovementMode = PlayerMovementController.MovementMode.AIRBORNE;
        }
    }
}
