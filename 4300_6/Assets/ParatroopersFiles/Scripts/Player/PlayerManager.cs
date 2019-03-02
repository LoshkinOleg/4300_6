using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public enum Weapon
{
    PISTOL,
    SHOTGUN,
    SNIPER,
    BAZOOKA,
    MINIGUN
}

public class PlayerManager : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] bool _isLeftPlayer = true;
    [SerializeField] Weapon startingWeapon = Weapon.PISTOL;
    [SerializeField] int startingHealth = 10;

    // References
    [SerializeField] Transform _frontArmTransform = null;
    [SerializeField] Transform _backArmTransform = null;
    [SerializeField] Transform _firingAndReloadingFX_Transform = null;
    [SerializeField] SpriteRenderer _parachute_SpriteRenderer = null;
    [SerializeField] SpriteRenderer _frontArm_SpriteRenderer = null;
    [SerializeField] SpriteRenderer _backArm_SpriteRenderer = null;
    [SerializeField] SpriteRenderer _firingAndReloadingFX_SpriteRenderer = null;
    [SerializeField] WeaponData[] _weaponsData = new WeaponData[(int)Weapon.MINIGUN + 1]; // 0: pistol, 1: shotgun, 2: sniper, 3: bazooka, 4: minigun
    PlayerMovementController movementController = null;
    PlayerFiringController firingController = null;
    PlayerOrientation orientationController = null;
    PlayerPhysicsHandler physicsHandler = null;
    PlayerInputHandler inputHandler = null;
    PlayerStunController stunController = null;
    PlayerFeedbackController feedbackController = null;
    PlayerUIController uiController = null;

    // Private variables
    int _health = 10;
    bool _parachuteIsOpen;
    #endregion

    // Public properties
    #region Public properties
    // PlayerManager's properties
    public WeaponData[] WeaponsData => _weaponsData;
    public bool IsLeftPlayer => _isLeftPlayer;
    public bool ParachuteIsOpen => _parachuteIsOpen;
    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            // Apply bounds to passed value.
            value = value < 0 ? 0 : value;
            value = value > 10 ? 10 : value;

            if (value - _health < 0) // If we're being damaged
            {
                feedbackController.HighlightPlayer(Color.red);
                feedbackController.PlayHurtSound();
            }
            else // If we're being healed.
            {
                feedbackController.HighlightPlayer(Color.green);
            }

            uiController.HighlightHealthBar(value - _health);
            _health = value;
        }
    }
    public Transform FrontArmTransform => _frontArmTransform;
    public Transform BackArmTransform => _backArmTransform;
    public Transform FiringAndReloadingFX_Transform => _firingAndReloadingFX_Transform;
    public SpriteRenderer Parachute_SpriteRenderer => _parachute_SpriteRenderer;
    public SpriteRenderer FrontArm_SpriteRenderer => _frontArm_SpriteRenderer;
    public SpriteRenderer BackArm_SpriteRenderer => _backArm_SpriteRenderer;
    public SpriteRenderer FiringAndReloadingFX_SpriteRenderer => _firingAndReloadingFX_SpriteRenderer;
    // Movement controller
    public PlayerMovementController.MovementMode CurrentMovementMode
    {
        get
        {
            return movementController.CurrentMovementMode;
        }
        set
        {
            movementController.CurrentMovementMode = value;
        }
    }
    // Firing controller
    public Weapon CurrentWeapon => firingController.CurrentWeapon;
    public int CurrentAmmo => firingController.CurrentAmmo;
    public bool HasRecentlyShot => firingController.HasRecentlyShot;
    public PlayerFiringController.MinigunStage CurrentMinigunStage => firingController.CurrentMinigunStage;
    // Animation and Orientation controller
    public Transform ArmsTransform => orientationController.ArmsTransform;
    // Physics handler
    public float SpeedLimit => physicsHandler.PlayerSpeedLimit;
    public float Gravity
    {
        get
        {
            return physicsHandler.Gravity;
        }
        set
        {
            physicsHandler.Gravity = value;
        }
    }
    public Vector2 Velocity
    {
        get
        {
            return physicsHandler.Velocity;
        }
        set
        {
            physicsHandler.Velocity = value;
        }
    }
    public float LinearDrag
    {
        get
        {
            return physicsHandler.LinearDrag;
        }
        set
        {
            physicsHandler.LinearDrag = value;
        }
    }
    // Input handler
    public InputDevice Gamepad
    {
        get
        {
            return inputHandler.Gamepad;
        }
        set
        {
            inputHandler.Gamepad = value;
        }
    }
    public float HorizontalInput => inputHandler.HorizontalInput;
    public float VerticalInput => inputHandler.VerticalInput;
    public float AimingHorizontalInput => inputHandler.AimingHorizontalInput;
    public float AimingVerticalInput => inputHandler.AimingVerticalInput;
    public bool TryingToOpenParachute => inputHandler.TryingToOpenParachute;
    public bool TryingToFire => inputHandler.TryingToFire;
    // Stun controller
    public float StunTimer => stunController.StunTimer;
    public float StunForceMultiplier => stunController.StunForceMultiplier;
    public float StunOpportunityTimer => stunController.StunOpportunityTimer;
    public float ProjectileHitStunWindow => stunController.ProjectileHitStunWindow;
    // Feedback controller
    public float BulletDestructionFeedbackLifetime => feedbackController.BulletDestructionFeedbackLifetime;
    public float SniperDestructionFeedbackLifetime => feedbackController.SniperDestructionFeedbackLifetime;
    public float BazookaDestructionFeedbackLifetime => feedbackController.BazookaDestructionFeedbackLifetime;
    #endregion

    // PUBLIC METHODS
    #region Public Methods
    // PlayerManager's methods
    public void ModifyHealth(int damage) // Use to damage (pass a negative number) or to heal (pass a positive number) the player.
    {
        Health += damage;
    }
    public void ProjectileHit(GameObject projectile, Weapon type) // Damages player and relays the message to the physics component for knockback.
    {
        switch (type)
        {
            case Weapon.PISTOL:
                {
                    ModifyHealth(-WeaponsData[0].damage);
                    physicsHandler.ProjectileHit(projectile, type);
                }
                break;
            case Weapon.SHOTGUN:
                {
                    ModifyHealth(-WeaponsData[1].damage);
                    physicsHandler.ProjectileHit(projectile, type);
                }
                break;
            case Weapon.MINIGUN:
                {
                    ModifyHealth(-WeaponsData[4].damage);
                    physicsHandler.ProjectileHit(projectile, type);
                }
                break;
            default:
                {
                    Debug.LogWarning("PlayerManager.cs: ProjectileHit() got passed a non valid projectile type: " + type);
                }
                break;
        }
        // Reset stun opportunity timer.
        stunController.StunOpportunityTimer = ProjectileHitStunWindow;
    }
    public void CrateBottomHit(BoxCollider2D crate)
    {
        if (ParachuteIsOpen)
        {
            ToggleParachute();
        }
        stunController.Stun();
        physicsHandler.CrateBottomHit(crate);
    }
    public void ExplosionHit(Vector3 position)
    {
        ModifyHealth(-WeaponsData[3].damage);
        physicsHandler.ExplosionHit(position);
        stunController.StunOpportunityTimer = ProjectileHitStunWindow;
    }
    public void SniperHit()
    {
        ModifyHealth(-WeaponsData[2].damage);
        physicsHandler.SniperHit();
        stunController.StunOpportunityTimer = ProjectileHitStunWindow;
    }
    public void Kill()
    {
        _health = 10;
        uiController.ResetHealthbar();
        if (IsLeftPlayer) // Increment the opponent's killstreak.
        {
            GameManager.Instance.Players[1].IncrementKillstreak();
        }
        else
        {
            GameManager.Instance.Players[0].IncrementKillstreak();
        }
        uiController.ResetKillstreak(); // Reset own killstreak.
        Respawn();
    }
    public void ToggleParachute()
    {
        physicsHandler.ToggleGravity();
        feedbackController.ToggleParachute();
        _parachuteIsOpen = !_parachuteIsOpen;
    }
    public void SwitchToWeapon(Weapon type)
    {
        firingController.CurrentWeapon = type;
    }
    public void ResetInputs()
    {
        inputHandler.ResetInputs();
    }
    // Firing controller
    public void SpeedBulletsUp()
    {
        firingController.SpeedBulletsUp();
    }
    // Physics handler
    public void ToggleGravity()
    {
        physicsHandler.ToggleGravity();
    }
    public void ResetGravity()
    {
        physicsHandler.ResetGravity();
    }
    public bool IsTouching(Collider2D collider)
    {
        return physicsHandler.IsTouching(collider);
    }
    public void ApplyFiringKnockback(Vector2 unitaryDirection, float magnitude)
    {
        physicsHandler.AddForce(unitaryDirection, magnitude);
    }
    public void AddForce(Vector2 unitaryDirection, float magnitude)
    {
        physicsHandler.AddForce(unitaryDirection, magnitude);
    }
    // Stun controller
    public void Stun()
    {
        stunController.Stun();
    }
    // Feedback controller
    public void UpdateCurrentWeaponSprite()
    {
        feedbackController.UpdateCurrentWeaponSprite();
    }
    public void DisplayAmmoLeft(string text,Color color)
    {
        feedbackController.DisplayAmmoLeft(text,color);
    }
    public void PlayMinigunSound(PlayerFiringController.MinigunStage stage)
    {
        feedbackController.PlayMinigunSound(stage);
    }
    public void InstantiateWeaponCatridge()
    {
        feedbackController.InstantiateWeaponCatridge();
    }
    public void ShakeScreen()
    {
        feedbackController.ShakeScreen();
    }
    public void PlayFiringSound()
    {
        feedbackController.PlayFiringSound();
    }
    public void DisplayOutOfAmmoFeedbacks()
    {
        feedbackController.DisplayOutOfAmmoFeedbacks();
    }
    public void UpdateMuzzleFlash()
    {
        feedbackController.UpdateMuzzleFlash();
    }
    public void DisplayReloadingFeedbacks()
    {
        feedbackController.DisplayReloadingFeedbacks();
    }
    public void DisplayStunEffect(float stunDuration)
    {
        feedbackController.DisplayStunEffect(stunDuration);
    }
    public void HighlightPlayer(Color color)
    {
        feedbackController.HighlightPlayer(color);
    }
    // UI controller
    public void IncrementKillstreak()
    {
        uiController.IncrementKillstreak();
    }
    #endregion

    // Private methods
    #region Private methods
    void Respawn()
    {
        transform.position = new Vector3(0, 0, 0);
    }
    #endregion

    // INHERITED METHODS
    #region Inherited methods
    private void Start()
    {
        // Get components
        movementController = GetComponent<PlayerMovementController>();
        firingController = GetComponent<PlayerFiringController>();
        orientationController = GetComponent<PlayerOrientation>();
        physicsHandler = GetComponent<PlayerPhysicsHandler>();
        inputHandler = GetComponent<PlayerInputHandler>();
        stunController = GetComponent<PlayerStunController>();
        feedbackController = GetComponent<PlayerFeedbackController>();
        uiController = GetComponent<PlayerUIController>();

        // If all components have been gotten, initialize them.
        if (movementController      == null ||
            firingController        == null ||
            orientationController   == null ||
            physicsHandler          == null ||
            inputHandler            == null ||
            stunController          == null)
        {
            Debug.LogError("PlayerManager not set up properly!");
        }
        else
        {
            movementController.PlayerManager = this;
            firingController.PlayerManager = this;
            orientationController.PlayerManager = this;
            physicsHandler.PlayerManager = this;
            inputHandler.PlayerManager = this;
            stunController.PlayerManager = this;
            feedbackController.PlayerManager = this;
            uiController.PlayerManager = this;
            movementController.Init();
            firingController.Init();
            orientationController.Init();
            physicsHandler.Init();
            inputHandler.Init();
            stunController.Init();
            feedbackController.Init();
            uiController.Init();
        }

        // Set up starting weapon and HP.
        firingController.CurrentWeapon = startingWeapon;
        Health = startingHealth;
    }
    private void Update()
    {
        if (_health < 1)
        {
            Kill();
        }
    }
    #endregion
}
