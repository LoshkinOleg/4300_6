using System.Collections;
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
        SHOTGUN,
        SNIPER,
        BAZOOKA,
        MINIGUN,
    }

    [SerializeField] Type type = Type.HEALTH;

    Rigidbody2D pickupRigidbody2D = null;

    // Inherited methods
    #region Inherited methods
    private void Start()
    {
        pickupRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle picking up.
        if (collision.gameObject.tag == "Player1" || collision.gameObject.tag == "Player2")
        {
            switch (type)
            {
                case Type.HEALTH:
                    {
                        PickupManager.Instance.Pickup_Health(collision.gameObject.tag);
                    }
                    break;
                case Type.SHIELD:
                    {
                        PickupManager.Instance.Pickup_Shield(collision.gameObject.tag);
                    }
                    break;
                case Type.SPEED_UP:
                    {
                        PickupManager.Instance.Pickup_Speedup(collision.gameObject.tag);
                    }
                    break;
                case Type.SHOTGUN:
                    {
                        PickupManager.Instance.Pickup_Shotgun(collision.gameObject.tag);
                    }
                    break;
                case Type.SNIPER:
                    {
                        PickupManager.Instance.Pickup_Sniper(collision.gameObject.tag);
                    }
                    break;
                case Type.BAZOOKA:
                    {
                        PickupManager.Instance.Pickup_Bazooka(collision.gameObject.tag);
                    }
                    break;
                case Type.MINIGUN:
                    {
                        PickupManager.Instance.Pickup_Minigun(collision.gameObject.tag);
                    }
                    break;
            }
        }
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        // Limit pickup speed.
        if (pickupRigidbody2D != null)
        {
            if (Mathf.Abs(pickupRigidbody2D.velocity.y) > PickupManager.Instance.PickupSpeedLimit)
            {
                if (pickupRigidbody2D.velocity.y > 0)
                {
                    pickupRigidbody2D.velocity = new Vector2(pickupRigidbody2D.velocity.x, PickupManager.Instance.PickupSpeedLimit);
                }
                else
                {
                    pickupRigidbody2D.velocity = new Vector2(pickupRigidbody2D.velocity.x, -PickupManager.Instance.PickupSpeedLimit);
                }
            }
        }
    }
    #endregion
}
