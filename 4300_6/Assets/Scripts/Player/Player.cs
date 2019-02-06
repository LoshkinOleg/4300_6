﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Classes and Enums
    #region Classes and Enums
    public enum MovementMode
    {
        AIRBORNE,
        GROUND,
        JETPACK
    }
    public enum PlayerDirection
    {
        LEFT,
        RIGHT
    }
    public enum GunDirection
    {
        FORWARD,
        UP,
        DOWN,
        ANYWHERE
    }
    #endregion

    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] bool isLeftPlayer = true;
    [SerializeField] float dragForce = 0.1f;
    [SerializeField] float _groundHorizontalVelocity = 3;
    [SerializeField] float _airborneHorizontalForce = 20;
    [SerializeField] float _firingFrequency = 4; // 4 projectiles / second.
    [SerializeField] float _playerSpeedLimit = 5;
    [SerializeField] float dodgeDuration = 0.05f;
    [SerializeField] float dodgeSpeedupMagnitude = 10;
    [SerializeField] float stunForceMultiplier = 10;
    [SerializeField] float stunDuration = 0.5f;

    // References
    [SerializeField] GameObject bulletPrefab = null;
    [SerializeField] GameObject healthImageGO = null;
    [SerializeField] GameObject parachuteGO = null;
    [SerializeField] GameObject gunGO = null;
    BoxCollider2D playerCollider = null;
    Rigidbody2D playerRigidbody = null;
    Image healthImage = null;

    // Properties
    public float groundHorizontalVelocity
    {
        get
        {
            return _groundHorizontalVelocity;
        }
        set
        {
            _groundHorizontalVelocity = value;
        }
    }
    public float airborneHorizontalForce
    {
        get
        {
            return _airborneHorizontalForce;
        }
        set
        {
            _airborneHorizontalForce = value;
        }
    }
    public float firingFrequency
    {
        get
        {
            return _firingFrequency;
        }
        set
        {
            _firingFrequency = value;
        }
    }
    public float playerSpeedLimit
    {
        get
        {
            return _playerSpeedLimit;
        }
        set
        {
            _playerSpeedLimit = value;
        }
    }
    public MovementMode currentMovementMode
    {
        get
        {
            return _currentMovementMode;
        }
        set
        {
            _currentMovementMode = value;
        }
    }
    public int lives
    {
        get
        {
            return _lives;
        }
        set
        {
            _lives = value;
        }
    }
    public float health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
        }
    }

    // Private variables
    MovementMode _currentMovementMode = MovementMode.AIRBORNE;
    int _lives = 3;
    float _health = 1;
    float currentBulletsSpeed;
    bool parachuteIsOpen = false;
    float fireCooldown;
    float dodgeTimer;
    float speedupTimer;
    bool firing = false;
    float horizontalInput = 0;
    float verticalInput = 0;
    float aimingHorizontalInput = 0;
    float aimingVerticalInput = 0;
    Vector2 dodgeDirection = new Vector2();
    bool dodging = false;
    float jetpackTimer;
    float stunTimer;
    PlayerDirection currentPlayerDirection;
    GunDirection currentGunDirection = GunDirection.FORWARD;
    #endregion

    // PUBLIC METHODS
    #region Public Methods
    public void ModifyHealth(float damage) // Use to damage (pass a negative number) or to heal (pass a positive number) the player.
    {
        health += damage;
        healthImage.fillAmount = health;
    }
    public void SpeedBulletsUp()
    {
        currentBulletsSpeed = PickupManager.instance.speedupBulletSpeed;
        speedupTimer = PickupManager.instance.speedupPickupTime;
    }
    public void SetMovementMode(MovementMode mode)
    {
        switch (mode)
        {
            case MovementMode.AIRBORNE:
                {
                    if (currentMovementMode == MovementMode.JETPACK)
                    {
                        playerRigidbody.gravityScale = 1;
                    }
                }
                break;
            case MovementMode.GROUND:
                {
                    
                }
                break;
            case MovementMode.JETPACK:
                {
                    playerRigidbody.gravityScale = 0;
                    jetpackTimer = PickupManager.instance.jetpackDuration;
                }
                break;
        }
        currentMovementMode = mode;
    }
    public void Stun()
    {
        ToggleParachute();
        if (playerCollider.IsTouching(GameManager.instance.bottomBoundsCollider))
        {
            Kill();
        }
        else
        {
            playerRigidbody.AddForce(Vector2.down * stunForceMultiplier);
            stunTimer = stunDuration;
        }
    }
    public void Kill()
    {
        _lives--;
        if (lives < 1)
        {
            GameManager.instance.GameOver(gameObject);
        }
    }
    #endregion

    // PRIVATE METHODS
    #region Private methods
    float CalculateNorm(Vector2 vector)
    {
        return Mathf.Sqrt(Mathf.Pow(vector.x,2)+Mathf.Pow(vector.y,2));
    }
    void ApplyDrag()
    {
        if (Input.GetAxisRaw("Player1_Horizontal") == 0)
        {
            if (Mathf.Abs(playerRigidbody.velocity.x) > 0.05f) // Applies drag if the horizontal speed is greater than 0.05f
            {
                playerRigidbody.velocity += new Vector2(-Mathf.Sign(playerRigidbody.velocity.x) * dragForce, 0);
            }
            else // Stops all horizontal movement if the speed if smaller than 0.05f
            {
                playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
            }
        }
    }
    bool CheckEnemyDirection() // Returns true if the enemy is on the right or at the same x axis, false if he is on the left.
    {
        if (isLeftPlayer)
        {
            if ((GameManager.instance.player2.gameObject.transform.position - transform.position).x >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if ((GameManager.instance.player1.gameObject.transform.position - transform.position).x >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    void AnalyseInputsForDirection()
    {
        if (currentMovementMode != MovementMode.GROUND)
        {
            // Handle player sprite orientation.
            if (horizontalInput != 0)
            {
                if (horizontalInput > 0)
                {
                    // H+
                    currentPlayerDirection = PlayerDirection.RIGHT;
                }
                else
                {
                    // H-
                    currentPlayerDirection = PlayerDirection.LEFT;
                }
            }
            else
            {
                // H = 0
                if (CheckEnemyDirection())
                {
                    currentPlayerDirection = PlayerDirection.RIGHT;
                }
                else
                {
                    currentPlayerDirection = PlayerDirection.LEFT;
                }
            }

            //Handle gun orientation
                // vertcial is biggest
            if (aimingVerticalInput > 0)
            {
                // V+
                currentGunDirection = GunDirection.DOWN;
            }
            else if (aimingVerticalInput < 0)
            {
                // V-
                currentGunDirection = GunDirection.UP;
            }
            else
            {
                currentGunDirection = GunDirection.FORWARD;
            }
        }
        else
        {
            // Handle ground mode inputs
            // Handle player sprite orientation.
            if (horizontalInput != 0)
            {
                if (horizontalInput > 0)
                {
                    // H+
                    currentPlayerDirection = PlayerDirection.RIGHT;
                }
                else
                {
                    // H-
                    currentPlayerDirection = PlayerDirection.LEFT;
                }
            }
            else
            {
                // H = 0
                if (CheckEnemyDirection())
                {
                    currentPlayerDirection = PlayerDirection.RIGHT;
                }
                else
                {
                    currentPlayerDirection = PlayerDirection.LEFT;
                }
            }
            currentGunDirection = GunDirection.ANYWHERE;
        }
    }
    void OrientSpriteAndGun()
    {
        AnalyseInputsForDirection();

        if (currentMovementMode != MovementMode.GROUND)
        {
            switch (currentPlayerDirection)
            {
                case PlayerDirection.LEFT:
                    {
                        transform.eulerAngles = new Vector3(0, 180, 0);

                        switch (currentGunDirection)
                        {
                            case GunDirection.FORWARD:
                                {
                                    gunGO.transform.localEulerAngles = new Vector3(0, 0, 0);
                                }
                                break;
                            case GunDirection.UP:
                                {
                                    gunGO.transform.localEulerAngles = new Vector3(0, 0, -90);
                                }
                                break;
                            case GunDirection.DOWN:
                                {
                                    gunGO.transform.localEulerAngles = new Vector3(0, 0, 90);
                                }
                                break;
                            case GunDirection.ANYWHERE:
                                {
                                    gunGO.transform.localEulerAngles = new Vector3(0, 180, Mathf.Atan2(aimingVerticalInput, aimingHorizontalInput) * Mathf.Rad2Deg);
                                }
                                break;
                        }
                    }
                    break;
                case PlayerDirection.RIGHT:
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);

                        switch (currentGunDirection)
                        {
                            case GunDirection.FORWARD:
                                {
                                    gunGO.transform.localEulerAngles = new Vector3(0, 0, 0);
                                }
                                break;
                            case GunDirection.UP:
                                {
                                    gunGO.transform.localEulerAngles = new Vector3(0, 0, -90);
                                }
                                break;
                            case GunDirection.DOWN:
                                {
                                    gunGO.transform.localEulerAngles = new Vector3(0, 0, 90);
                                }
                                break;
                            case GunDirection.ANYWHERE:
                                {
                                    gunGO.transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(aimingVerticalInput, aimingHorizontalInput) * Mathf.Rad2Deg);
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        else // If it is GROUND
        {
            if (aimingHorizontalInput > 0)
            {
                transform.localEulerAngles = new Vector3(0,0,0);
                gunGO.transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(aimingVerticalInput, aimingHorizontalInput) * Mathf.Rad2Deg);
            }
            else if (aimingHorizontalInput < 0)
            {
                transform.localEulerAngles = new Vector3(0, 180, 0);
                gunGO.transform.localEulerAngles = new Vector3(180, 180, -Mathf.Atan2(aimingVerticalInput, aimingHorizontalInput) * Mathf.Rad2Deg);
            }
            else
            {
                if (CheckEnemyDirection())
                {
                    // enemy on the right
                    transform.localEulerAngles = new Vector3(0, 0, 0);
                    gunGO.transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(aimingVerticalInput, aimingHorizontalInput) * Mathf.Rad2Deg);
                }
                else
                {
                    // enemy on the left
                    transform.localEulerAngles = new Vector3(0, 180, 0);
                }
            }
        }
    }
    void Move()
    {
        switch (_currentMovementMode)
        {
            case MovementMode.AIRBORNE:
                {
                    // Handle horizontal input
                    if (horizontalInput != 0)
                    {
                        playerRigidbody.AddForce(Vector2.right * horizontalInput * _airborneHorizontalForce);
                    }

                    if (dodgeTimer > 0 && !dodging) // Accelerates player up or down as long as the timer is greater than 0.
                    {
                        if (playerRigidbody.velocity.y > 0)
                        {
                            // Dodge down since we just closed the parachute
                            dodgeDirection = Vector2.down;
                        }
                        else
                        {
                            // Dodge up since we just opened the parachute
                            dodgeDirection = Vector2.up;
                        }
                        dodging = true;
                    }
                    if (dodging)
                    {
                        // Normalize player velocity and add a fixed speed boost.
                        playerRigidbody.velocity = (Vector2)Vector3.Normalize(playerRigidbody.velocity) + dodgeDirection * dodgeSpeedupMagnitude;
                    }
                }
                break;
            case MovementMode.GROUND:
                {
                    // Controlls horizontal movement precisely by affecting velocity.
                    float horizontalInput = Input.GetAxisRaw("Player1_Horizontal");
                    if (horizontalInput != 0)
                    {
                        playerRigidbody.velocity = new Vector2(_groundHorizontalVelocity * horizontalInput, playerRigidbody.velocity.y);
                    }
                    else
                    {
                        playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
                    }

                    if (dodgeTimer > 0)
                    {
                        playerRigidbody.velocity = ((Vector2)Vector3.Normalize(playerRigidbody.velocity) + Vector2.up * dodgeSpeedupMagnitude) * -Mathf.Sign(playerRigidbody.velocity.x);
                    }
                }
                break;
            case MovementMode.JETPACK:
                {
                    if (jetpackTimer > 0)
                    {
                        // Controls all movement precisely by affecting velocity.
                        playerRigidbody.velocity = Vector3.Normalize(new Vector2(horizontalInput, verticalInput)) * PickupManager.instance.jetpackVelocity;
                    }
                    else
                    {
                        _currentMovementMode = MovementMode.AIRBORNE;
                    }
                }
                break;
        }
    }
    void Shoot()
    {
        if (firing)
        {
            if (fireCooldown < 0)
            {
                Instantiate(bulletPrefab, transform.position, gunGO.transform.rotation).GetComponent<Projectile>().speed = currentBulletsSpeed;
                fireCooldown = 1 / _firingFrequency;
                firing = false;
            }
        }
    }
    void ApplySpeedLimit()
    {
        Vector2 velocityV2 = playerRigidbody.velocity;
        float velocityFloat = CalculateNorm(velocityV2);
        float slope = velocityV2.y / velocityV2.x;

        if (velocityFloat > _playerSpeedLimit)
        {
            playerRigidbody.velocity = (Vector2)Vector3.Normalize(velocityV2) * _playerSpeedLimit;
        }
    }
    void ToggleParachute()
    {
        playerRigidbody.gravityScale = -playerRigidbody.gravityScale;
        parachuteGO.SetActive(!parachuteGO.activeSelf);
        parachuteIsOpen = !parachuteIsOpen;
        dodgeTimer = dodgeDuration;
    }
    #endregion

    // INHERITED METHODS
    #region Inherited methods
    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        healthImage = healthImageGO.GetComponent<Image>();
        fireCooldown = 1 / firingFrequency;
        currentBulletsSpeed = GameManager.instance.defaultBulletSpeed;
        SetMovementMode(MovementMode.AIRBORNE);

        if (isLeftPlayer)
        {
            currentPlayerDirection = PlayerDirection.RIGHT;
        }
        else
        {
            currentPlayerDirection = PlayerDirection.LEFT;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            ModifyHealth(-0.1f);
        }
    }

    private void FixedUpdate()
    {
        ApplySpeedLimit();
        OrientSpriteAndGun();
        Move();
        ApplyDrag();
        Shoot();
    }

    private void Update()
    {
        // Handle inputs
        if (stunTimer < 0)
        {
            if (isLeftPlayer)
            {
                horizontalInput = Input.GetAxisRaw("Player1_Horizontal");
                verticalInput = Input.GetAxisRaw("Player1_Vertical");
                aimingHorizontalInput = Input.GetAxisRaw("Player1_Aiming_Horizontal");
                aimingVerticalInput = Input.GetAxisRaw("Player1_Aiming_Vertical");

                switch (currentMovementMode)
                {
                    case MovementMode.AIRBORNE:
                        {
                            if (Input.GetButtonDown("Player1_Parachute")) // Handle parachute input
                            {
                                ToggleParachute();
                            }
                        }
                        break;
                    case MovementMode.GROUND:
                        {
                            if (Input.GetButtonDown("Player1_Parachute")) // Handle parachute input
                            {
                                ToggleParachute();
                            }
                        }
                        break;
                }

                if (Input.GetButtonDown("Player1_Fire")) // Handle firing input
                {
                    firing = true;
                }
            }
            else
            {
                horizontalInput = Input.GetAxisRaw("Player2_Horizontal");
                verticalInput = Input.GetAxisRaw("Player2_Vertical");
                aimingHorizontalInput = Input.GetAxisRaw("Player2_Aiming_Horizontal");
                aimingVerticalInput = Input.GetAxisRaw("Player2_Aiming_Vertical");

                switch (currentMovementMode)
                {
                    case MovementMode.AIRBORNE:
                        {
                            if (Input.GetButtonDown("Player2_Parachute")) // Handle parachute input
                            {
                                playerRigidbody.gravityScale = -playerRigidbody.gravityScale;
                                parachuteGO.SetActive(!parachuteGO.activeSelf);
                                parachuteIsOpen = !parachuteIsOpen;
                                dodgeTimer = dodgeDuration;
                            }
                        }
                        break;
                    case MovementMode.GROUND:
                        {
                            if (Input.GetButtonDown("Player2_Parachute")) // Handle parachute input
                            {
                                playerRigidbody.gravityScale = -playerRigidbody.gravityScale;
                                parachuteGO.SetActive(!parachuteGO.activeSelf);
                                parachuteIsOpen = !parachuteIsOpen;
                                dodgeTimer = dodgeDuration;
                            }
                        }
                        break;
                }

                if (Input.GetButtonDown("Player2_Fire")) // Handle firing input
                {
                    firing = true;
                }
            }
        }
        
        // Handle losing condition
        if (health <= 0)
        {
            Kill();
        }

        // Update timers
        fireCooldown -= Time.deltaTime;
        dodgeTimer -= Time.deltaTime;
        speedupTimer -= Time.deltaTime;
        jetpackTimer -= Time.deltaTime;
        if (dodgeTimer < 0)
        {
            // Reset dodge direction
            dodgeDirection = new Vector2();
            dodging = false;
        }
    }
    #endregion

}
