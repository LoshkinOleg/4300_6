using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Attributes
    #region Attributes
    // References
    [SerializeField] GameObject destructionSpriteGO = null;
    Rigidbody2D bulletRigidbody2D = null;
    CircleCollider2D bulletCollider = null;

    // Private variables
    bool isPlayingDestructionAnimation = false;
    #endregion

    // Private methods
    #region Private methods
    IEnumerator DestroyBullet()
    {
        isPlayingDestructionAnimation = true;

        GameManager.instance.PlaySound(GameManager.SoundType.BULLET_DESTRUCTION);

        bulletRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        bulletCollider.enabled = false;
        destructionSpriteGO.SetActive(true);

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
            bulletRigidbody2D.velocity = Vector2.right * GameManager.instance.leftPlayerBulletSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(DestroyBullet());
    }
    #endregion

}
