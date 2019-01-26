using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    The class can either be set up as a left or right player.
    Player's movement is either velocity based when outside of the buffer zones or force based when inside the buffer zones.
*/

public class PlayerController : MonoBehaviour
{
    // Classes and Enums
    #region Classes and Enums
    private enum HorizontalBuffer
    {
        NONE,
        LEFT,
        RIGHT
    }
    private enum VerticalBuffer
    {
        NONE,
        TOP,
        BOTTOM
    }
    #endregion

    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] bool isLeftPlayer = true;
    [SerializeField] float dragForce = 0.1f;
    [SerializeField] float movementVelocity = 7;
    [SerializeField] float movementForce = 1;
    [SerializeField] float screenEdgeBufferDistance = 1.5f;
    [SerializeField] float screenEdgeBufferForce = 25f;
    [SerializeField] float firingFrequency = 0.25f; // Smaller is quicker. 0.25 => 4 bullets/second for instance.
    [SerializeField] float horizontalSpeedLimit = 5.5f;
    [SerializeField] float spriteWidthInPixels = 100; // Used to prevent the sprite from being able to go all the way to the center and clip with the other player if he's also in the center.
    [SerializeField] float health = 1;

    // References
    [SerializeField] GameObject bulletPrefab = null;
    [SerializeField] Image healthImage = null;
    Rigidbody2D playerRigidbody = null;

    // PRIVATE VARIABLES
    #region Private variables
    // Input related
    float horizontalInput = 0;

    // Movement related
    bool movingUsingVelocity = true;

    // Buffers related
    float horizontalScreenHalfSizeInMeters = 8.8f;
    float verticalScreenHalfSizeInMeters = 5;
    HorizontalBuffer currentHorizontalBuffer = HorizontalBuffer.NONE;
    VerticalBuffer currentVerticalBuffer = VerticalBuffer.NONE;
    bool canMoveHorizontally = true; // Prevents player from bumping against the wall when maintaining a horizontal movement direction.

    // Firing related
    float firingTimer = 0;
    #endregion
    #endregion

    // PUBLIC METHODS
    #region Public Methods
    public void DamageOnce(float damage)
    {
        health -= damage;
        healthImage.fillAmount = health;
    }
    public void LifePickup()
    {
        health += GameManager.instance.healthPickupValue;
        if (health > 1)
        {
            health = 1;
        }
        healthImage.fillAmount = health;
    }
    public void SpeedPickup()
    {
        GameManager.instance.SpeedUpBullets(gameObject);
    }
    public void TriggerStorm()
    {
        GameManager.instance.TriggerStorm();
    }
    #endregion

    // PRIVATE METHODS
    #region Private methods
    void InstantiateBullet()
    {
        Vector3 position;
        if (isLeftPlayer)
        {
            position = transform.position + new Vector3(0.5f,0,0);
        }
        else
        {
            position = transform.position + new Vector3(-0.5f, 0, 0);
        }
        Instantiate(bulletPrefab, position, new Quaternion()).GetComponent<BulletController>().Init(isLeftPlayer);
    }
    void ApplyDrag()
    {
        if (horizontalInput == 0)
        {
            if (Mathf.Abs(playerRigidbody.velocity.x) > 0.05f)
            {
                if (playerRigidbody.velocity.x < 0)
                {
                    playerRigidbody.velocity += new Vector2(dragForce, 0);
                }
                else
                {
                    playerRigidbody.velocity += new Vector2(-dragForce, 0);
                }
            }
            else
            {
                playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
            }
        }
    }
    void ApplyBufferForces() // Prevents the player from saying too close the the map edges
    {
        switch (currentHorizontalBuffer)    
        {
            case HorizontalBuffer.NONE:
                break;
            case HorizontalBuffer.LEFT:
                playerRigidbody.AddForce(new Vector2(screenEdgeBufferForce, 0));
                break;
            case HorizontalBuffer.RIGHT:
                playerRigidbody.AddForce(new Vector2(-screenEdgeBufferForce, 0));
                break;
            default:
                break;
        }

        switch (currentVerticalBuffer)
        {
            case VerticalBuffer.NONE:
                break;
            case VerticalBuffer.TOP:
                playerRigidbody.AddForce(new Vector2(0, -screenEdgeBufferForce));
                break;
            case VerticalBuffer.BOTTOM:
                playerRigidbody.AddForce(new Vector2(0, screenEdgeBufferForce));
                break;
            default:
                break;
        }
    }
    void UpdateBufferVariables() // Check if the player is in a buffer zone. If he is, set variables accordingly.
    {
        if (transform.position.y < (-verticalScreenHalfSizeInMeters + screenEdgeBufferDistance))
        {
            currentVerticalBuffer = VerticalBuffer.BOTTOM;
            movingUsingVelocity = false;
        }
        else if (transform.position.y > (verticalScreenHalfSizeInMeters - screenEdgeBufferDistance))
        {
            currentVerticalBuffer = VerticalBuffer.TOP;
            movingUsingVelocity = false;
        }
        else
        {
            currentVerticalBuffer = VerticalBuffer.NONE;
            movingUsingVelocity = true;
        }

        if (transform.position.x < (-horizontalScreenHalfSizeInMeters + screenEdgeBufferDistance))
        {
            currentHorizontalBuffer = HorizontalBuffer.LEFT;
            movingUsingVelocity = false;
            canMoveHorizontally = false;
        }
        else if (transform.position.x > (horizontalScreenHalfSizeInMeters - screenEdgeBufferDistance))
        {
            currentHorizontalBuffer = HorizontalBuffer.RIGHT;
            movingUsingVelocity = false;
            canMoveHorizontally = false;
        }
        else
        {
            currentHorizontalBuffer = HorizontalBuffer.NONE;
            movingUsingVelocity = true;
        }
    }
    void Move()
    {
        if (movingUsingVelocity)
        {
            if (horizontalInput != 0)
            {
                playerRigidbody.velocity = new Vector2(movementVelocity * horizontalInput, playerRigidbody.velocity.y);
            }
            else
            {
                playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
            }
        }
        else
        {
            if (horizontalInput != 0)
            {
                playerRigidbody.AddForce(new Vector2(movementForce * horizontalInput,0));
            }
        }
    }
    void ApplySpeedLimit()
    {
        if (Mathf.Abs(playerRigidbody.velocity.y) > horizontalSpeedLimit)
        {
            if (playerRigidbody.velocity.y < 0)
            {
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, -horizontalSpeedLimit);
            }
            else
            {
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, horizontalSpeedLimit);
            }
        }
    }
    void RestrictPlayerMovementZone() // Prevents the left player from going on the right side of the screen and vice versa
    {
        if (isLeftPlayer)
        {
            if (transform.position.x + (spriteWidthInPixels/100)/2 >= 0)
            {
                if (horizontalInput > 0)
                {
                    playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
                }
                transform.position = new Vector3((-spriteWidthInPixels/100) / 2,transform.position.y,transform.position.z);
            }
        }
        else
        {
            if (transform.position.x - (spriteWidthInPixels / 100) / 2 <= 0)
            {
                if (horizontalInput < 0)
                {
                    playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
                }
                transform.position = new Vector3((spriteWidthInPixels/100) / 2, transform.position.y, transform.position.z);
            }
        }
    }
    #endregion

    // INHERITED METHODS
    #region Inherited methods
    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        UpdateBufferVariables();
        ApplyBufferForces();
        Move();
        ApplyDrag();
        ApplySpeedLimit();
        RestrictPlayerMovementZone();
    }

    private void Update()
    {
        // Handle input
        if (isLeftPlayer)
        {
            // Horizontal movement
            if (canMoveHorizontally)
            {
                horizontalInput = Input.GetAxisRaw("Horizontal_LeftPlayer");
            }
            else
            {
                if (Input.GetAxisRaw("Horizontal_LeftPlayer") > 0)
                {
                    horizontalInput = Input.GetAxisRaw("Horizontal_LeftPlayer");
                }
                else
                {
                    horizontalInput = 0;
                    if (Input.GetButtonDown("Horizontal_LeftPlayer"))
                    {
                        canMoveHorizontally = true;
                    }
                }
            }

            // Parachute
            if (Input.GetButtonDown("Parachute_LeftPlayer"))
            {
                // Inverts gravity
                playerRigidbody.gravityScale = -playerRigidbody.gravityScale;
            }

            // Handle firing input. Handles both button holding and tapping
            if (Input.GetButtonDown("Fire_LeftPlayer"))
            {
                if (firingTimer > firingFrequency)
                {
                    InstantiateBullet();
                    firingTimer = 0;
                }
            }
            else
            {
                if (Input.GetButton("Fire_LeftPlayer"))
                {
                    if (firingTimer > firingFrequency)
                    {
                        InstantiateBullet();
                        firingTimer = 0;
                    }
                }
            }

        }
        else
        {
            if (canMoveHorizontally)
            {
                horizontalInput = Input.GetAxisRaw("Horizontal_RightPlayer");
            }
            else
            {
                if (Input.GetAxisRaw("Horizontal_RightPlayer") < 0)
                {
                    horizontalInput = Input.GetAxisRaw("Horizontal_RightPlayer");
                }
                else
                {
                    horizontalInput = 0;
                    if (Input.GetButtonDown("Horizontal_RightPlayer"))
                    {
                        canMoveHorizontally = true;
                    }
                }
            }

            if (Input.GetButtonDown("Parachute_RightPlayer"))
            {
                playerRigidbody.gravityScale = -playerRigidbody.gravityScale;
            }
            
            if (Input.GetButtonDown("Fire_RightPlayer"))
            {
                if (firingTimer > firingFrequency)
                {
                    InstantiateBullet();
                    firingTimer = 0;
                }
            }
            else
            {
                if (Input.GetButton("Fire_RightPlayer"))
                {
                    if (firingTimer > firingFrequency)
                    {
                        InstantiateBullet();
                        firingTimer = 0;
                    }
                }
            }
        }

        // Handle losing condition
        if (health <= 0)
        {
            GameManager.instance.GameOver(gameObject);
        }

        // Update firing timer
        firingTimer += Time.deltaTime;
    }
    #endregion

}
