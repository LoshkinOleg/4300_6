using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    // Attributes
    #region Attributes
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

    // Private variables
    float _horizontalInput;
    float _verticalInput;
    float _aimingHorizontalInput;
    float _aimingVerticalInput;
    bool _tryingToOpenParachute;
    bool _tryingToFire;
    #endregion

    // Public methods
    #region Public methods
    public void UpdateInputs()
    {
        if (playerManager.isLeftPlayer)
        {
            // Handle analog sticks inputs.
            _horizontalInput = Input.GetAxisRaw("Player1_Horizontal");
            _verticalInput = Input.GetAxisRaw("Player1_Vertical");
            _aimingHorizontalInput = Input.GetAxisRaw("Player1_Aiming_Horizontal");
            _aimingVerticalInput = Input.GetAxisRaw("Player1_Aiming_Vertical");

            // Handle parachute inputs toggling in applicable movement modes.
            if (Input.GetButtonDown("Player1_Parachute"))
            {
                if (playerManager.currentMovementMode != PlayerMovementController.MovementMode.JETPACK)
                {
                    tryingToOpenParachute = true;
                }
            }
            if (Input.GetButtonUp("Player1_Parachute"))
            {
                if (playerManager.currentMovementMode != PlayerMovementController.MovementMode.JETPACK)
                {
                    tryingToOpenParachute = false;
                }
            }
            
            // Handle firing inputs.
            if (Input.GetButtonDown("Player1_Fire"))
            {
                _tryingToFire = true;
            }
            if (Input.GetButtonUp("Player1_Fire"))
            {
                _tryingToFire = false;
            }
        }
        else
        {
            _horizontalInput = Input.GetAxisRaw("Player2_Horizontal");
            _verticalInput = Input.GetAxisRaw("Player2_Vertical");
            _aimingHorizontalInput = Input.GetAxisRaw("Player2_Aiming_Horizontal");
            _aimingVerticalInput = Input.GetAxisRaw("Player2_Aiming_Vertical");

            if (Input.GetButtonDown("Player2_Parachute"))
            {
                if (playerManager.currentMovementMode != PlayerMovementController.MovementMode.JETPACK)
                {
                    _tryingToOpenParachute = true;
                }
            }
            if (Input.GetButtonUp("Player2_Parachute"))
            {
                if (playerManager.currentMovementMode != PlayerMovementController.MovementMode.JETPACK)
                {
                    _tryingToOpenParachute = false;
                }
            }
            
            if (Input.GetButtonDown("Player2_Fire"))
            {
                _tryingToFire = true;
            }
            if (Input.GetButtonUp("Player2_Fire"))
            {
                _tryingToFire = false;
            }
        }
    }
    public void Init()
    {

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
