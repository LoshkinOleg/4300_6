using System.Collections;
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
    public enum Weapon
    {
        PISTOL,
        SHOTGUN,
        SNIPER,
        BAZOOKA,
        MINIGUN
    }
    #endregion

    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] bool isLeftPlayer = true;
    [SerializeField] float dragForce = 0.1f;
    [SerializeField] float _groundHorizontalVelocity = 3;
    [SerializeField] float _airborneHorizontalForce = 20;
    [SerializeField] float _playerSpeedLimit = 5;
    [SerializeField] float dodgeDuration = 0.05f;
    [SerializeField] float dodgeSpeedupMagnitude = 10;
    [SerializeField] float stunDuration = 0.5f;

    // References
    [SerializeField] GameObject bulletPrefab = null;
    [SerializeField] GameObject healthImageGO = null;
    [SerializeField] GameObject parachuteGO = null;
    [SerializeField] GameObject gunGO = null;
    [SerializeField] GameObject[] weaponsGOs = new GameObject[5];
    BoxCollider2D playerCollider = null;
    Rigidbody2D playerRigidbody = null;
    Image healthImage = null;

    // Properties
    public MovementMode currentMovementMode
    {
        get
        {
            return _currentMovementMode;
        }
        set
        {
            if (value == MovementMode.JETPACK)
            {
                if (parachuteIsOpen)
                {
                    ToggleParachute();
                }
            }
            _currentMovementMode = value;
        }
    }
    public Weapon currentWeapon
    {
        get
        {
            return _currentWeapon;
        }
        set
        {
            switch (value)
            {
                case Weapon.BAZOOKA:
                    {
                        for (int i = 0; i < weaponsGOs.Length; i++)
                        {
                            if (i == 3)
                            {
                                weaponsGOs[i].SetActive(true);
                            }
                            else
                            {
                                weaponsGOs[i].SetActive(false);
                            }
                        }
                        baseFirerate = PickupManager.instance.firerate_bazooka;
                        baseBulletSpeed = PickupManager.instance.bulletSpeed_bazooka;
                        currentBulletDamage = PickupManager.instance.bulletDamage_bazooka;
                        currentBulletSpread = PickupManager.instance.bulletSpread_bazooka;
                    }
                    break;
                case Weapon.MINIGUN:
                    {
                        for (int i = 0; i < weaponsGOs.Length; i++)
                        {
                            if (i == 4)
                            {
                                weaponsGOs[i].SetActive(true);
                            }
                            else
                            {
                                weaponsGOs[i].SetActive(false);
                            }
                        }
                        baseFirerate = PickupManager.instance.firerate_minigun;
                        baseBulletSpeed = PickupManager.instance.bulletSpeed_minigun;
                        currentBulletDamage = PickupManager.instance.bulletDamage_minigun;
                        currentBulletSpread = PickupManager.instance.bulletSpread_minigun;
                    }
                    break;
                case Weapon.PISTOL:
                    {
                        for (int i = 0; i < weaponsGOs.Length; i++)
                        {
                            if (i == 0)
                            {
                                weaponsGOs[i].SetActive(true);
                            }
                            else
                            {
                                weaponsGOs[i].SetActive(false);
                            }
                        }
                        baseFirerate = PickupManager.instance.firerate_pistol;
                        baseBulletSpeed = PickupManager.instance.bulletSpeed_pistol;
                        currentBulletDamage = PickupManager.instance.bulletDamage_pistol;
                        currentBulletSpread = PickupManager.instance.bulletSpread_pistol;
                    }
                    break;
                case Weapon.SHOTGUN:
                    {
                        for (int i = 0; i < weaponsGOs.Length; i++)
                        {
                            if (i == 1)
                            {
                                weaponsGOs[i].SetActive(true);
                            }
                            else
                            {
                                weaponsGOs[i].SetActive(false);
                            }
                        }
                        baseFirerate = PickupManager.instance.firerate_shotgun;
                        baseBulletSpeed = PickupManager.instance.bulletSpeed_shotgun;
                        currentBulletDamage = PickupManager.instance.bulletDamage_shotgun;
                        currentBulletSpread = PickupManager.instance.bulletSpread_shotgun;
                    }
                    break;
                case Weapon.SNIPER:
                    {
                        for (int i = 0; i < weaponsGOs.Length; i++)
                        {
                            if (i == 2)
                            {
                                weaponsGOs[i].SetActive(true);
                            }
                            else
                            {
                                weaponsGOs[i].SetActive(false);
                            }
                        }
                        baseFirerate = PickupManager.instance.firerate_sniper;
                        baseBulletSpeed = PickupManager.instance.bulletSpeed_sniper;
                        currentBulletDamage = PickupManager.instance.bulletDamage_sniper;
                        currentBulletSpread = PickupManager.instance.bulletSpread_sniper;
                    }
                    break;
            }
            currentFirerate = baseFirerate;
            currentBulletsSpeed = baseBulletSpeed;
            _currentWeapon = value;
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

    // Private variables
    #region Private variables
    // Basic movement related
    MovementMode _currentMovementMode = MovementMode.AIRBORNE;
    float horizontalInput = 0;
    bool parachuteIsOpen = false;
    // Dodging related
    bool dodging = false;
    Vector2 dodgeDirection = new Vector2();
    float dodgeTimer;
    // Jetpack related
    float jetpackTimer;
    float verticalInput = 0;
    // Player and gun orientation related
    PlayerDirection currentPlayerDirection;
    GunDirection currentGunDirection = GunDirection.FORWARD;
    // Speedup pickup related
    float currentBulletsSpeed;
    float baseBulletSpeed;
    bool isSpeedup = false;
    float speedupTimer;
    // Firing related
    Weapon _currentWeapon = Weapon.PISTOL;
    float aimingHorizontalInput = 0;
    float aimingVerticalInput = 0;
    bool firing = false;
    float currentFirerate = 1;
    float baseFirerate;
    float firingTimer = 0;
    float currentBulletDamage;
    float currentBulletSpread;
    // Stun related
    float stunForceMultiplier = 50;
    float stunTimer = 0;
    BoxCollider2D lastCrateBottomHit_collider = null;

    int _lives = 3;
    float health = 1;
    #endregion
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
        if (!isSpeedup)
        {
            isSpeedup = true;
            speedupTimer = PickupManager.instance.speedupPickupTime;
            currentBulletsSpeed *= PickupManager.instance.speedupMultiplier;
            currentFirerate *= PickupManager.instance.speedupMultiplier;
        }
        else
        {
            speedupTimer = PickupManager.instance.speedupPickupTime;
        }
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
        stunTimer = stunDuration;
    }
    public void HitCrateBottom(BoxCollider2D crate)
    {
        lastCrateBottomHit_collider = crate;
        Stun();
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
            health = 1;
            healthImage.fillAmount = health;
            transform.position = new Vector3(0, 0, 0);
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
                        if (!dodging)
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
                        else
                        {
                            // Normalize player velocity and add a fixed speed boost.
                            playerRigidbody.velocity = (Vector2)Vector3.Normalize(playerRigidbody.velocity) + dodgeDirection * dodgeSpeedupMagnitude;
                        }
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
            switch (currentWeapon)
            {
                case Weapon.PISTOL:
                    {
                        if (firingTimer < 0)
                        {
                            Projectile newProjectile = Instantiate(bulletPrefab, transform.position, gunGO.transform.rotation).GetComponent<Projectile>();
                            newProjectile.speed = currentBulletsSpeed;
                            newProjectile.type = Projectile.Type.SIMPLE;
                            firingTimer = 1 / currentFirerate;
                        }
                    }
                    break;
                case Weapon.SHOTGUN:
                    {
                        if (firingTimer < 0)
                        {
                            for (int i = 0; i < PickupManager.instance.numberOfShotgunPelletsPerShot; i++)
                            {
                                Quaternion rotation = gunGO.transform.rotation * Quaternion.Euler(0,0,0);
                                Projectile newProjectile = Instantiate(bulletPrefab, transform.position, gunGO.transform.rotation).GetComponent<Projectile>();
                                newProjectile.speed = currentBulletsSpeed;
                                newProjectile.type = Projectile.Type.SIMPLE;
                            }
                            firingTimer = 1 / currentFirerate;
                        }
                    }
                    break;
                case Weapon.SNIPER:
                    {
                        if (firingTimer < 0)
                        {
                            Projectile newProjectile = Instantiate(bulletPrefab, transform.position, gunGO.transform.rotation).GetComponent<Projectile>();
                            newProjectile.speed = currentBulletsSpeed;
                            newProjectile.type = Projectile.Type.SIMPLE;
                            firingTimer = 1 / currentFirerate;
                        }
                    }
                    break;
                case Weapon.BAZOOKA:
                    {
                        if (firingTimer < 0)
                        {
                            Projectile newProjectile = Instantiate(bulletPrefab, transform.position, gunGO.transform.rotation).GetComponent<Projectile>();
                            newProjectile.speed = currentBulletsSpeed;
                            newProjectile.type = Projectile.Type.SIMPLE;
                            firingTimer = 1 / currentFirerate;
                        }
                    }
                    break;
                case Weapon.MINIGUN:
                    {
                        if (firingTimer < 0)
                        {
                            Projectile newProjectile = Instantiate(bulletPrefab, transform.position, gunGO.transform.rotation).GetComponent<Projectile>();
                            newProjectile.speed = currentBulletsSpeed;
                            newProjectile.type = Projectile.Type.SIMPLE;
                            firingTimer = 1 / currentFirerate;
                        }
                    }
                    break;
            }
        }

        if (speedupTimer < 0)
        {
            if (isSpeedup)
            {
                currentBulletsSpeed = baseBulletSpeed;
                currentFirerate = baseFirerate;
                isSpeedup = false;
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
    void ProcessStunMechanic()
    {
        if (stunTimer > 0)
        {
            playerRigidbody.gravityScale = 0;
            if (lastCrateBottomHit_collider != null)
            {
                if (playerCollider.IsTouching(GameManager.instance.bottomBoundsCollider) && playerCollider.IsTouching(lastCrateBottomHit_collider))
                {
                    Kill();
                }
                else if (playerCollider.IsTouching(lastCrateBottomHit_collider))
                {
                    playerRigidbody.AddForce(Vector2.down * stunForceMultiplier);
                }
            }
        }
    }
    #endregion

    // INHERITED METHODS
    #region Inherited methods
    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        healthImage = healthImageGO.GetComponent<Image>();
        firingTimer = 1 / currentFirerate;
        baseFirerate = PickupManager.instance.firerate_pistol;
        currentFirerate = baseFirerate;
        baseBulletSpeed = PickupManager.instance.bulletSpeed_pistol;
        currentBulletsSpeed = baseBulletSpeed;
        SetMovementMode(MovementMode.AIRBORNE);
        currentWeapon = Weapon.PISTOL;

        if (isLeftPlayer)
        {
            currentPlayerDirection = PlayerDirection.RIGHT;
        }
        else
        {
            currentPlayerDirection = PlayerDirection.LEFT;
        }
    }

    private void FixedUpdate()
    {
        ApplySpeedLimit();
        OrientSpriteAndGun();
        Move();
        ApplyDrag();
        Shoot();
        ProcessStunMechanic();
    }

    private void Update()
    {
        if (stunTimer <= 0)
        {
            // Reset gravity after the player has just exited stun mode.
            if (Mathf.Abs(playerRigidbody.gravityScale) != 2)
            {
                if (parachuteIsOpen)
                {
                    playerRigidbody.gravityScale = -2;
                }
                else
                {
                    playerRigidbody.gravityScale = 2;
                }
            }

            if (isLeftPlayer)
            {
                // Handle inputs
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
                if (Input.GetButtonUp("Player1_Fire"))
                {
                    firing = false;
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
                            if (Input.GetButtonDown("Player2_Parachute"))
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
                            if (Input.GetButtonDown("Player2_Parachute"))
                            {
                                playerRigidbody.gravityScale = -playerRigidbody.gravityScale;
                                parachuteGO.SetActive(!parachuteGO.activeSelf);
                                parachuteIsOpen = !parachuteIsOpen;
                                dodgeTimer = dodgeDuration;
                            }
                        }
                        break;
                }

                if (Input.GetButtonDown("Player2_Fire"))
                {
                    firing = true;
                }
                if (Input.GetButtonUp("Player2_Fire"))
                {
                    firing = false;
                }
            }
        }
        
        // Handle losing condition
        if (health <= 0)
        {
            Kill();
        }

        // Update timers
        firingTimer -= Time.deltaTime;
        dodgeTimer -= Time.deltaTime;
        speedupTimer -= Time.deltaTime;
        jetpackTimer -= Time.deltaTime;
        stunTimer -= Time.deltaTime;
    }
    #endregion

}
