using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Classes and Enums
    public enum Type
    {
        LIFE,
        HEALTH,
        SHIELD,
        SPEED_UP,
        STORM,
        JETPACK
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
                case Type.STORM:
                    {
                        PickupManager.instance.Pickup_Storm(collision.gameObject.tag);
                    }
                    break;
                case Type.JETPACK:
                    {
                        PickupManager.instance.Pickup_Jetpack(collision.gameObject.tag);
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
    #endregion
}
