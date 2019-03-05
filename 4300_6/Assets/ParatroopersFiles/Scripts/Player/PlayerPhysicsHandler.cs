﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysicsHandler : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Setup variables
    [HideInInspector] public PlayerManager playerManager = null;

    // Inspector variables
    [SerializeField] float _playerSpeedLimit = 5;
    [SerializeField] float screenEdgeBuffer = 1f;
    [SerializeField] float bufferForceMultiplier = 35f;

    // References
    Rigidbody2D playerRigidbody = null;
    CapsuleCollider2D playerCollider = null;

    // Private variables
    bool moveToRespawn;
    float lerp_journeyLength;
    float lerp_startTime;
    float lerp_speed;
    Vector3 lerp_startingPosition;
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
    public void Init()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
    }
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
    public void DisableCollisions()
    {
        playerRigidbody.simulated = false;
    }
    public void MoveToRespawn()
    {
        lerp_journeyLength = Vector3.Distance(new Vector3(),transform.position); // Respawn is at (0,0,0)
        lerp_startTime = Time.time;
        lerp_speed = lerp_journeyLength; // v = d/t, here d is journeyLength and t is 1 second.
        lerp_startingPosition = transform.position;

        moveToRespawn = true;
    }
    #endregion

    // Private methods
    #region Private methods
    void ApplySpeedLimit()
    {
        if (PlayerManager.CurrentWeapon != Weapon.PISTOL && PlayerManager.CurrentWeapon != Weapon.MINIGUN)
        {
            if (PlayerManager.StunOpportunityTimer <= 0 && !PlayerManager.HasRecentlyShot)
            {
                if (playerRigidbody.velocity.magnitude > _playerSpeedLimit)
                {
                    playerRigidbody.velocity = (Vector2)Vector3.Normalize(playerRigidbody.velocity) * _playerSpeedLimit;
                }
            }
        }
        else // Always apply speed limit to pistol and bazooka since their firerates are too high, resulting int speed limit completely removed for the entire duration when the player's firing.
        {
            if (playerRigidbody.velocity.magnitude > _playerSpeedLimit)
            {
                playerRigidbody.velocity = (Vector2)Vector3.Normalize(playerRigidbody.velocity) * _playerSpeedLimit;
            }
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
        if (moveToRespawn)
        {
            float distanceCovered = (Time.time - lerp_startTime) * lerp_speed;
            float journeyFraction = distanceCovered / lerp_journeyLength;
            transform.position = Vector3.Lerp(lerp_startingPosition, new Vector3(), journeyFraction);

            if (Vector3.Distance(transform.position, new Vector3()) <= 0.05f)
            {
                playerRigidbody.simulated = true;
                playerRigidbody.velocity = new Vector2();
                moveToRespawn = false;
            }
        }
        else
        {
            ApplyBufferForces();
            ApplySpeedLimit();
        }
    }
    #endregion
}
