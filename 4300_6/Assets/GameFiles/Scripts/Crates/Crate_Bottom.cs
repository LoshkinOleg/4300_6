using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate_Bottom : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            GameManager.Instance.Players[0].CrateBottomHit(GetComponentInParent<CrateRootIdentifier>().gameObject.GetComponent<BoxCollider2D>());
        }
        else if (collision.gameObject.tag == "Player2")
        {
            GameManager.Instance.Players[1].CrateBottomHit(GetComponentInParent<CrateRootIdentifier>().gameObject.GetComponent<BoxCollider2D>());
        }
    }
}
