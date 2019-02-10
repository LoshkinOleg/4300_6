using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    - Links all player components together.
    - Handles player health, lives and loosing conditions.
    - Handles parachute toggling.
    - Provides other components with WeaponData.
*/

public class PlayerManager : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] bool _isLeftPlayer = true;

    // References
    PlayerMovementController _movementController = null;
    PlayerFiringController _firingController = null;
    PlayerAnimationAndOrientationController _animationAndOrientationController = null;
    PlayerPhysicsHandler _physicsHandler = null;
    PlayerInputHandler _inputHandler = null;
    PlayerStunController _stunController = null;
    PlayerUIController _uiController = null;
    [SerializeField] Weapon_Template[] _weaponsData = new Weapon_Template[(int)PlayerFiringController.Weapon.MINIGUN + 1];

    // Properties
    public PlayerMovementController movementController => _movementController;
    public PlayerFiringController firingController => _firingController;
    public PlayerAnimationAndOrientationController animationAndOrientationController => _animationAndOrientationController;
    public PlayerPhysicsHandler physicsHandler => _physicsHandler;
    public PlayerInputHandler inputHandler => _inputHandler;
    public PlayerStunController stunController => _stunController;
    public PlayerUIController uiController => _uiController;
    public Weapon_Template[] weaponsData => _weaponsData;
    public bool parachuteIsOpen { get; set; }
    public float health => _health;
    public bool isLeftPlayer => _isLeftPlayer;
    public int lives { get; set; } = 3;

    // Other components properties
    // Read only
    public float stunTimer => stunController.stunTimer;
    public float horizontalInput => inputHandler.horizontalInput;
    public float verticalInput => inputHandler.verticalInput;
    public float aimingHorizontalInput => inputHandler.aimingHorizontalInput;
    public float aimingVerticalInput => inputHandler.aimingVerticalInput;
    public bool tryingToOpenParachute => inputHandler.tryingToOpenParachute;
    public bool tryingToFire => inputHandler.tryingToFire;
    public GameObject armGO => animationAndOrientationController.armGO;
    // Writable
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

    // Private variables
    float _health = 1;
    #endregion

    // PUBLIC METHODS
    #region Public Methods
    public void ModifyHealth(float damage) // Use to damage (pass a negative number) or to heal (pass a positive number) the player.
    {
        _health += damage;
        uiController.UpdateHealthBar();
    }
    public void ProjectileHit(GameObject projectile, PlayerFiringController.Weapon type)
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
            case PlayerFiringController.Weapon.SNIPER:
                {
                    ModifyHealth(-weaponsData[2].damage);
                    physicsHandler.ProjectileHit(projectile, type);
                }
                break;
            case PlayerFiringController.Weapon.BAZOOKA:
                {
                    ModifyHealth(-weaponsData[3].damage);
                    physicsHandler.ProjectileHit(projectile, type);
                }
                break;
            case PlayerFiringController.Weapon.MINIGUN:
                {
                    ModifyHealth(-weaponsData[4].damage);
                    physicsHandler.ProjectileHit(projectile, type);
                }
                break;
        }
    } // Damages player and relays the message to the physics component for knockback.
    public void CrateBottomHit(BoxCollider2D collider)
    {
        // Give orders to stun and physics components
    }
    public void Kill()
    {
        lives--;
        if (lives < 1)
        {
            GameManager.instance.GameOver(gameObject);
        }
        else
        {
            _health = 1;
            uiController.UpdateHealthBar();
            transform.position = new Vector3(0, 0, 0);
        }
    }
    public void ToggleParachute()
    {
        physicsHandler.ToggleGravity();
        animationAndOrientationController.ToggleParachute();
        parachuteIsOpen = !parachuteIsOpen;
    }
    #endregion

    // INHERITED METHODS
    #region Inherited methods
    private void Start()
    {
        // Give other components a reference to the PlayerManager.
        movementController.playerManager = this;
        firingController.playerManager = this;
        animationAndOrientationController.playerManager = this;
        physicsHandler.playerManager = this;
        inputHandler.playerManager = this;
        stunController.playerManager = this;
        uiController.playerManager = this;
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
