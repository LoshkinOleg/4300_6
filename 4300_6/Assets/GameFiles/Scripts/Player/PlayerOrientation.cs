using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOrientation : MonoBehaviour
{
    // Classes and Enums
    #region Classes and Enums
    public enum PlayerDirection
    {
        LEFT,
        RIGHT
    }
    public enum GunDirection
    {
        FORWARD,
        UP,
        DOWN,
        ANYWHERE
    }
    #endregion

    // Attributes
    #region Attributes
    // References
    [HideInInspector] public PlayerManager _playerManager = null;
    [SerializeField] Transform _armTransform = null;

    // Private variables
    PlayerDirection currentPlayerDirection;
    GunDirection currentGunDirection = GunDirection.FORWARD;
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
    public Transform ArmsTransform => _armTransform;
    #endregion

    // Public methods
    #region Public methods
    public void Init()
    {
        if (PlayerManager.IsLeftPlayer)
        {
            currentPlayerDirection = PlayerDirection.RIGHT;
        }
        else
        {
            currentPlayerDirection = PlayerDirection.LEFT;
        }
    }
    #endregion

    // Private methods
    #region Private methods
    void HandlePlayerOrientation()
    {
        if (PlayerManager.HorizontalInput != 0)
        {
            if (PlayerManager.HorizontalInput > 0)
            {
                // H+
                currentPlayerDirection = PlayerDirection.RIGHT;
            }
            else
            {
                // H-
                currentPlayerDirection = PlayerDirection.LEFT;
            }
        }
        else
        {
            // H = 0
            // Face enemy if the joystick is in it's neutral position.
            if (CheckEnemyDirection())
            {
                currentPlayerDirection = PlayerDirection.RIGHT;
            }
            else
            {
                currentPlayerDirection = PlayerDirection.LEFT;
            }
        }
    }
    void HandleGunOrientation()
    {
        if (PlayerManager.CurrentMovementMode != PlayerMovementController.MovementMode.GROUND)
        {
            // If player is aiming up or down.
            if (Mathf.Abs(PlayerManager.AimingVerticalInput) >= Mathf.Abs(PlayerManager.AimingHorizontalInput))
            {
                if (PlayerManager.AimingVerticalInput > 0)
                {
                    // V+
                    currentGunDirection = GunDirection.DOWN;
                }
                else if (PlayerManager.AimingVerticalInput < 0)
                {
                    // V-
                    currentGunDirection = GunDirection.UP;
                }
                else
                {   // V = 0
                    currentGunDirection = GunDirection.FORWARD;
                }
            }
            else // If the player is aiming left or right.
            {
                currentGunDirection = GunDirection.FORWARD;
                // Change player orientation if the player is going one way and firing in the other.
                if (PlayerManager.AimingHorizontalInput < 0 && currentPlayerDirection == PlayerDirection.RIGHT)
                {
                    currentPlayerDirection = PlayerDirection.LEFT;
                }
                else if (PlayerManager.AimingHorizontalInput > 0 && currentPlayerDirection == PlayerDirection.LEFT)
                {
                    currentPlayerDirection = PlayerDirection.RIGHT;
                }
            }
        }
        else // If current movement mode is GROUND
        {
            // Handle gun orientation
            currentGunDirection = GunDirection.ANYWHERE;
        }
    }
    void OrientSpriteAndGun()
    {
        // Analyse inputs.
        HandlePlayerOrientation();
        HandleGunOrientation();

        // Apply orientations accordingly.
        if (PlayerManager.CurrentMovementMode != PlayerMovementController.MovementMode.GROUND)
        {
            switch (currentPlayerDirection)
            {
                case PlayerDirection.LEFT:
                    {
                        transform.eulerAngles = new Vector3(0, 180, 0);

                        if (_armTransform != null)
                        {
                            switch (currentGunDirection)
                            {
                                case GunDirection.FORWARD:
                                    {
                                        _armTransform.localEulerAngles = new Vector3(0, 0, 0);
                                    }
                                    break;
                                case GunDirection.UP:
                                    {
                                        _armTransform.localEulerAngles = new Vector3(0, 0, -90);
                                    }
                                    break;
                                case GunDirection.DOWN:
                                    {
                                        _armTransform.localEulerAngles = new Vector3(0, 0, 90);
                                    }
                                    break;
                                case GunDirection.ANYWHERE:
                                    {
                                        _armTransform.localEulerAngles = new Vector3(0, 180, Mathf.Atan2(PlayerManager.AimingVerticalInput, PlayerManager.AimingHorizontalInput) * Mathf.Rad2Deg);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Variable is not set up!");
                        }

                    }
                    break;
                case PlayerDirection.RIGHT:
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);

                        if (_armTransform != null)
                        {
                            switch (currentGunDirection)
                            {
                                case GunDirection.FORWARD:
                                    {
                                        _armTransform.localEulerAngles = new Vector3(0, 0, 0);
                                    }
                                    break;
                                case GunDirection.UP:
                                    {
                                        _armTransform.localEulerAngles = new Vector3(0, 0, -90);
                                    }
                                    break;
                                case GunDirection.DOWN:
                                    {
                                        _armTransform.localEulerAngles = new Vector3(0, 0, 90);
                                    }
                                    break;
                                case GunDirection.ANYWHERE:
                                    {
                                        _armTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(PlayerManager.AimingVerticalInput, PlayerManager.AimingHorizontalInput) * Mathf.Rad2Deg);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Variable is not set up!");
                        }
                    }
                    break;
            }
        }
        else // If it is GROUND
        {
            if (PlayerManager.AimingHorizontalInput > 0)
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
                if (_armTransform != null)          _armTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(PlayerManager.AimingVerticalInput, PlayerManager.AimingHorizontalInput) * Mathf.Rad2Deg);            else Debug.LogWarning("Variable not set up!");
            }
            else if (PlayerManager.AimingHorizontalInput < 0)
            {
                transform.localEulerAngles = new Vector3(0, 180, 0);
                if (_armTransform != null)          _armTransform.localEulerAngles = new Vector3(180, 180, -Mathf.Atan2(PlayerManager.AimingVerticalInput, PlayerManager.AimingHorizontalInput) * Mathf.Rad2Deg);       else Debug.LogWarning("Variable not set up!");
            }
            else
            {
                if (CheckEnemyDirection())
                {
                    // enemy on the right
                    transform.localEulerAngles = new Vector3(0, 0, 0);
                    if (_armTransform != null)      _armTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(PlayerManager.AimingVerticalInput, PlayerManager.AimingHorizontalInput) * Mathf.Rad2Deg);            else Debug.LogWarning("Variable not set up!");
                }
                else
                {
                    // enemy on the left
                    transform.localEulerAngles = new Vector3(0, 180, 0);
                }
            }
        }
    }
    bool CheckEnemyDirection() // Returns true if the enemy is on the right or at the same x axis, false if he is on the left.
    {
        if (PlayerManager.IsLeftPlayer)
        {
            if (GameManager.Instance != null)
            {
                if ((GameManager.Instance.Player2.gameObject.transform.position - transform.position).x >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.LogWarning("Variable not set up!");
                return true;
            }
        }
        else
        {
            if (GameManager.Instance != null)
            {
                if ((GameManager.Instance.Player1.gameObject.transform.position - transform.position).x >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.LogWarning("Variable not set up!");
                return true;
            }
        }
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void FixedUpdate()
    {
        OrientSpriteAndGun();
    }
    #endregion
}
