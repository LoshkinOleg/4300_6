using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate_Sides : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            if (GameManager.Instance.Players[0].StunOpportunityTimer > 0)
            {
                GameManager.Instance.Players[0].Stun();
            }
        }
        else if (collision.gameObject.tag == "Player2")
        {
            if (GameManager.Instance.Players[1].StunOpportunityTimer > 0)
            {
                GameManager.Instance.Players[1].Stun();
            }
        }
    }
}
