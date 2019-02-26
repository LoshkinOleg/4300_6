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
    int _health = 3;
    int healthBars = 10;
    int _lives = 3;
    bool _parachuteIsOpen;
    #endregion

    // Public properties
    #region Public properties
    // PlayerManager's properties
    public WeaponData[] WeaponsData => _weaponsData;
    public bool IsLeftPlayer => _isLeftPlayer;
    public bool ParachuteIsOpen => _parachuteIsOpen;
    public int Lives => _lives;
    public int Health => _health;
    public Transform FrontArmTransform => _frontArmTransform;
    public Transform BackArmTransform => _backArmTransform;
    public Transform FiringAndReloadingFX_Transform => _firingAndReloadingFX_Transform;
    public SpriteRenderer Parachute_SpriteRenderer => _parachute_SpriteRenderer;
    public SpriteRenderer FrontArm_SpriteRenderer => _frontArm_SpriteRenderer;
    public SpriteRenderer BackArm_SpriteRenderer => _backArm_SpriteRenderer;
    public SpriteRenderer FiringAndReloadingFX_SpriteRenderer => _firingAndReloadingFX_SpriteRenderer;
    // Movement controller
    public PlayerMovementController.MovementMode CurrentMovementMode => movementController.CurrentMovementMode;
    // Firing controller
    public Weapon CurrentWeapon => firingController.CurrentWeapon;
    public int CurrentAmmo => firingController.CurrentAmmo;
    public bool HasRecentlyShot => firingController.HasRecentlyShot;
    // Animation and Orientation controller
    public Transform ArmsTransform => orientationController.ArmsTransform;
    // Physics handler
    public float SpeedLimit => physicsHandler.PlayerSpeedLimit;
    public float Gravity => physicsHandler.Gravity;
    public Vector2 Velocity => physicsHandler.Velocity;
    public float LinearDrag => physicsHandler.LinearDrag;
    // Input handler
    public InputDevice Gamepad => inputHandler.Gamepad;
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
    #endregion

    // PUBLIC METHODS
    #region Public Methods
    // PlayerManager's methods
    public void ModifyHealth(int damage) // Use to damage (pass a negative number) or to heal (pass a positive number) the player.
    {
        _health += damage;

        if (_health < 1)
        {
            DestroyHealthBar();
        }
        else if (_health > 3)
        {
            AddAHealthBar();
        }

        uiController.UpdateHealth();
    }
    public void ModifyLives(int life)
    {
        _lives += life;
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
        _lives--;
        if (healthBars < 1)
        {
            GameManager.Instance.GameOver(gameObject);
        }
        else
        {
            _health = 1;
            uiController.UpdateHealth();
            uiController.UpdateLives();
            Respawn();
        }
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
    public void SwitchToJetpack()
    {
        movementController.CurrentMovementMode = PlayerMovementController.MovementMode.JETPACK;
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
    #endregion

    // Private methods
    #region Private methods
    void DestroyHealthBar()
    {
        healthBars--;
        _health = 3;
        feedbackController.HighlightHealthBar();
    }
    void AddAHealthBar()
    {
        healthBars++;
        _health = 3;
        feedbackController.HighlightHealthBar();
    }
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
        if (movementController == null ||
            firingController == null ||
            orientationController == null ||
            physicsHandler == null ||
            inputHandler == null ||
            stunController == null)
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

        // Set up starting weapon.
        firingController.CurrentWeapon = startingWeapon;
    }
    private void Update()
    {
        // Handle losing condition
        if (healthBars < 1)
        {
            Kill();
        }
    }
    #endregion
}
