using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStunController : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Setup variables
    [HideInInspector] public PlayerManager _playerManager = null;
    
    // Inspector variables
    [SerializeField] float stunDuration = 0.5f;
    [SerializeField] float _stunForceMultiplier = 500;
    [SerializeField] float _projectileHitStunWindow = 0.25f;
    [SerializeField] float stunDragValue = 4f;

    // Private variables
    float _stunTimer;
    float _stunOpportunityTimer;
    float defaultDrag;
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
    public float StunTimer => _stunTimer;
    public float StunForceMultiplier => _stunForceMultiplier;
    public float ProjectileHitStunWindow
    {
        get
        {
            return _projectileHitStunWindow;
        }
        set
        {
            _projectileHitStunWindow = value;
        }
    }
    public float StunOpportunityTimer
    {
        get
        {
            return _stunOpportunityTimer;
        }
        set
        {
            _stunOpportunityTimer = value;
        }
    }
    #endregion

    // Public methods
    #region Public methods
    public void Stun()
    {
        _stunTimer = stunDuration;
        PlayerManager.LinearDrag = stunDragValue;
        PlayerManager.DisplayStunEffect(stunDuration);
    }
    public void Init()
    {
        defaultDrag = PlayerManager.LinearDrag;
    }
    #endregion

    // Private methods
    #region Private methods
    void DisableGravityIfApplicable()
    {
        if (StunTimer > 0) // If stunned.
        {
            PlayerManager.Gravity = 0; // Set gravity to 0.
        }
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void FixedUpdate()
    {
        DisableGravityIfApplicable();
    }
    private void Update()
    {
        if (StunTimer <= 0) // If player is not stunned.
        {
            // Reset gravity after the player has just exited stun mode where gravity is set to 0.
            if (Mathf.Abs(PlayerManager.Gravity) != 2)
            {
                PlayerManager.ResetGravity();
                PlayerManager.LinearDrag = defaultDrag; // Reset drag
            }
        }

        _stunTimer -= Time.deltaTime;
        _stunOpportunityTimer -= Time.deltaTime;
    }
    #endregion
}
