﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStunController : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] float stunDuration = 0.5f;
    [SerializeField] float _stunForceMultiplier = 500;
    [SerializeField] float _projectileHitStunWindow = 0.25f;
    [SerializeField] float stunDragValue = 4f;

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
    public float stunTimer => _stunTimer;
    public float stunForceMultiplier => _stunForceMultiplier;
    public float projectileHitStunWindow => _projectileHitStunWindow;

    // Private variables
    float _stunTimer = 0;
    #endregion

    // Public methods
    #region Public methods
    public void Stun()
    {
        _stunTimer = stunDuration;
        playerManager.physicsHandler.ModifyLinearDrag(stunDragValue);
    }
    public void CrateBottomHit(BoxCollider2D crate)
    {
        Stun();
    }
    public void Init()
    {

    }
    public void CrateSideStun()
    {
        Stun();
    }
    #endregion

    // Private methods
    #region Private methods
    void ProcessStunMechanic()
    {
        if (stunTimer > 0) // If stunned.
        {
            playerManager.gravity = 0; // Set gravity to 0.
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
                playerManager.physicsHandler.ModifyLinearDrag(1); // Reset drag
            }
        }

        _stunTimer -= Time.deltaTime;
    }
    #endregion
}
