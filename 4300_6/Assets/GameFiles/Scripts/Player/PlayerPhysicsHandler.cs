using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysicsHandler : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] float _playerSpeedLimit = 5;
    [SerializeField] float screenEdgeBuffer = 1f;
    [SerializeField] float bufferForceMultiplier = 35f;

    // References
    [HideInInspector] public PlayerManager playerManager = null;
    Rigidbody2D playerRigidbody = null;
    CapsuleCollider2D playerCollider = null;
    #endregion

    // Public properties
    #region Public properties
    public PlayerManager PlayerManager
    {
        get
        {
            return playerManager;
        }
        set
        {
            if (playerManager == null)
            {
                playerManager = value;
            }
            else
            {
                Debug.LogWarning("Attempting to modify PlayerManager reference after it has been set.");
            }
        }
    }
    public float Gravity
    {
        get
        {
            if (playerRigidbody != null)
            {
                return playerRigidbody.gravityScale;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return 0;
            }
        }
        set
        {
            if (playerRigidbody != null)
            {
                playerRigidbody.gravityScale = value;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
            }
        }
    }
    public Vector2 Velocity
    {
        get
        {
            if (playerRigidbody != null)
            {
                return playerRigidbody.velocity;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return new Vector2();
            }
        }
        set
        {
            if (playerRigidbody != null)
            {
                playerRigidbody.velocity = value;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
            }
        }
    }
    public float PlayerSpeedLimit => _playerSpeedLimit;
    public float LinearDrag
    {
        get
        {
            if (playerRigidbody != null)
            {
                return playerRigidbody.drag;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return 0;
            }
        }
        set
        {
            if (playerRigidbody != null)
            {
                playerRigidbody.drag = value;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
            }
        }
    }
    #endregion

    // Public methods
    #region Public methods
    public void ProjectileHit(GameObject projectile, Weapon type)
    {
        Vector2 directionOfKnockback = -Vector3.Normalize((Vector2)projectile.transform.position - (Vector2)transform.position);
        switch (type)
        {
            case Weapon.PISTOL:
                {
                    Vector2 forceToApply = directionOfKnockback * PlayerManager.WeaponsData[0].hitKnockback;
                    playerRigidbody.AddForce(forceToApply);
                }
                break;
            case Weapon.SHOTGUN:
                {
                    Vector2 forceToApply = directionOfKnockback * PlayerManager.WeaponsData[1].hitKnockback;
                    playerRigidbody.AddForce(forceToApply);
                }
                break;
            case Weapon.MINIGUN:
                {
                    Vector2 forceToApply = directionOfKnockback * PlayerManager.WeaponsData[4].hitKnockback;
                    playerRigidbody.AddForce(forceToApply);
                }
                break;
            default:
                {
                    Debug.LogWarning("PlayerPhysicsHandler.cs: ProjectileHit() got passed a non valid projectile type: " + type);
                }break;
        }

    }
    public void CrateBottomHit(BoxCollider2D crate)
    {
        if (playerCollider.IsTouching(crate))
        {
            if (playerCollider.IsTouching(GameManager.Instance.BottomBoundsCollider))
            {
                PlayerManager.Kill();
            }
            else
            {
                playerRigidbody.AddForce(Vector2.down * PlayerManager.StunForceMultiplier);
            }
        }
    }
    public void ExplosionHit(Vector2 position)
    {
        Vector2 direction = -(position - (Vector2)transform.position);
        playerRigidbody.AddForce(direction * PlayerManager.WeaponsData[3].hitKnockback);
    }
    public void SniperHit()
    {
        Vector2 direction;
        if (PlayerManager.IsLeftPlayer)
        {
            direction = -(GameManager.Instance.Players[1].transform.position - transform.position);
        }
        else
        {
            direction = -(GameManager.Instance.Players[0].transform.position - transform.position);
        }
        playerRigidbody.AddForce(direction * PlayerManager.WeaponsData[2].hitKnockback);
    }
    public void ToggleGravity()
    {
        playerRigidbody.gravityScale = -playerRigidbody.gravityScale;
    }
    public void ResetGravity()
    {
        if (PlayerManager.ParachuteIsOpen)
        {
            playerRigidbody.gravityScale = -1;
        }
        else
        {
            playerRigidbody.gravityScale = 1;
        }
    }
    public void AddForce(Vector2 unitaryDirection, float magnitude)
    {
        playerRigidbody.AddForce(unitaryDirection * magnitude);
    }
    public bool IsTouching(Collider2D collider)
    {
        return playerCollider.IsTouching(collider);
    }
    public void Init()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
    }
    #endregion

    // Private methods
    #region Private methods
    void ApplySpeedLimit()
    {
        if (playerRigidbody != null)
        {
            if (PlayerManager.StunOpportunityTimer <= 0 && !PlayerManager.HasRecentlyShot)
            {
                float velocityFloat = playerRigidbody.velocity.magnitude;

                if (velocityFloat > _playerSpeedLimit)
                {
                    playerRigidbody.velocity = (Vector2)Vector3.Normalize(playerRigidbody.velocity) * _playerSpeedLimit;
                }
            }
        }
        else
        {
            Debug.LogWarning("Variable not set!");
        }
    }
    void ApplyBufferForces()
    {
        // Right buffer
        if (transform.position.x > GameManager.Instance.GameViewHorizontalDistanceInMeters / 2 - screenEdgeBuffer)
        {
            if (playerRigidbody != null)            playerRigidbody.AddForce(Vector2.left * bufferForceMultiplier);         else Debug.LogWarning("Variable not set!");
        }
        // Left buffer
        if (transform.position.x < -GameManager.Instance.GameViewHorizontalDistanceInMeters / 2 + screenEdgeBuffer)
        {
            if (playerRigidbody != null)            playerRigidbody.AddForce(Vector2.right * bufferForceMultiplier);        else Debug.LogWarning("Variable not set!");
        }
        // Top buffer
        if (transform.position.y > GameManager.Instance.GameViewVerticalDistanceInMeters / 2 - screenEdgeBuffer)
        {
            if (playerRigidbody != null)            playerRigidbody.AddForce(Vector2.down * bufferForceMultiplier);         else Debug.LogWarning("Variable not set!");
        }
        // Bottom buffer
        if (transform.position.y < -GameManager.Instance.GameViewVerticalDistanceInMeters / 2 + screenEdgeBuffer)
        {
            if (playerRigidbody != null)            playerRigidbody.AddForce(Vector2.up * bufferForceMultiplier);           else Debug.LogWarning("Variable not set!");
        }
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void FixedUpdate()
    {
        ApplyBufferForces();
        ApplySpeedLimit();
    }
    #endregion
}
