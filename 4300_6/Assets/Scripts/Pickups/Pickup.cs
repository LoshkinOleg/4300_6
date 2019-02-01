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

    // Inspector variables
    [SerializeField] Type type = Type.LIFE;
    [SerializeField] float pickupSpeedLimit = 3;

    // References
    [SerializeField] GameObject shieldPrefab = null;
    [SerializeField] GameObject stormPrefab = null;
    Rigidbody2D pickupRigidbody2D = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            switch (type)
            {
                case Type.LIFE:
                    {
                        GameManager.instance.player1.life++;
                    }break;
                case Type.HEALTH:
                    {
                        GameManager.instance.player1.health += 0.5f;
                    }
                    break;
                case Type.SHIELD:
                    {
                        Instantiate(shieldPrefab).GetComponent<Shield>().target = GameManager.instance.player1;
                    }
                    break;
                case Type.SPEED_UP:
                    {
                        GameManager.instance.player1.SpeedBulletsUp();
                    }
                    break;
                case Type.STORM:
                    {
                        Instantiate(stormPrefab).GetComponent<Storm>().target = GameManager.instance.player2;
                    }
                    break;
                case Type.JETPACK:
                    {
                        GameManager.instance.player1.EnterJetpackMode();
                    }
                    break;
            }

        }
        else if (collision.gameObject.tag == "Player2")
        {
            switch (type)
            {
                case Type.LIFE:
                    {
                        GameManager.instance.player2.life++;
                    }
                    break;
                case Type.HEALTH:
                    {
                        GameManager.instance.player2.health += 0.5f;
                    }
                    break;
                case Type.SHIELD:
                    {
                        Instantiate(shieldPrefab).GetComponent<Shield>().target = GameManager.instance.player2;
                    }
                    break;
                case Type.SPEED_UP:
                    {
                        GameManager.instance.player2.SpeedBulletsUp();
                    }
                    break;
                case Type.STORM:
                    {
                        Instantiate(stormPrefab).GetComponent<Storm>().target = GameManager.instance.player1;
                    }
                    break;
                case Type.JETPACK:
                    {
                        GameManager.instance.player2.EnterJetpackMode();
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
