using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Private variables
    bool isLeftPlayerBullet = false;

    // References
    Rigidbody2D bulletRigidbody2D = null;

    public void Init(bool isLeftPlayerBullet)
    {
        if (isLeftPlayerBullet)
        {
            this.isLeftPlayerBullet = true;
        }
    }

    private void Start()
    {
        bulletRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (isLeftPlayerBullet)
        {
            bulletRigidbody2D.velocity = Vector2.right * GameManager.instance.bulletSpeed;
        }
        else
        {
            bulletRigidbody2D.velocity = -Vector2.right * GameManager.instance.bulletSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isLeftPlayerBullet)
        {
            if (collision.gameObject.tag == "RightPlayer")
            {
                GameManager.instance.rightPlayer.GetComponent<PlayerController>().DamageOnce(0.1f);
                Destroy(gameObject);
            }
            else if(collision.gameObject.tag != "LeftPlayer")
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (collision.gameObject.tag == "LeftPlayer")
            {
                GameManager.instance.leftPlayer.GetComponent<PlayerController>().DamageOnce(0.1f);
                Destroy(gameObject);
            }
            else if (collision.gameObject.tag != "RightPlayer")
            {
                Destroy(gameObject);
            }
        }
    }
}
