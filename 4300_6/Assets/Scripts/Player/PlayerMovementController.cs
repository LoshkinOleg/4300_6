using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    // Classes and Enums
    public enum MovementMode
    {
        AIRBORNE,
        GROUND,
        JETPACK
    }

    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] float airborneHorizontalMovementForceMultiplier = 20;
    [SerializeField] float groundHorizontalVelocity = 3;

    // References
    [HideInInspector] public PlayerManager _playerManager = null;

    // Public properties
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
    public MovementMode currentMovementMode
    {
        get
        {
            return _currentMovementMode;
        }
        set
        {
            if (value == MovementMode.JETPACK)
            {
                // Close the parachute if it's open.
                if (playerManager.parachuteIsOpen)
                {
                    playerManager.ToggleParachute();
                }
                playerManager.gravity = 0;
                jetpackTimer = PickupManager.instance.jetpackDuration;
            }
            _currentMovementMode = value;
        }
    }

    // Private variables
    MovementMode _currentMovementMode = MovementMode.AIRBORNE;
    float jetpackTimer;
    #endregion

    // Public methods
    #region Public methods
    public void Init()
    {
        currentMovementMode = MovementMode.AIRBORNE;
    }
    #endregion

    // Private methods
    #region Private methods
    void Move()
    {
        switch (_currentMovementMode)
        {
            case MovementMode.AIRBORNE:
                {
                    // Handle horizontal input
                    if (playerManager.inputHandler.horizontalInput != 0)
                    {
                        playerManager.physicsHandler.AddForce(Vector2.right * playerManager.inputHandler.horizontalInput * airborneHorizontalMovementForceMultiplier);
                    }
                }
                break;
            case MovementMode.GROUND:
                {
                    // Controlls horizontal movement precisely by affecting velocity.
                    if (playerManager.inputHandler.horizontalInput != 0)
                    {
                        playerManager.velocity = new Vector2(playerManager.inputHandler.horizontalInput * groundHorizontalVelocity, playerManager.physicsHandler.velocity.y);
                    }
                    else
                    {
                        playerManager.velocity = new Vector2(0, playerManager.physicsHandler.velocity.y);
                    }
                }
                break;
            case MovementMode.JETPACK:
                {
                    if (jetpackTimer > 0)
                    {
                        // Controls all movement precisely by affecting velocity.
                        playerManager.velocity = new Vector2(playerManager.inputHandler.horizontalInput, playerManager.inputHandler.verticalInput) * PickupManager.instance.jetpackVelocity;
                    }
                    else
                    {
                        // Exit jetpack mode.
                        _currentMovementMode = MovementMode.AIRBORNE;
                        playerManager.physicsHandler.ResetGravity();
                    }
                }
                break;
        }
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void FixedUpdate()
    {
        Move();
    }
    private void Update()
    {
        jetpackTimer -= Time.deltaTime;
    }
    #endregion
}
