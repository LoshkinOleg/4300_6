using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate_Top : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            GameManager.Instance.Player1.CurrentMovementMode = PlayerMovementController.MovementMode.GROUND;
        }
        else if (collision.gameObject.tag == "Player2")
        {
            GameManager.Instance.Player2.CurrentMovementMode = PlayerMovementController.MovementMode.GROUND;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            GameManager.Instance.Player1.CurrentMovementMode = PlayerMovementController.MovementMode.AIRBORNE;
        }
        else if (collision.gameObject.tag == "Player2")
        {
            GameManager.Instance.Player2.CurrentMovementMode = PlayerMovementController.MovementMode.AIRBORNE;
        }
    }
}
