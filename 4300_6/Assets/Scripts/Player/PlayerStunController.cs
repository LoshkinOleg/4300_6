using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStunController : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] float stunDuration = 0.5f;
    [SerializeField] float stunForceMultiplier = 7;

    // References
    [HideInInspector] public PlayerManager _playerManager = null;
    BoxCollider2D lastCrateBottomHit_collider = null;

    // Public properties
    public PlayerManager playerManager
    {
        get
        {
            return _playerManager;
        }
        set
        {
            if (_playerManager = null)
            {
                _playerManager = value;
            }
            else
            {
                Debug.LogWarning("Attempting to modify PlayerManager reference after it has been set.");
            }
        }
    }
    public float stunTimer => _stunTimer;

    // Private variables
    float _stunTimer = 0;
    #endregion

    // Public methods
    #region Public methods
    public void Stun()
    {
        _stunTimer = stunDuration;
    }
    #endregion

    // Private methods
    #region Private methods
    void ProcessStunMechanic()
    {
        if (stunTimer > 0) // If stunned.
        {
            playerManager.gravity = 0; // Set gravity to 0.
            if (lastCrateBottomHit_collider != null)
            {
                if (playerManager.physicsHandler.IsTouching(GameManager.instance.bottomBoundsCollider) && playerManager.physicsHandler.IsTouching(lastCrateBottomHit_collider))
                {
                    playerManager.Kill(); // Kill player if he's caught between the bottom of a box and the bottom bounds.
                }
                else if (playerManager.physicsHandler.IsTouching(lastCrateBottomHit_collider))
                {
                    playerManager.physicsHandler.AddForce(Vector2.down * stunForceMultiplier);
                }
            }
        }
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void FixedUpdate()
    {
        ProcessStunMechanic();
    }
    private void Update()
    {
        if (stunTimer <= 0) // If player is not stunned.
        {
            // Reset gravity after the player has just exited stun mode where gravity is set to 0.
            if (Mathf.Abs(playerManager.gravity) != 2)
            {
                playerManager.physicsHandler.ResetGravity();
            }
        }

        _stunTimer -= Time.deltaTime;
    }
    #endregion
}
