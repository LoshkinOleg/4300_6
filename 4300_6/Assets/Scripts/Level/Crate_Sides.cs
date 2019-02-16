using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate_Sides : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            if (GameManager.instance.player1.hitByProjectileTimer > 0)
            {
                GameManager.instance.player1.stunController.CrateSideStun();
            }
        }
        else if (collision.gameObject.tag == "Player2")
        {
            if (GameManager.instance.player2.hitByProjectileTimer > 0)
            {
                GameManager.instance.player2.stunController.CrateSideStun();
            }
        }
    }
}
