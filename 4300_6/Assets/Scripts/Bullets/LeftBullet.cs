using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftBullet : MonoBehaviour
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

        GameManager.instance.PlaySound(GameManager.SoundType.BULLET_DESTRUCTION);

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

        GameManager.instance.PlaySound(GameManager.SoundType.SHOTGUN_FIRE);
    }

    private void FixedUpdate()
    {
        if (!isPlayingDestructionAnimation)
        {
            bulletRigidbody2D.velocity = Vector2.right * GameManager.instance.leftPlayerBulletSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle Shield collision
        if (collision.gameObject.tag == "RightShield")
        {
            collision.gameObject.GetComponent<RightShield>().Hit();

            GameManager.instance.PlaySound(GameManager.SoundType.SHIELD_HIT);

            StartCoroutine(DestroyBullet());
        }

        // Handle enemy collision
        if (collision.gameObject.tag == "RightPlayer")
        {
            GameManager.instance.rightPlayer.GetComponent<RightPlayer>().DamageOnce(0.1f);
            StartCoroutine(DestroyBullet());
        }
        else // Handle the rest of possible collisions
        {
            StartCoroutine(DestroyBullet());
        }
    }
    #endregion

}
