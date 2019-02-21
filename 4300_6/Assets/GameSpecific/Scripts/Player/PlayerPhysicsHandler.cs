﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysicsHandler : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] float dragForce = 0.1f;
    [SerializeField] float _playerSpeedLimit = 5;
    [SerializeField] float screenEdgeBuffer = 1f;
    [SerializeField] float bufferForceMultiplier = 35f;

    // References
    [HideInInspector] public PlayerManager _playerManager = null;
    Rigidbody2D playerRigidbody = null;
    CapsuleCollider2D playerCollider = null;
    #endregion

    // Public properties
    #region Public properties
    public PlayerManager playerManager
    {
        get
        {
            return _playerManager;
        }
        set
        {
            if (_playerManager == null)
            {
                _playerManager = value;
            }
            else
            {
                Debug.LogWarning("Attempting to modify PlayerManager reference after it has been set.");
            }
        }
    }
    public float gravity
    {
        get
        {
            return playerRigidbody.gravityScale;
        }
        set
        {
            playerRigidbody.gravityScale = value;
        }
    }
    public Vector2 velocity
    {
        get
        {
            return playerRigidbody.velocity;
        }
        set
        {
            playerRigidbody.velocity = value;
        }
    }
    public float playerSpeedLimit => _playerSpeedLimit;
    public float linearDrag
    {
        get
        {
            return playerRigidbody.drag;
        }
        set
        {
            playerRigidbody.drag = value;
        }
    }
    #endregion

    // Public methods
    #region Public methods
    public void ProjectileHit(GameObject projectile, PlayerFiringController.Weapon type)
    {
        Vector2 directionOfKnockback = -Vector3.Normalize((Vector2)projectile.transform.position - (Vector2)transform.position);
        switch (type)
        {
            case PlayerFiringController.Weapon.PISTOL:
                {
                    Vector2 forceToApply = directionOfKnockback * playerManager.weaponsData[0].hitKnockback;
                    playerRigidbody.AddForce(forceToApply);
                }
                break;
            case PlayerFiringController.Weapon.SHOTGUN:
                {
                    Vector2 forceToApply = directionOfKnockback * playerManager.weaponsData[1].hitKnockback;
                    playerRigidbody.AddForce(forceToApply);
                }
                break;
            case PlayerFiringController.Weapon.MINIGUN:
                {
                    Vector2 forceToApply = directionOfKnockback * playerManager.weaponsData[4].hitKnockback;
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
        if (playerCollider.IsTouching(crate)) // BUG: not touching, is it because of the force up?
        {
            if (playerCollider.IsTouching(GameManager.Instance.BottomBoundsCollider))
            {
                playerManager.Kill();
            }
            else
            {
                playerRigidbody.AddForce(Vector2.down * playerManager.stunForceMultiplier);
            }
        }
    }
    public void ExplosionHit(Vector2 position)
    {
        Vector2 direction = -(position - (Vector2)transform.position);
        playerRigidbody.AddForce(direction * playerManager.weaponsData[3].hitKnockback);
    }
    public void SniperHit()
    {
        Vector2 direction;
        if (playerManager.isLeftPlayer)
        {
            direction = -(GameManager.Instance.Player2.transform.position - transform.position);
        }
        else
        {
            direction = -(GameManager.Instance.Player1.transform.position - transform.position);
        }
        playerRigidbody.AddForce(direction * playerManager.weaponsData[2].hitKnockback);
    }
    public void ToggleGravity()
    {
        playerRigidbody.gravityScale = -playerRigidbody.gravityScale;
    }
    public void ResetGravity()
    {
        if (playerManager.parachuteIsOpen)
        {
            playerRigidbody.gravityScale = -2;
        }
        else
        {
            playerRigidbody.gravityScale = 2;
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
    void ApplyDrag()
    {
        if (playerManager.horizontalInput == 0)
        {
            if (Mathf.Abs(playerRigidbody.velocity.x) > 0.05f) // Applies drag if the horizontal speed is greater than 0.05f
            {
                playerRigidbody.velocity += new Vector2(-Mathf.Sign(playerRigidbody.velocity.x) * dragForce, 0);
            }
            else // Stops all horizontal movement if the speed if smaller than 0.05f
            {
                playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
            }
        }
    }
    void ApplySpeedLimit()
    {
        float velocityFloat = playerRigidbody.velocity.magnitude;

        if (velocityFloat > _playerSpeedLimit)
        {
            playerRigidbody.velocity = (Vector2)Vector3.Normalize(playerRigidbody.velocity) * _playerSpeedLimit;
        }
    }
    void ApplyBufferForces()
    {
        // Right buffer
        if (transform.position.x > GameManager.Instance.GameViewHorizontalDistanceInMeters / 2 - screenEdgeBuffer)
        {
            playerRigidbody.AddForce(Vector2.left * bufferForceMultiplier);
        }
        // Left buffer
        if (transform.position.x < -GameManager.Instance.GameViewHorizontalDistanceInMeters / 2 + screenEdgeBuffer)
        {
            playerRigidbody.AddForce(Vector2.right * bufferForceMultiplier);
        }
        // Top buffer
        if (transform.position.y > GameManager.Instance.GameViewVerticalDistanceInMeters / 2 - screenEdgeBuffer)
        {
            playerRigidbody.AddForce(Vector2.down * bufferForceMultiplier);
        }
        // Bottom buffer
        if (transform.position.y < -GameManager.Instance.GameViewVerticalDistanceInMeters / 2 + screenEdgeBuffer)
        {
            playerRigidbody.AddForce(Vector2.up * bufferForceMultiplier);
        }
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void FixedUpdate()
    {
        ApplyBufferForces();
        // ApplySpeedLimit();
        // ApplyDrag();
    }
    #endregion
}
