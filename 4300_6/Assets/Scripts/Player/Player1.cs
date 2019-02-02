using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player1 : MonoBehaviour
{
    // Classes and Enums
    #region Classes and Enums
    public enum MovementMode
    {
        AIRBORNE,
        GROUND,
        JETPACK
    }
    #endregion

    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] float dragForce = 0.1f;
    [SerializeField] float _groundHorizontalVelocity = 7;
    [SerializeField] float _airborneHorizontalForce = 7;
    [SerializeField] float _firingFrequency = 4; // 4 projectiles / second.
    [SerializeField] float _playerSpeedLimit = 5;
    [SerializeField] float dodgeDuration = 0.2f;
    [SerializeField] float dodgeSpeedupMagnitude = 1;

    // References
    [SerializeField] GameObject bulletPrefab = null;
    [SerializeField] GameObject healthImageGO = null;
    [SerializeField] GameObject parachuteGO = null;
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
    Vector2 dodgeDirection = new Vector2();
    bool dodging = false;
    float jetpackTimer;
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
    public void EnterJetpackMode()
    {
        _currentMovementMode = MovementMode.JETPACK;
        jetpackTimer = PickupManager.instance.jetpackDuration;
    }
    #endregion

    // PRIVATE METHODS
    #region Private methods
    float CalculateNorm(Vector2 vector)
    {
        return Mathf.Sqrt(Mathf.Pow(vector.x,2)+Mathf.Pow(vector.y,2));
    }
    void InstantiateBullet()
    {
        Instantiate(bulletPrefab, transform.position, new Quaternion());
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
                    if (dodgeTimer < 0)
                    {
                        // Reset dodge direction
                        dodgeDirection = new Vector2();
                        dodging = false;
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

                    if (Input.GetButtonDown("Player1_Parachute"))
                    {
                        playerRigidbody.gravityScale = -playerRigidbody.gravityScale;
                        parachuteGO.SetActive(!parachuteGO.activeSelf);
                        parachuteIsOpen = !parachuteIsOpen;
                        dodgeTimer = dodgeDuration;
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
    #endregion

    // INHERITED METHODS
    #region Inherited methods
    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        healthImage = healthImageGO.GetComponent<Image>();
        fireCooldown = 1 / firingFrequency;
        currentBulletsSpeed = GameManager.instance.defaultBulletSpeed;
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
        ApplySpeedLimit(); // Comes before HandleFiringAndMovement() because of the dodge mechanic that said function applies.
        Move();
        ApplyDrag();
    }

    private void Update()
    {
        // Handle inputs
        horizontalInput = Input.GetAxisRaw("Player1_Horizontal");
        verticalInput = Input.GetAxisRaw("Player1_Vertical");

        if (Input.GetButtonDown("Player1_Parachute")) // Handle parachute input
        {
            playerRigidbody.gravityScale = -playerRigidbody.gravityScale;
            parachuteGO.SetActive(!parachuteGO.activeSelf);
            parachuteIsOpen = !parachuteIsOpen;
            dodgeTimer = dodgeDuration;
        }
        if (Input.GetButtonDown("Player1_Fire")) // Handle firing input
        {
            if (fireCooldown < 0)
            {
                Instantiate(bulletPrefab, transform.position, new Quaternion()).GetComponent<Projectile>().speed = currentBulletsSpeed;
                fireCooldown = 1 / _firingFrequency;
            }
        }

        // Handle losing condition
        if (health <= 0)
        {
            GameManager.instance.GameOver(gameObject);
        }

        // Update timers
        fireCooldown -= Time.deltaTime;
        dodgeTimer -= Time.deltaTime;
        speedupTimer -= Time.deltaTime;
        jetpackTimer -= Time.deltaTime;
    }
    #endregion

}
