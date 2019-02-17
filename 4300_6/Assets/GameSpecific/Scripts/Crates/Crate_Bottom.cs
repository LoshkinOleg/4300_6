using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate_Bottom : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            GameManager.Instance.Player1.CrateBottomHit(GetComponent<BoxCollider2D>());
        }
        else if (collision.gameObject.tag == "Player2")
        {
            GameManager.Instance.Player2.CrateBottomHit(GetComponent<BoxCollider2D>());
        }
    }
}
