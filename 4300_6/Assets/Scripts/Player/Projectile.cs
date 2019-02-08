﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Attributes
    #region Attributes
    public float speed;

    // Inspector variables
    [SerializeField] float hitDamage = 0.01f;

    // References
    [SerializeField] GameObject projectileSpriteGO = null;
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
        if (bulletRigidbody2D != null)
        {
            isPlayingDestructionAnimation = true;

            bulletRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            bulletCollider.enabled = false;
            destructionSpriteGO.SetActive(true);
            projectileSpriteGO.SetActive(false);

            yield return new WaitForSeconds(0.5f);

            Destroy(gameObject);
        }
        else
        {
            yield return new WaitForSeconds(0);
        }
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
            bulletRigidbody2D.velocity = transform.right * speed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            GameManager.instance.player1.ModifyHealth(-hitDamage);
        }
        else if (collision.gameObject.tag == "Player2")
        {
            GameManager.instance.player2.ModifyHealth(-hitDamage);
        }

        if (!isPlayingDestructionAnimation)
        {
            StartCoroutine(DestroyBullet());
        }
    }
    #endregion

}
