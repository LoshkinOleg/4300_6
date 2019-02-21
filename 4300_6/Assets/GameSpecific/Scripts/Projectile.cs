using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Setup variables
    [HideInInspector] public float speed;
    [HideInInspector] public PlayerFiringController.Weapon type;

    // Inspector variables
    [SerializeField] float explosionRadius = 1.5f;

    // References
    [SerializeField] GameObject projectileSpriteGO = null;
    [SerializeField] GameObject destructionSpriteGO = null;
    [SerializeField] Sprite[] projectileSprites = new Sprite[(int)PlayerFiringController.Weapon.MINIGUN + 1];
    Rigidbody2D bulletRigidbody2D = null;
    CircleCollider2D bulletCollider = null;

    // Private variables
    bool isPlayingDestructionAnimation = false;
    float x; // x variable in the Cos(x) function. Used to create a sinusoidal movement for the rocket projectile.
    SpriteRenderer currentProjectileSprite = null;
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
        currentProjectileSprite = projectileSpriteGO.GetComponent<SpriteRenderer>();
        currentProjectileSprite.sprite = projectileSprites[(int)type];
    }

    private void FixedUpdate()
    {
        if (type != PlayerFiringController.Weapon.BAZOOKA || type != PlayerFiringController.Weapon.SNIPER)
        {
            if (!isPlayingDestructionAnimation)
            {
                bulletRigidbody2D.velocity = transform.right * speed;
            }
        }
        else if (type == PlayerFiringController.Weapon.BAZOOKA)
        {
            // It is a rocket
            if (!isPlayingDestructionAnimation)
            {
                // Apply sinusoidal rotation
                x += Time.fixedDeltaTime;

                // Mathf.Cos(x * Mathf.PI * 2) is a cos function with a frequency of 1 and an amplitude of 1.
                transform.rotation *= Quaternion.Euler(0,0, Mathf.Cos(x * Mathf.PI * 2) * 7); // The 7 is dark magic that sets up the amplitude of the function.

                bulletRigidbody2D.velocity = transform.right * speed;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type != PlayerFiringController.Weapon.BAZOOKA && type != PlayerFiringController.Weapon.SNIPER)
        {
            if (collision.gameObject.tag == "Player1")
            {
                GameManager.Instance.Player1.ProjectileHit(gameObject, type);
            }
            else if (collision.gameObject.tag == "Player2")
            {
                GameManager.Instance.Player2.ProjectileHit(gameObject, type);
            }
        }
        else
        {
            // It is a rocket.
            if (Vector3.Distance(GameManager.Instance.Player1.transform.position, transform.position) < explosionRadius)
            {
                GameManager.Instance.Player1.ExplosionHit(transform.position);
            }
            if (Vector3.Distance(GameManager.Instance.Player2.transform.position, transform.position) < explosionRadius)
            {
                GameManager.Instance.Player2.ExplosionHit(transform.position);
            }

            // Play audio
            SoundManager.Instance.PlaySound("bazooka_hit");
        }

        if (!isPlayingDestructionAnimation)
        {
            StartCoroutine(DestroyBullet());
        }
    }
    #endregion

}
