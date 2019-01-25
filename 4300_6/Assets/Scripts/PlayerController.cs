using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] bool isLeftPlayer = true;
    [SerializeField] float dragForce = 0.1f;
    [SerializeField] float defaultMovementVelocity = 7;
    [SerializeField] float screenEdgeBufferDistance = 1f;
    [SerializeField] float screenEdgeBufferForce = 25f;
    [SerializeField] float firingFrequency = 0.25f; // Smaller is quicker. 0.25 => 4 bullets/second for instance.
    [SerializeField] float horizontalSpeedLimit = 5.5f;
    [SerializeField] float spriteWidthInPixels = 100;

    // References
    [SerializeField] GameObject bulletPrefab = null;

    // Private variables
    float horizontalInput = 0;
    Rigidbody2D playerRigidbody = null;
    Vector2 playerVelocity = new Vector2();
    float horizontalScreenHalfSizeInMeters = 8.8f;
    float verticalScreenHalfSizeInMeters = 5;
    float firingTimer = 0;
    bool inSideBuffer = false;
    float movementVelocity;
    #endregion

    // Private methods
    #region Private methods
    void InstantiateBullet()
    {
        if (isLeftPlayer)
        {
            Instantiate(bulletPrefab, transform.position, new Quaternion());
        }
        else
        {
            Quaternion direction = Quaternion.Euler(0,180,0);
            GameObject newBullet = Instantiate(bulletPrefab, transform.position, direction);
        }
    }
    void ApplyDrag()
    {
        if (horizontalInput == 0)
        {
            playerVelocity = playerRigidbody.velocity;
            if (Mathf.Abs(playerVelocity.x) > 0.05f)
            {
                if (playerVelocity.x < 0)
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
                playerRigidbody.velocity = new Vector2(0, playerVelocity.y);
            }
        }
    }
    void ApplyBufferForces()
    {
        // If entering bottom buffer zone.
        if (transform.position.y < (-verticalScreenHalfSizeInMeters + screenEdgeBufferDistance))
        {
            playerRigidbody.AddForce(new Vector2(0, screenEdgeBufferForce));
        }
        // If entering top buffer zone.
        else if (transform.position.y > (verticalScreenHalfSizeInMeters - screenEdgeBufferDistance))
        {
            playerRigidbody.AddForce(new Vector2(0, -screenEdgeBufferForce));
        }
        // If entering left buffer zone.
        else if (transform.position.x < (-horizontalScreenHalfSizeInMeters + screenEdgeBufferDistance))
        {
            playerRigidbody.AddForce(new Vector2(screenEdgeBufferForce, 0));
            inSideBuffer = true;
        }
        // If entering right buffer zone.
        else if (transform.position.x > (horizontalScreenHalfSizeInMeters - screenEdgeBufferDistance))
        {
            playerRigidbody.AddForce(new Vector2(-screenEdgeBufferForce , 0));
            inSideBuffer = true;
        }
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        movementVelocity = defaultMovementVelocity;
    }

    private void FixedUpdate()
    {
        ApplyBufferForces();

        if (horizontalInput != 0)
        {
            if (!inSideBuffer)
            {
                playerRigidbody.velocity = new Vector2(movementVelocity * horizontalInput, playerRigidbody.velocity.y);
            }
        }
        else
        {
            playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
        }
        inSideBuffer = false; // Reset bool.

        ApplyDrag();

        // Limit horizontal movement speed to prevent gravity from accelerating the players too much
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

        // Limit player movement to half the screen horizontally
        if (isLeftPlayer)
        {
            if (transform.position.x + spriteWidthInPixels /(100/2) >= 0)
            {
                transform.position = new Vector3(-spriteWidthInPixels /(100/2),transform.position.y,transform.position.z);
            }
        }
        else
        {
            if (transform.position.x - spriteWidthInPixels / (100 / 2) <= 0)
            {
                transform.position = new Vector3(spriteWidthInPixels / (100 / 2), transform.position.y, transform.position.z);
            }
        }
    }

    private void Update()
    {
        // Handle input
        if (isLeftPlayer)
        {
            // Horizontal movement
            horizontalInput = Input.GetAxisRaw("Horizontal_LeftPlayer");

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
            horizontalInput = Input.GetAxisRaw("Horizontal_RightPlayer");
            
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

        // Update firing timer
        firingTimer += Time.deltaTime;
    }
    #endregion

}
