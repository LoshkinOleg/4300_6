using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate_Bottom : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            GameManager.instance.player1.Stun();
        }
        else if (collision.gameObject.tag == "Player2")
        {
            GameManager.instance.player2.Stun();
        }
    }
}
