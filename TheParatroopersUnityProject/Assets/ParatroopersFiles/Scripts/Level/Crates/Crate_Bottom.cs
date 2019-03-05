using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stuns the player and ejects them down if they enter the bottom trigger. If the player is stuck between the crate and the bottom bound, they kill themselves.

public class Crate_Bottom : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            GameManager.Instance.Players[0].CrateBottomHit(GetComponentInParent<CrateRootIdentifier>().gameObject.GetComponent<BoxCollider2D>()); // Argument passed is crate's main collider located on the parent GO.
        }
        else if (collision.gameObject.tag == "Player2")
        {
            GameManager.Instance.Players[1].CrateBottomHit(GetComponentInParent<CrateRootIdentifier>().gameObject.GetComponent<BoxCollider2D>());
        }
    }
}
