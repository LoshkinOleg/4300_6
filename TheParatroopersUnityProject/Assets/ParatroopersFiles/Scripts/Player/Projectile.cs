﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Setup variables
    [HideInInspector] public float speed;
    [HideInInspector] public Weapon type;
    [HideInInspector] public float visualFeedbackLifetime;

    // Inspector variables
    [SerializeField] float explosionRadius = 1.5f;

    // References
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Sprite[] projectile_Sprites = new Sprite[3]; // 0: pistol and minigun, 1: shotgun, 2: bazooka
    [SerializeField] Sprite[] projectileDestruction_Sprites = new Sprite[3]; // 0: pistol, shotgun and minigun, 1: sniper, 2: bazooka
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

        bulletRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        bulletCollider.enabled = false;
        DisplayBulletDestruction();

        yield return new WaitForSeconds(visualFeedbackLifetime);

        Destroy(gameObject);
    }
    void DisplayBulletDestruction()
    {
        switch (type)
        {
            case Weapon.PISTOL:
                spriteRenderer.sprite = projectileDestruction_Sprites[0];
                break;
            case Weapon.SHOTGUN:
                spriteRenderer.sprite = projectileDestruction_Sprites[0];
                break;
            case Weapon.SNIPER:
                spriteRenderer.sprite = projectileDestruction_Sprites[1];
                break;
            case Weapon.BAZOOKA:
                spriteRenderer.sprite = projectileDestruction_Sprites[2];
                break;
            case Weapon.MINIGUN:
                spriteRenderer.sprite = projectileDestruction_Sprites[0];
                break;
        }
    }
    void DisplayAppropriateProjectile()
    {
        switch (type)
        {
            case Weapon.PISTOL:
                spriteRenderer.sprite = projectile_Sprites[0];
                break;
            case Weapon.SHOTGUN:
                spriteRenderer.sprite = projectile_Sprites[1];
                break;
            case Weapon.BAZOOKA:
                spriteRenderer.sprite = projectile_Sprites[2];
                break;
            case Weapon.MINIGUN:
                spriteRenderer.sprite = projectile_Sprites[0];
                break;
        }
    }
    public void RocketFeedback()
    {
        GameManager.Instance.ScreenShake.ShakeScreen(type);
        SoundManager.Instance.PlaySound("bazooka_hit");
    }
    IEnumerator SwitchToMissileLayer()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.layer = 17;
    }
    #endregion

    // Inherited methods
    #region Inherited Methods
    private void Start()
    {
        bulletRigidbody2D = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<CircleCollider2D>();
        DisplayAppropriateProjectile();

        if (type == Weapon.SNIPER)
        {
            if (gameObject.tag == "Player1Projectile")
            {
                transform.position = GameManager.Instance.Players[0].transform.position;
            }
            else
            {
                transform.position = GameManager.Instance.Players[1].transform.position;
            }
            StartCoroutine(DestroyBullet());
        }

        if (type == Weapon.BAZOOKA) // Make missile collide with anything.
        {
            StartCoroutine(SwitchToMissileLayer());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        
        if ((tag == "Player1Shield" && gameObject.tag == "Player1Projectile") || (tag == "Player2Shield" && gameObject.tag == "Player2Projectile"))
        {
            return;
        }
        else
        {
            if (type != Weapon.BAZOOKA && type != Weapon.SNIPER)
            {
                if (tag == "Player1")
                {
                    GameManager.Instance.Players[0].ProjectileHit(gameObject, type);
                }
                else if (tag == "Player2")
                {
                    GameManager.Instance.Players[1].ProjectileHit(gameObject, type);
                }
            }
            else
            {
                // It is a rocket.
                if (Vector3.Distance(GameManager.Instance.Players[0].transform.position, transform.position) < explosionRadius)
                {
                    GameManager.Instance.Players[0].ExplosionHit(transform.position);
                }
                if (Vector3.Distance(GameManager.Instance.Players[1].transform.position, transform.position) < explosionRadius)
                {
                    GameManager.Instance.Players[1].ExplosionHit(transform.position);
                }
                RocketFeedback();
            }

            if (!isPlayingDestructionAnimation)
            {
                StartCoroutine(DestroyBullet());
            }
        }
    }
    private void FixedUpdate()
    {
        if (type != Weapon.SNIPER)
        {
            if (!isPlayingDestructionAnimation)
            {

                bulletRigidbody2D.velocity = transform.right * speed;
            }
        }
    }
    #endregion

}
