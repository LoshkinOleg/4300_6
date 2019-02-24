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
    PlayerMovementController movementController = null;
    PlayerFiringController firingController = null;
    PlayerOrientation orientationController = null;
    PlayerPhysicsHandler physicsHandler = null;
    PlayerInputHandler inputHandler = null;
    PlayerStunController stunController = null;
    [SerializeField] WeaponData[] weaponsData = new WeaponData[(int)Weapon.MINIGUN + 1]; // 0: pistol, 1: shotgun, 2: sniper, 3: bazooka, 4: minigun
    public SpriteRenderer headSpriteRenderer = null;
    public SpriteRenderer parachuteSpriteRenderer = null;
    public SpriteRenderer weaponSpriteRenderer = null;
    public SpriteRenderer muzzleFlashSpriteRenderer = null;

    // Private variables
    float _health = 1;
    int _lives = 3;
    bool _parachuteIsOpen;
    #endregion

    // Public properties
    #region Public properties
    // PlayerManager's properties
    public WeaponData[] WeaponsData
    {
        get
        {
            if (weaponsData[0] != null)
            {
                return weaponsData;
            }
            else
            {
                Debug.Log("Variable is not set up!");
                return new WeaponData[0];
            }
        }
    }
    public bool IsLeftPlayer => _isLeftPlayer;
    public int Lives
    {
        get
        {
            return _lives;
        }
        set
        {
            if (FeedbackManager.Instance != null)           FeedbackManager.Instance.UpdateLives(gameObject, value);            else Debug.LogWarning("Variable not set up!");
            _lives = value;
        }
    }
    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            if (FeedbackManager.Instance != null)           FeedbackManager.Instance.UpdateHealth(gameObject, value);                  else Debug.LogWarning("Variable not set up!");
            _health = value;
        }
    }
    public bool ParachuteIsOpen => _parachuteIsOpen;
    // Movement controller
    public PlayerMovementController.MovementMode CurrentMovementMode
    {
        get
        {
            if (movementController != null)
            {
                return movementController.CurrentMovementMode;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return PlayerMovementController.MovementMode.AIRBORNE;
            }
        }
        set
        {
            if (movementController != null)
            {
                movementController.CurrentMovementMode = value;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
            }
        }
    }
    // Firing controller
    public Weapon CurrentWeapon
    {
        get
        {
            if (firingController != null)
            {
                return firingController.CurrentWeapon;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return Weapon.PISTOL;
            }
        }
        set
        {
            if (firingController != null)
            {
                firingController.CurrentWeapon = value;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
            }
        }
    }
    public int CurrentAmmo
    {
        get
        {
            if (firingController != null)
            {
                return firingController.CurrentAmmo;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return 10;
            }
        }
    }
    // Animation and Orientation controller
    public Transform ArmTransform
    {
        get
        {
            if (orientationController != null)
            {
                return orientationController.ArmTransform;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return gameObject.transform;
            }
        }
    }
    // Physics handler
    public float SpeedLimit
    {
        get
        {
            if (physicsHandler != null)
            {
                return physicsHandler.PlayerSpeedLimit;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return Mathf.Infinity;
            }
        }
    }
    public float Gravity
    {
        get
        {
            if (physicsHandler != null)
            {
                return physicsHandler.Gravity;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return 1;
            }
        }
        set
        {
            if (physicsHandler != null)
            {
                physicsHandler.Gravity = value;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
            }
        }
    }
    public Vector2 Velocity
    {
        get
        {
            if (physicsHandler != null)
            {
                return physicsHandler.Velocity;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return new Vector2();
            }
        }
        set
        {
            if (physicsHandler != null)
            {
                physicsHandler.Velocity = value;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
            }
        }
    }
    public float LinearDrag
    {
        get
        {
            if (physicsHandler != null)
            {
                return physicsHandler.LinearDrag;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return 0;
            }
        }
        set
        {
            if (physicsHandler != null)
            {
                physicsHandler.LinearDrag = value;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
            }
        }
    }
    // Input handler
    public InputDevice Gamepad
    {
        get
        {
            if (inputHandler != null)
            {
                return inputHandler.Gamepad;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return new InputDevice();
            }
        }
        set
        {
            if (inputHandler != null)
            {
                inputHandler.Gamepad = value;
            }
        }
    }
    public float HorizontalInput
    {
        get
        {
            if (inputHandler != null)
            {
                return inputHandler.HorizontalInput;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return 0;
            }
        }
    }
    public float VerticalInput
    {
        get
        {
            if (inputHandler != null)
            {
                return inputHandler.VerticalInput;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return 0;
            }
        }
    }
    public float AimingHorizontalInput
    {
        get
        {
            if (inputHandler != null)
            {
                return inputHandler.AimingHorizontalInput;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return 0;
            }
        }
    }
    public float AimingVerticalInput
    {
        get
        {
            if (inputHandler != null)
            {
                return inputHandler.AimingVerticalInput;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return 0;
            }
        }
    }
    public bool TryingToOpenParachute
    {
        get
        {
            if (inputHandler != null)
            {
                return inputHandler.TryingToOpenParachute;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return false;
            }
        }
    }
    public bool TryingToFire
    {
        get
        {
            if (inputHandler != null)
            {
                return inputHandler.TryingToFire;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return false;
            }
        }
    }
    // Stun controller
    public float StunTimer
    {
        get
        {
            if (stunController != null)
            {
                return stunController.StunTimer;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return 0;
            }
        }
    }
    public float StunForceMultiplier
    {
        get
        {
            if (stunController != null)
            {
                return stunController.StunForceMultiplier;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return 0;
            }
        }
    }
    public float StunOpportunityTimer
    {
        get
        {
            if (stunController != null)
            {
                return stunController.StunOpportunityTimer;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return 0;
            }
        }
        set
        {
            if (stunController != null)
            {
                stunController.StunOpportunityTimer = value;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
            }
        }
    }
    public float ProjectileHitStunWindow
    {
        get
        {
            if (stunController != null)
            {
                return stunController.ProjectileHitStunWindow;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
                return 1;
            }
        }
        set
        {
            if (stunController != null)
            {
                stunController.ProjectileHitStunWindow = value;
            }
            else
            {
                Debug.LogWarning("Variable is not set up!");
            }
        }
    }
    #endregion

    // PUBLIC METHODS
    #region Public Methods
    // PlayerManager's methods
    public void ModifyHealth(float damage) // Use to damage (pass a negative number) or to heal (pass a positive number) the player.
    {
        _health += damage;
        if (FeedbackManager.Instance != null)           FeedbackManager.Instance.UpdateHealth(gameObject, _health);          else Debug.LogWarning("Variable not set up!");
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
                    if (physicsHandler != null)             physicsHandler.ProjectileHit(projectile, type);             else Debug.LogWarning("Variable not set!");
                }
                break;
            case Weapon.SHOTGUN:
                {
                    ModifyHealth(-WeaponsData[1].damage);
                    if (physicsHandler != null)             physicsHandler.ProjectileHit(projectile, type);             else Debug.LogWarning("Variable not set!");
                }
                break;
            case Weapon.MINIGUN:
                {
                    ModifyHealth(-WeaponsData[4].damage);
                    if (physicsHandler != null)             physicsHandler.ProjectileHit(projectile, type);             else Debug.LogWarning("Variable not set!");
                }
                break;
            default:
                {
                    Debug.LogWarning("PlayerManager.cs: ProjectileHit() got passed a non valid projectile type: " + type);
                }
                break;
        }
        // Reset stun opportunity timer.
        if (FeedbackManager.Instance != null)               FeedbackManager.Instance.DisplayHit(gameObject);            else Debug.LogWarning("Variable not set!");
        StunOpportunityTimer = ProjectileHitStunWindow;
    }
    public void CrateBottomHit(BoxCollider2D crate)
    {
        if (ParachuteIsOpen)
        {
            ToggleParachute();
        }
        if (stunController != null)             stunController.Stun();                              else Debug.LogWarning("Variable not set!");
        if (physicsHandler != null)             physicsHandler.CrateBottomHit(crate);               else Debug.LogWarning("Variable not set!");
        if (FeedbackManager.Instance != null)   FeedbackManager.Instance.DisplayHit(gameObject);    else Debug.LogWarning("Variable not set!");
    }
    public void ExplosionHit(Vector3 position)
    {
        if (weaponsData[3] != null)             ModifyHealth(-WeaponsData[3].damage);               else Debug.LogWarning("Variable not set!");
        if (physicsHandler != null)             physicsHandler.ExplosionHit(position);              else Debug.LogWarning("Variable not set!");
        if (FeedbackManager.Instance != null)   FeedbackManager.Instance.DisplayHit(gameObject);    else Debug.LogWarning("Variable not set!");
        StunOpportunityTimer = ProjectileHitStunWindow;
    }
    public void SniperHit()
    {
        if (weaponsData[3] != null)             ModifyHealth(-WeaponsData[2].damage);               else Debug.LogWarning("Variable not set!");
        if (physicsHandler != null)             physicsHandler.SniperHit();                         else Debug.LogWarning("Variable not set!");
        if (FeedbackManager.Instance != null)   FeedbackManager.Instance.DisplayHit(gameObject);    else Debug.LogWarning("Variable not set!");
        StunOpportunityTimer = ProjectileHitStunWindow;
    }
    public void Kill()
    {
        _lives--;
        if (Lives < 1)
        {
            if (GameManager.Instance != null)   GameManager.Instance.GameOver(gameObject);      else Debug.LogWarning("Variable not set!");
        }
        else
        {
            _health = 1;
            if (FeedbackManager.Instance != null)           FeedbackManager.Instance.UpdateHealth(gameObject, _health);          else Debug.LogWarning("Variable not set up!");
            if (FeedbackManager.Instance != null)           FeedbackManager.Instance.UpdateLives(gameObject, Lives);    else Debug.LogWarning("Variable not set up!");
            transform.position = new Vector3(0, 0, 0);
        }
    }
    public void ToggleParachute()
    {
        if (physicsHandler != null)             physicsHandler.ToggleGravity();                         else Debug.LogWarning("Variable not set!");
        if (FeedbackManager.Instance != null)   FeedbackManager.Instance.ToggleParachute(this);         else Debug.LogWarning("Variable not set up!");
        _parachuteIsOpen = !_parachuteIsOpen;
    }
    // Firing controller
    public void SpeedBulletsUp()
    {
        if (firingController != null)           firingController.SpeedBulletsUp();                      else Debug.LogWarning("Variable not set up!");
    }
    // Physics handler
    public void ToggleGravity()
    {
        if (physicsHandler != null)             physicsHandler.ToggleGravity();                         else Debug.LogWarning("Variable not set!");
    }
    public void ResetGravity()
    {
        if (physicsHandler != null)             physicsHandler.ResetGravity();                          else Debug.LogWarning("Variable not set!");
    }
    public bool IsTouching(Collider2D collider)
    {
        if (physicsHandler != null)             return physicsHandler.IsTouching(collider);             else { Debug.LogWarning("Variable not set!"); return false; }
    }
    public void ApplyFiringKnockback(Vector2 unitaryDirection, float magnitude)
    {
        if (physicsHandler != null)             physicsHandler.AddForce(unitaryDirection, magnitude);   else Debug.LogWarning("Variable not set!");
    }
    public void AddForce(Vector2 unitaryDirection, float magnitude)
    {
        if (physicsHandler != null)             physicsHandler.AddForce(unitaryDirection, magnitude);   else Debug.LogWarning("Variable not set!");
    }
    // Stun controller
    public void Stun()
    {
        if (stunController != null)             stunController.Stun();                                  else Debug.LogWarning("Variable not set!");
    }
    #endregion

    // INHERITED METHODS
    #region Inherited methods
    private void Start()
    {
        movementController = GetComponent<PlayerMovementController>();
        movementController.PlayerManager = this;
        movementController.Init();

        firingController = GetComponent<PlayerFiringController>();
        firingController.PlayerManager = this;
        firingController.Init();

        orientationController = GetComponent<PlayerOrientation>();
        orientationController.PlayerManager = this;
        orientationController.Init();

        physicsHandler = GetComponent<PlayerPhysicsHandler>();
        physicsHandler.PlayerManager = this;
        physicsHandler.Init();

        inputHandler = GetComponent<PlayerInputHandler>();
        inputHandler.PlayerManager = this;
        inputHandler.Init();

        stunController = GetComponent<PlayerStunController>();
        stunController.PlayerManager = this;
        stunController.Init();

        CurrentWeapon = startingWeapon;
    }
    private void Update()
    {
        // Handle losing condition
        if (Health <= 0)
        {
            Kill();
        }
    }
    #endregion
}
