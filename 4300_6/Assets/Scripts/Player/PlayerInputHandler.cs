using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerInputHandler : MonoBehaviour
{
    // Attributes
    #region Attributes
    // References
    [HideInInspector] public PlayerManager _playerManager = null;

    // Private variables
    float _horizontalInput;
    float _verticalInput;
    float _aimingHorizontalInput;
    float _aimingVerticalInput;
    bool _tryingToOpenParachute;
    bool _tryingToFire;
    InputDevice _gamepad = null;
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
    public float horizontalInput => _horizontalInput;
    public float verticalInput => _verticalInput;
    public float aimingHorizontalInput => _aimingHorizontalInput;
    public float aimingVerticalInput => _aimingVerticalInput;
    public bool tryingToOpenParachute
    {
        get
        {
            return _tryingToOpenParachute;
        }
        set
        {
            if (value)
            {
                playerManager.ToggleParachute();
            }
            _tryingToOpenParachute = value;
        }
    }
    public bool tryingToFire => _tryingToFire;
    public InputDevice gamepad
    {
        get
        {
            return _gamepad;
        }
        set
        {
            _gamepad = value;
        }
    }
    #endregion

    // Public methods
    #region Public methods
    public void Init()
    {

    }
    #endregion

    // Private methods
    #region Private methods
    void UpdateInputs()
    {
        if (gamepad != null)
        {
            if (playerManager.stunTimer > 0)
            {
                _horizontalInput = 0;
                _verticalInput = 0;
                _aimingHorizontalInput = 0;
                _aimingVerticalInput = 0;
            }
            else
            {
                // Handle analog sticks inputs.
                _horizontalInput = _gamepad.LeftStick.X;
                _verticalInput = _gamepad.LeftStick.Y;
                _aimingHorizontalInput = _gamepad.RightStick.X;
                _aimingVerticalInput = _gamepad.RightStick.Y;

                // Handle parachute inputs toggling in applicable movement modes.
                if (_gamepad.LeftBumper.WasPressed)
                {
                    if (playerManager.currentMovementMode != PlayerMovementController.MovementMode.JETPACK)
                    {
                        tryingToOpenParachute = true;
                    }
                }
                if (_gamepad.LeftBumper.WasReleased)
                {
                    if (playerManager.currentMovementMode != PlayerMovementController.MovementMode.JETPACK)
                    {
                        tryingToOpenParachute = false;
                    }
                }

                // Handle firing inputs.
                if (_gamepad.RightBumper.WasPressed)
                {
                    _tryingToFire = true;
                }
                if (_gamepad.RightBumper.WasReleased)
                {
                    _tryingToFire = false;
                }
            }
        }
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Update()
    {
        UpdateInputs();
    }
    #endregion
}
