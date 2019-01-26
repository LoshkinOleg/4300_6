using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Classes and Enums
    public enum Type
    {
        LIFE,
        SPEED_UP,
        STORM
    }

    // Inspector variables
    [SerializeField] Type type = Type.LIFE;
    [SerializeField] float pickupSpeedLimit = 3;

    // References
    Rigidbody2D pickupRigidbody2D = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "LeftPlayer")
        {
            switch (type)
            {
                case Type.LIFE:
                    {
                        GameManager.instance.leftPlayer.GetComponent<PlayerController>().LifePickup();
                    }break;
                case Type.SPEED_UP:
                    {
                        GameManager.instance.leftPlayer.GetComponent<PlayerController>().SpeedPickup();
                    }
                    break;
                case Type.STORM:
                    {
                        GameManager.instance.leftPlayer.GetComponent<PlayerController>().TriggerStorm();
                    }
                    break;
            }

        }
        else if(collision.gameObject.tag == "RightPlayer")
        {
            switch (type)
            {
                case Type.LIFE:
                    {
                        GameManager.instance.rightPlayer.GetComponent<PlayerController>().LifePickup();
                    }
                    break;
                case Type.SPEED_UP:
                    {
                        GameManager.instance.rightPlayer.GetComponent<PlayerController>().SpeedPickup();
                    }
                    break;
                case Type.STORM:
                    {
                        GameManager.instance.rightPlayer.GetComponent<PlayerController>().TriggerStorm();
                    }
                    break;
            }
        }
        Destroy(gameObject);
    }

    private void Start()
    {
        pickupRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(pickupRigidbody2D.velocity.y) > pickupSpeedLimit)
        {
            if (pickupRigidbody2D.velocity.y > 0)
            {
                pickupRigidbody2D.velocity = new Vector2(pickupRigidbody2D.velocity.x, pickupSpeedLimit);
            }
            else
            {
                pickupRigidbody2D.velocity = new Vector2(pickupRigidbody2D.velocity.x, -pickupSpeedLimit);
            }
        }
    }
}
