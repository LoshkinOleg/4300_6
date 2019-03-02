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
    public void ResetInputs()
    {
        _horizontalInput = 0;
        _verticalInput = 0;
        _aimingHorizontalInput = 0;
        _aimingVerticalInput = 0;
        _tryingToFire = false;
        _tryingToOpenParachute = false;
    }
    #endregion

    // Private methods
    #region Private methods
    void UpdateInputs()
    {
        // Handle gamepad controls
        if (Gamepad != null)
        {
            if (!GameManager.Instance.IsPaused)
            {
                if (PlayerManager.StunTimer > 0)
                {
                    ResetInputs();
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
                         TryingToOpenParachute = true;
                    }
                    if (_gamepad.LeftBumper.WasReleased)
                    {
                        TryingToOpenParachute = false;
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
        else // Handle keyboard controls.
        {
            if (!GameManager.Instance.IsPaused)
            {

                if (PlayerManager.IsLeftPlayer)
                {
                    if (PlayerManager.StunTimer > 0)
                    {
                        ResetInputs();
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.A))
                        {
                            _horizontalInput = -1;
                        }
                        else if (Input.GetKey(KeyCode.D))
                        {
                            _horizontalInput = 1;
                        }
                        else
                        {
                            _horizontalInput = 0;
                        }
                        if (Input.GetKey(KeyCode.W))
                        {
                            _aimingVerticalInput = 1;
                        }
                        else if (Input.GetKey(KeyCode.S))
                        {
                            _aimingVerticalInput = -1;
                        }
                        else
                        {
                            _aimingVerticalInput = 0;
                        }

                        // Handle parachute inputs toggling in applicable movement modes.
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            TryingToOpenParachute = true;
                        }
                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            TryingToOpenParachute = false;
                        }

                        // Handle firing inputs.
                        if (Input.GetKeyDown(KeyCode.LeftControl))
                        {
                            _tryingToFire = true;
                        }
                        if (Input.GetKeyUp(KeyCode.LeftControl))
                        {
                            _tryingToFire = false;
                        }
                    }
                }
                else
                {
                    if (PlayerManager.StunTimer > 0)
                    {
                        ResetInputs();
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.LeftArrow))
                        {
                            _horizontalInput = -1;
                        }
                        else if (Input.GetKey(KeyCode.RightArrow))
                        {
                            _horizontalInput = 1;
                        }
                        else
                        {
                            _horizontalInput = 0;
                        }
                        if (Input.GetKey(KeyCode.UpArrow))
                        {
                            _aimingVerticalInput = 1;
                        }
                        else if (Input.GetKey(KeyCode.DownArrow))
                        {
                            _aimingVerticalInput = -1;
                        }
                        else
                        {
                            _aimingVerticalInput = 0;
                        }

                        // Handle parachute inputs toggling in applicable movement modes.
                        if (Input.GetKeyDown(KeyCode.Keypad0))
                        {
                            TryingToOpenParachute = true;
                        }
                        if (Input.GetKeyUp(KeyCode.Keypad0))
                        {
                            TryingToOpenParachute = false;
                        }

                        // Handle firing inputs.
                        if (Input.GetKeyDown(KeyCode.KeypadPlus))
                        {
                            _tryingToFire = true;
                        }
                        if (Input.GetKeyUp(KeyCode.KeypadPlus))
                        {
                            _tryingToFire = false;
                        }
                    }
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
