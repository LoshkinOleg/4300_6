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
    public float HorizontalInput => _horizontalInput;
    public float VerticalInput => _verticalInput;
    public float AimingHorizontalInput => _aimingHorizontalInput;
    public float AimingVerticalInput => _aimingVerticalInput;
    public bool TryingToOpenParachute
    {
        get
        {
            return _tryingToOpenParachute;
        }
        set
        {
            if (value)
            {
                PlayerManager.ToggleParachute();
            }
            _tryingToOpenParachute = value;
        }
    }
    public bool TryingToFire => _tryingToFire;
    public InputDevice Gamepad
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
        if (Gamepad != null)
        {
            if (PlayerManager.StunTimer > 0)
            {
                _horizontalInput = 0;
                _verticalInput = 0;
                _aimingHorizontalInput = 0;
                _aimingVerticalInput = 0;
                _tryingToFire = false;
                _tryingToOpenParachute = false;
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
                    if (PlayerManager.CurrentMovementMode != PlayerMovementController.MovementMode.JETPACK)
                    {
                        TryingToOpenParachute = true;
                    }
                }
                if (_gamepad.LeftBumper.WasReleased)
                {
                    if (PlayerManager.CurrentMovementMode != PlayerMovementController.MovementMode.JETPACK)
                    {
                        TryingToOpenParachute = false;
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
