using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightBullet : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Private variables
    bool isPlayingDestructionAnimation = false;

    // References
    [SerializeField] GameObject destructionSprite = null;
    Rigidbody2D bulletRigidbody2D = null;
    CircleCollider2D bulletCollider = null;
    #endregion

    // Private methods
    #region Private methods
    IEnumerator DestroyBullet()
    {
        isPlayingDestructionAnimation = true;

        bulletRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        bulletCollider.enabled = false;
        destructionSprite.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
    #endregion

    // Inherited methods
    #region Inherited Methods
    private void Start()
    {
        bulletRigidbody2D = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        if (!isPlayingDestructionAnimation)
        {
            bulletRigidbody2D.velocity = -Vector2.right * GameManager.instance.rightPlayerBulletSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle Shield collision
        if (collision.gameObject.tag == "LeftShield")
        {
            collision.gameObject.GetComponent<LeftShield>().Hit();
            StartCoroutine(DestroyBullet());
        }

        // Handle enemy collision
        if (collision.gameObject.tag == "LeftPlayer")
        {
            GameManager.instance.leftPlayer.GetComponent<LeftPlayer>().DamageOnce(0.1f);
            StartCoroutine(DestroyBullet());
        }
        else // Handle the rest of possible collisions
        {
            StartCoroutine(DestroyBullet());
        }
    }
    #endregion
}
