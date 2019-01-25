using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// get input
// move player accordingly
// fire a projectile if there was input for it
// if health < 1 ==> die

public class PlayerController : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] bool isLeftPlayer = true;
    [SerializeField] float dragForce = 0.1f;
    [SerializeField] float movementForceMultiplier = 1;
    [SerializeField] float screenEdgeBufferDistance = 0.5f;
    [SerializeField] float screenEdgeBufferForceMultiplier = 1;
    [SerializeField] float firingFrequency = 1; // Smaller is quicker. 2 => 2/second = 0.5 for instance.

    // References
    [SerializeField] GameObject bulletPrefab = null;

    // Private variables
    float horizontalInput = 0;
    Rigidbody2D playerRigidbody = null;
    Vector2 playerVelocity = new Vector2();
    float horizontalScreenHalfSizeInMeters = 9.8f;
    float verticalScreenHalfSizeInMeters = 5;
    float firingTimer = 0;
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
        playerVelocity = playerRigidbody.velocity;
        if (Mathf.Abs(playerVelocity.x) > 0.05f)
        {
            if (playerVelocity.x < 0)
            {
                playerRigidbody.velocity += new Vector2(-dragForce, 0);
            }
            else
            {
                playerRigidbody.velocity += new Vector2(dragForce, 0);
            }
        }
        else
        {
            playerRigidbody.velocity = new Vector2(0, playerVelocity.y);
        }
    }
    void ApplyBufferForces()
    {
        // If entering bottom buffer zone.
        if (transform.position.y < (-verticalScreenHalfSizeInMeters + screenEdgeBufferDistance))
        {
            float distanceFromThreshold = Vector2.Distance(transform.position, new Vector2(transform.position.x, -verticalScreenHalfSizeInMeters + screenEdgeBufferDistance));
            playerRigidbody.AddForce(new Vector2(0, screenEdgeBufferForceMultiplier * distanceFromThreshold));
        }
        // If entering top buffer zone.
        else if (transform.position.y > (verticalScreenHalfSizeInMeters - screenEdgeBufferDistance))
        {
            float distanceFromThreshold = Vector2.Distance(transform.position, new Vector2(transform.position.x, verticalScreenHalfSizeInMeters - screenEdgeBufferDistance));
            playerRigidbody.AddForce(new Vector2(0, -screenEdgeBufferForceMultiplier * distanceFromThreshold));
        }
        // If entering left buffer zone.
        else if (transform.position.x < (-horizontalScreenHalfSizeInMeters + screenEdgeBufferDistance))
        {
            float distanceFromThreshold = Vector2.Distance(transform.position, new Vector2(-horizontalScreenHalfSizeInMeters + screenEdgeBufferDistance , transform.position.y));
            playerRigidbody.AddForce(new Vector2(screenEdgeBufferForceMultiplier * distanceFromThreshold, 0));
        }
        // If entering right buffer zone.
        else if (transform.position.x > (horizontalScreenHalfSizeInMeters - screenEdgeBufferDistance))
        {
            float distanceFromThreshold = Vector2.Distance(transform.position, new Vector2(horizontalScreenHalfSizeInMeters - screenEdgeBufferDistance, transform.position.y));
            playerRigidbody.AddForce(new Vector2(-screenEdgeBufferForceMultiplier * distanceFromThreshold, 0));
        }
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (horizontalInput != 0)
        {
            playerRigidbody.AddForce(new Vector2(horizontalInput * movementForceMultiplier, 0));
        }

        ApplyDrag();
        ApplyBufferForces();
    }

    private void Update()
    {
        // Handle input
        if (isLeftPlayer)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal_LeftPlayer");
            if (Input.GetButtonDown("Parachute_LeftPlayer"))
            {
                // Inverts gravity
                playerRigidbody.gravityScale = -playerRigidbody.gravityScale;
            }
            if (Input.GetButton("Fire_LeftPlayer"))
            {
                if (firingTimer > firingFrequency)
                {
                    InstantiateBullet();
                    firingTimer = 0;
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
            if (Input.GetButton("Fire_RightPlayer"))
            {
                InstantiateBullet();
            }
        }

        // Update firing timer
        firingTimer += Time.deltaTime;
    }
    #endregion

}
