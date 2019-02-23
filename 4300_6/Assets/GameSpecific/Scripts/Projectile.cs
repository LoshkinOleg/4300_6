using System.Collections;
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
    [SerializeField] float rocketRotationPerFrameAmplitude = 7;

    // References
    [SerializeField] SpriteRenderer spriteRenderer = null;
    Rigidbody2D bulletRigidbody2D = null;
    CircleCollider2D bulletCollider = null;

    // Private variables
    float x; // x variable in the Cos(x) function. Used to create a sinusoidal movement for the rocket projectile.
    bool isPlayingDestructionAnimation = false;
    #endregion

    // Private methods
    #region Private methods
    IEnumerator DestroyBullet()
    {
        isPlayingDestructionAnimation = true;

        if (bulletRigidbody2D != null)          bulletRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;                               else Debug.LogWarning("Variable not set up!");
        if (bulletCollider != null)             bulletCollider.enabled = false;                                                                 else Debug.LogWarning("Variable not set up!");
        if (FeedbackManager.Instance != null)   FeedbackManager.Instance.DisplayBulletDestruction(transform.position, type, spriteRenderer);    else Debug.LogWarning("Variable not set up!");

        yield return new WaitForSeconds(visualFeedbackLifetime);

        Destroy(gameObject);
    }
    #endregion

    // Inherited methods
    #region Inherited Methods
    private void Start()
    {
        bulletRigidbody2D = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<CircleCollider2D>();
        if (FeedbackManager.Instance != null) FeedbackManager.Instance.DisplayAppropriateProjectile(spriteRenderer, type); else Debug.LogWarning("Variable not set up!");

        if (type == Weapon.SNIPER)
        {
            if (gameObject.tag == "Player1Projectile")
            {
                transform.position = GameManager.Instance.Player1.transform.position;
            }
            else
            {
                transform.position = GameManager.Instance.Player2.transform.position;
            }
            StartCoroutine(DestroyBullet());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type != Weapon.BAZOOKA && type != Weapon.SNIPER)
        {
            if (collision.gameObject.tag == "Player1")
            {
                if (GameManager.Instance != null)           GameManager.Instance.Player1.ProjectileHit(gameObject, type);           else Debug.LogWarning("Variable not set up!");
                if (FeedbackManager.Instance != null)       FeedbackManager.Instance.DisplayHitByProjectile(collision.gameObject);  else Debug.LogWarning("Variable not set up!");
            }
            else if (collision.gameObject.tag == "Player2")
            {
                if (GameManager.Instance != null)           GameManager.Instance.Player2.ProjectileHit(gameObject, type);           else Debug.LogWarning("Variable not set up!");
                if (FeedbackManager.Instance != null)       FeedbackManager.Instance.DisplayHitByProjectile(collision.gameObject);  else Debug.LogWarning("Variable not set up!");
            }
        }
        else
        {
            // It is a rocket.
            if (Vector3.Distance(GameManager.Instance.Player1.transform.position, transform.position) < explosionRadius)
            {
                if (GameManager.Instance != null)           GameManager.Instance.Player1.ExplosionHit(transform.position);                              else Debug.LogWarning("Variable not set up!");
                if (FeedbackManager.Instance != null)       FeedbackManager.Instance.DisplayHitByProjectile(GameManager.Instance.Player1.gameObject);   else Debug.LogWarning("Variable not set up!");
            }
            if (Vector3.Distance(GameManager.Instance.Player2.transform.position, transform.position) < explosionRadius)
            {
                if (GameManager.Instance != null)           GameManager.Instance.Player2.ExplosionHit(transform.position);                              else Debug.LogWarning("Variable not set up!");
                if (FeedbackManager.Instance != null)       FeedbackManager.Instance.DisplayHitByProjectile(GameManager.Instance.Player2.gameObject);   else Debug.LogWarning("Variable not set up!");
            }
            if (FeedbackManager.Instance != null)           FeedbackManager.Instance.RocketFeedback();                              else Debug.LogWarning("Variable not set up!");
        }

        if (!isPlayingDestructionAnimation)
        {
            StartCoroutine(DestroyBullet());
        }
    }
    private void FixedUpdate()
    {
        if (type == Weapon.BAZOOKA)
        {
            if (!isPlayingDestructionAnimation) // Not currently self destroying
            {
                // Apply sinusoidal rotation. Note: Mathf.Cos(2*x*Mathf.PI + Mathf.PI/2) is a cos function with a frequency of 1[Hz], an amplitude of 1, whose incept is at 0[y] and that starts off by going into the negatives.
                transform.rotation *= Quaternion.Euler(0,0, Mathf.Cos(2*x*Mathf.PI + Mathf.PI/2) * rocketRotationPerFrameAmplitude);

                // Move the projectile.
                if (bulletRigidbody2D != null)          bulletRigidbody2D.velocity = transform.right * speed;           else Debug.LogWarning("Variable not set up!");
            }
        }
        else if (type != Weapon.SNIPER)
        {
            if (!isPlayingDestructionAnimation)
            {

                if (bulletRigidbody2D != null)          bulletRigidbody2D.velocity = transform.right * speed;           else Debug.LogWarning("Variable not set up!");
            }
        }

        x += Time.fixedDeltaTime;
    }
    #endregion

}
