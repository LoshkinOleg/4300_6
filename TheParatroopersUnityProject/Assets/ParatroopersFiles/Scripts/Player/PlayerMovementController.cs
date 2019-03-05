using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    // Classes and Enums
    public enum MovementMode
    {
        AIRBORNE,
        GROUND
    }

    // Attributes
    #region Attributes
    // Set up variables
    [HideInInspector] public PlayerManager _playerManager = null;

    // Inspector variables
    [SerializeField] float airborneHorizontalMovementForceMultiplier = 20;
    [SerializeField] float groundHorizontalVelocity = 3;
    
    // Private variables
    MovementMode _currentMovementMode = MovementMode.AIRBORNE;
    float jetpackTimer;
    #endregion

    // Public properties
    #region Public properties
    public PlayerManager PlayerManager
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
    public MovementMode CurrentMovementMode
    {
        get
        {
            return _currentMovementMode;
        }
        set
        {
            _currentMovementMode = value;
        }
    }
    #endregion

    // Public methods
    #region Public methods
    public void Init()
    {
        CurrentMovementMode = MovementMode.AIRBORNE;
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
                    if (PlayerManager.HorizontalInput != 0)
                    {
                        PlayerManager.AddForce(Vector2.right, PlayerManager.HorizontalInput * airborneHorizontalMovementForceMultiplier);
                    }
                }
                break;
            case MovementMode.GROUND:
                {
                    // Controlls horizontal movement precisely by affecting velocity.
                    if (PlayerManager.HorizontalInput != 0)
                    {
                        PlayerManager.Velocity = new Vector2(PlayerManager.HorizontalInput * groundHorizontalVelocity, PlayerManager.Velocity.y);
                    }
                    else
                    {
                        PlayerManager.Velocity = new Vector2(0, PlayerManager.Velocity.y);
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
