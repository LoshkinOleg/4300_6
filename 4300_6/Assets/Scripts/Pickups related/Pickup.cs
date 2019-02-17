﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Classes and Enums
    public enum Type
    {
        HEALTH,
        SHIELD,
        SPEED_UP,
        JETPACK,
        SHOTGUN,
        SNIPER,
        BAZOOKA,
        MINIGUN,
        LIFE
    }

    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] Type type = Type.LIFE;

    // References
    Rigidbody2D pickupRigidbody2D = null;
    #endregion

    // Inherited methods
    #region Inherited methods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1" || collision.gameObject.tag == "Player2")
        {
            switch (type)
            {
                case Type.LIFE:
                    {
                        PickupManager.instance.Pickup_Life(collision.gameObject.tag);
                    }
                    break;
                case Type.HEALTH:
                    {
                        PickupManager.instance.Pickup_Health(collision.gameObject.tag);
                    }
                    break;
                case Type.SHIELD:
                    {
                        PickupManager.instance.Pickup_Shield(collision.gameObject.tag);
                    }
                    break;
                case Type.SPEED_UP:
                    {
                        PickupManager.instance.Pickup_Speedup(collision.gameObject.tag);
                    }
                    break;
                case Type.JETPACK:
                    {
                        PickupManager.instance.Pickup_Jetpack(collision.gameObject.tag);
                    }
                    break;
                case Type.SHOTGUN:
                    {
                        PickupManager.instance.Pickup_Shotgun(collision.gameObject.tag);
                    }
                    break;
                case Type.SNIPER:
                    {
                        PickupManager.instance.Pickup_Sniper(collision.gameObject.tag);
                    }
                    break;
                case Type.BAZOOKA:
                    {
                        PickupManager.instance.Pickup_Bazooka(collision.gameObject.tag);
                    }
                    break;
                case Type.MINIGUN:
                    {
                        PickupManager.instance.Pickup_Minigun(collision.gameObject.tag);
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
        if (pickupRigidbody2D != null)
        {
            if (Mathf.Abs(pickupRigidbody2D.velocity.y) > PickupManager.instance.pickupSpeedLimit)
            {
                if (pickupRigidbody2D.velocity.y > 0)
                {
                    pickupRigidbody2D.velocity = new Vector2(pickupRigidbody2D.velocity.x, PickupManager.instance.pickupSpeedLimit);
                }
                else
                {
                    pickupRigidbody2D.velocity = new Vector2(pickupRigidbody2D.velocity.x, -PickupManager.instance.pickupSpeedLimit);
                }
            }
        }
    }
    #endregion
}
