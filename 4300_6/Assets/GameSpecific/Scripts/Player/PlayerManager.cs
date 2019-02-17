using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

/*
    - Links all player components together.
    - Provides other components with it's variables.
    - Handles player health, lives and loosing conditions.
    - Handles hits from projectiles.
    - Handles parachute toggling.
    - Relays function calls to other components.
*/

public class PlayerManager : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] bool _isLeftPlayer = true;

    // References
    PlayerMovementController movementController = null;
    PlayerFiringController firingController = null;
    PlayerAnimationAndOrientationController animationAndOrientationController = null;
    PlayerPhysicsHandler physicsHandler = null;
    PlayerInputHandler inputHandler = null;
    PlayerStunController stunController = null;
    PlayerUIController uiController = null;
    [SerializeField] Weapon[] _weaponsData = new Weapon[(int)PlayerFiringController.Weapon.MINIGUN + 1]; // 0: pistol, 1: shotgun, 2: sniper, 3: bazooka, 4: minigun

    // Private variables
    float _health = 1;
    int _lives = 3;
    bool _parachuteIsOpen;
    #endregion

    // Public properties
    #region Public properties
    // PlayerManager's properties
    public Weapon[] weaponsData => _weaponsData;
    public bool isLeftPlayer => _isLeftPlayer;
    public int lives => _lives;
    public float health => _health;
    public bool parachuteIsOpen => _parachuteIsOpen;
    // Movement controller
    public PlayerMovementController.MovementMode currentMovementMode
    {
        get
        {
            return movementController.currentMovementMode;
        }
        set
        {
            movementController.currentMovementMode = value;
        }
    }
    // Firing controller
    public PlayerFiringController.Weapon currentWeapon
    {
        get
        {
            return firingController.currentWeapon;
        }
        set
        {
            firingController.currentWeapon = value;
        }
    }
    // Animation and Orientation controller
    public GameObject armGO => animationAndOrientationController.armGO;
    // Physics handler
    public float speedLimit => physicsHandler.playerSpeedLimit;
    public float gravity
    {
        get
        {
            return physicsHandler.gravity;
        }
        set
        {
            physicsHandler.gravity = value;
        }
    }
    public Vector2 velocity
    {
        get
        {
            return physicsHandler.velocity;
        }
        set
        {
            physicsHandler.velocity = value;
        }
    }
    public float linearDrag
    {
        get
        {
            return physicsHandler.linearDrag;
        }
        set
        {
            physicsHandler.linearDrag = value;
        }
    }
    // Input handler
    public InputDevice gamepad
    {
        get
        {
            return inputHandler.gamepad;
        }
        set
        {
            inputHandler.gamepad = value;
        }
    }
    public float horizontalInput => inputHandler.horizontalInput;
    public float verticalInput => inputHandler.verticalInput;
    public float aimingHorizontalInput => inputHandler.aimingHorizontalInput;
    public float aimingVerticalInput => inputHandler.aimingVerticalInput;
    public bool tryingToOpenParachute => inputHandler.tryingToOpenParachute;
    public bool tryingToFire => inputHandler.tryingToFire;
    // Stun controller
    public float stunTimer => stunController.stunTimer;
    public float stunForceMultiplier => stunController.stunForceMultiplier;
    public float stunOpportunityTimer
    {
        get
        {
            return stunController.stunOpportunityTimer;
        }
        set
        {
            stunController.stunOpportunityTimer = value;
        }
    }
    public float projectileHitStunWindow
    {
        get
        {
            return stunController.projectileHitStunWindow;
        }
        set
        {
            stunController.projectileHitStunWindow = value;
        }
    }
    #endregion

    // PUBLIC METHODS
    #region Public Methods
    // PlayerManager's methods
    public void ModifyHealth(float damage) // Use to damage (pass a negative number) or to heal (pass a positive number) the player.
    {
        _health += damage;
        uiController.UpdateHealthBar();
    }
    public void ModifyLives(int life)
    {
        _lives += life;
        UpdateLives();
    }
    public void ProjectileHit(GameObject projectile, PlayerFiringController.Weapon type) // Damages player and relays the message to the physics component for knockback.
    {
        switch (type)
        {
            case PlayerFiringController.Weapon.PISTOL:
                {
                    ModifyHealth(-weaponsData[0].damage);
                    physicsHandler.ProjectileHit(projectile, type);
                }
                break;
            case PlayerFiringController.Weapon.SHOTGUN:
                {
                    ModifyHealth(-weaponsData[1].damage);
                    physicsHandler.ProjectileHit(projectile, type);
                }
                break;
            case PlayerFiringController.Weapon.MINIGUN:
                {
                    ModifyHealth(-weaponsData[4].damage);
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
        stunOpportunityTimer = projectileHitStunWindow;
    }
    public void CrateBottomHit(BoxCollider2D crate)
    {
        if (parachuteIsOpen)
        {
            ToggleParachute();
        }
        stunController.Stun();
        physicsHandler.CrateBottomHit(crate);
    }
    public void ExplosionHit(Vector3 position)
    {
        ModifyHealth(-weaponsData[3].damage);
        physicsHandler.ExplosionHit(position);
        stunOpportunityTimer = projectileHitStunWindow;
    }
    public void SniperHit()
    {
        ModifyHealth(-weaponsData[2].damage);
        stunOpportunityTimer = projectileHitStunWindow;
    }
    public void Kill()
    {
        _lives--;
        if (lives < 1)
        {
            GameManager.Instance.GameOver(gameObject);
        }
        else
        {
            _health = 1;
            uiController.UpdateHealthBar();
            uiController.UpdateLives();
            transform.position = new Vector3(0, 0, 0);
        }
    }
    public void ToggleParachute()
    {
        physicsHandler.ToggleGravity();
        animationAndOrientationController.ToggleParachute();
        _parachuteIsOpen = !_parachuteIsOpen;
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
    // UI controller
    public void UpdateHealthBar()
    {
        uiController.UpdateHealthBar();
    }
    public void UpdateLives()
    {
        uiController.UpdateLives();
    }
    #endregion

    // INHERITED METHODS
    #region Inherited methods
    private void Start()
    {
        if ((movementController = GetComponent<PlayerMovementController>()) == null)
        {
            Debug.LogError("PlayerManager.cs: player component not found.");
        }
        movementController.playerManager = this;
        movementController.Init();

        if ((firingController = GetComponent<PlayerFiringController>()) == null)
        {
            Debug.LogError("PlayerManager.cs: player component not found.");
        }
        firingController.playerManager = this;
        firingController.Init();

        if ((animationAndOrientationController = GetComponent<PlayerAnimationAndOrientationController>()) == null)
        {
            Debug.LogError("PlayerManager.cs: player component not found.");
        }
        animationAndOrientationController.playerManager = this;
        animationAndOrientationController.Init();

        if ((physicsHandler = GetComponent<PlayerPhysicsHandler>()) == null)
        {
            Debug.LogError("PlayerManager.cs: player component not found.");
        }
        physicsHandler.playerManager = this;
        physicsHandler.Init();

        if ((inputHandler = GetComponent<PlayerInputHandler>()) == null)
        {
            Debug.LogError("PlayerManager.cs: player component not found.");
        }
        inputHandler.playerManager = this;
        inputHandler.Init();

        if ((stunController = GetComponent<PlayerStunController>()) == null)
        {
            Debug.LogError("PlayerManager.cs: player component not found.");
        }
        stunController.playerManager = this;
        stunController.Init();

        if ((uiController = GetComponent<PlayerUIController>()) == null)
        {
            Debug.LogError("PlayerManager.cs: player component not found.");
        }
        uiController.playerManager = this;
        uiController.Init();
    }
    private void Update()
    {
        // Handle losing condition
        if (health <= 0)
        {
            Kill();
        }
    }
    #endregion
}
