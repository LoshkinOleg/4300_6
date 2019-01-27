using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Private variables
    bool isLeftPlayerBullet = false;
    bool isPlayingDestructionAnimation = false;

    // References
    [SerializeField] GameObject destructionSprite = null;
    Rigidbody2D bulletRigidbody2D = null;
    CircleCollider2D bulletCollider = null;

    public void Init(bool isLeftPlayerBullet)
    {
        if (isLeftPlayerBullet)
        {
            this.isLeftPlayerBullet = true;
        }
        else
        {
            gameObject.transform.eulerAngles = new Vector3(0,180,0);
        }
    }

    IEnumerator DestroyBullet()
    {
        isPlayingDestructionAnimation = true;

        bulletRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        bulletCollider.enabled = false;

        if (!isLeftPlayerBullet)
        {
            destructionSprite.SetActive(true);
            destructionSprite.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            destructionSprite.SetActive(true);
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void Start()
    {
        bulletRigidbody2D = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        if (!isPlayingDestructionAnimation)
        {
            if (isLeftPlayerBullet)
            {
                bulletRigidbody2D.velocity = Vector2.right * GameManager.instance.leftPlayerBulletSpeed;
            }
            else
            {
                bulletRigidbody2D.velocity = -Vector2.right * GameManager.instance.rightPlayerBulletSpeed;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Shield")
        {
            collision.gameObject.GetComponent<ShieldController>().Hit();
            StartCoroutine(DestroyBullet());
        }

        if (isLeftPlayerBullet)
        {
            if (collision.gameObject.tag == "RightPlayer")
            {
                GameManager.instance.rightPlayer.GetComponent<PlayerController>().DamageOnce(0.1f);
                StartCoroutine(DestroyBullet());
            }
            else if(collision.gameObject.tag != "LeftPlayer")
            {
                StartCoroutine(DestroyBullet());
            }
        }
        else
        {
            if (collision.gameObject.tag == "LeftPlayer")
            {
                GameManager.instance.leftPlayer.GetComponent<PlayerController>().DamageOnce(0.1f);
                StartCoroutine(DestroyBullet());
            }
            else if (collision.gameObject.tag != "RightPlayer")
            {
                StartCoroutine(DestroyBullet());
            }
        }
    }
}
