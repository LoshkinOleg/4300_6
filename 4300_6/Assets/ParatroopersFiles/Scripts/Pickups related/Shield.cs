using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    // Inspector variables
    [SerializeField] int hitCounter = 7;

    // Private variables
    Transform target = null;
    PlayerManager targetManager = null;
    bool leftPlayerIsTarget;
    bool isActive;

    // Public methods
    public void Init(PlayerManager target)
    {
        this.target = target.GetComponent<Transform>();
        targetManager = target;

        if (target.IsLeftPlayer)
        {
            leftPlayerIsTarget = true;
            gameObject.tag = "Player1Shield";
        }
        else
        {
            gameObject.tag = "Player2Shield";
        }

        targetManager.IncrementShieldCount(this);
    }
    public void Hit(int damage)
    {
        hitCounter -= damage;
    }

    // Inherited methods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive)
        {
            if (leftPlayerIsTarget)
            {
                if (collision.gameObject.tag == "Player2Projectile")
                {
                    hitCounter--;
                }
            }
            else
            {
                if (collision.gameObject.tag == "Player1Projectile")
                {
                    hitCounter--;
                }
            }
        }

    }
    private void FixedUpdate()
    {
        // Follow player.
        transform.position = target.position;
    }
    private void Update()
    {
        // It is this shield's turn to take damage.
        if (targetManager.ShieldCounter < 2 || targetManager.CurrentShield == this)
        {
            if (!isActive)
            {
                isActive = true;
            }
        }
        else // It is not this shield's turn to take damage.
        {
            if (isActive)
            {
                isActive = false;
            }
        }

        if (hitCounter < 1)
        {
            targetManager.DecrementShieldCount(this);
            Destroy(gameObject);
        }
    }
}
