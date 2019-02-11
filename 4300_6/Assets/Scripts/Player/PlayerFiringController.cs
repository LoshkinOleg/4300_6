using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFiringController : MonoBehaviour
{
    // Classes and Enums
    public enum Weapon
    {
        PISTOL,
        SHOTGUN,
        SNIPER,
        BAZOOKA,
        MINIGUN
    }

    // Attributes
    #region Attributes
    // References
    [HideInInspector] public PlayerManager _playerManager = null;
    [SerializeField] GameObject[] bulletsPrefabs = new GameObject[(int)Weapon.MINIGUN + 1];

    // Public properties
    public PlayerManager playerManager
    {
        get
        {
            return _playerManager;
        }
        set
        {
            if (_playerManager == null)
            {
                _playerManager = value;
            }
            else
            {
                Debug.LogWarning("Attempting to modify PlayerManager reference after it has been set.");
            }
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
            _currentWeapon = value;
            currentProjectileSpeed = playerManager.weaponsData[(int)value].projectileSpeed;
            currentFirerate = playerManager.weaponsData[(int)value].firerate;
            currentSpread = playerManager.weaponsData[(int)value].spread;
            currentNumberOfProjectilesPerShot = playerManager.weaponsData[(int)value].numberOfProjectiles;
            currentFiringKnockback = playerManager.weaponsData[(int)value].firingKnockback;
        }
    }

    // Private properties
    bool isSpeedup
    {
        get
        {
            return _isSpeedup;
        }
        set
        {
            if (value)
            {
                bulletsSpeedupTimer = PickupManager.instance.speedupPickupTime;
                currentProjectileSpeed *= PickupManager.instance.speedupMultiplier;
                currentFirerate *= PickupManager.instance.speedupMultiplier;
            }
            else
            {
                currentProjectileSpeed /= PickupManager.instance.speedupMultiplier;
                currentFirerate /= PickupManager.instance.speedupMultiplier;
            }
            _isSpeedup = value;
        }
    }

    // Private variables
    Weapon _currentWeapon = Weapon.PISTOL;
    float currentProjectileSpeed;
    float currentFirerate;
    float currentSpread;
    int currentNumberOfProjectilesPerShot;
    float currentFiringKnockback;
    bool _isSpeedup;
    float firingTimer;
    float bulletsSpeedupTimer;
    #endregion

    // Public methods
    #region Public methods
    public void SpeedBulletsUp()
    {
        isSpeedup = true;
    }
    public void Init()
    {
        currentWeapon = Weapon.BAZOOKA;
    }
    #endregion

    // Private methods
    #region Private methods
    void Shoot()
    {
        if (playerManager.tryingToFire)
        {
            switch (currentWeapon)
            {
                case Weapon.PISTOL:
                    {
                        if (firingTimer < 0)
                        {
                            // Calculate the direction the bullet will go with spread accounted.
                            float randomSpread = Random.Range(-currentSpread / 2, currentSpread / 2);
                            Quaternion rotation = playerManager.armGO.transform.rotation * Quaternion.Euler(0, 0, randomSpread);

                            Projectile newProjectile = Instantiate(bulletsPrefabs[0], transform.position, rotation).GetComponent<Projectile>();
                            newProjectile.speed = currentProjectileSpeed;
                            newProjectile.type = currentWeapon;

                            Vector2 direction = Vector3.Normalize(-playerManager.armGO.transform.right);
                            playerManager.physicsHandler.AddForce(direction * currentFiringKnockback);
                            firingTimer = 1 / currentFirerate;
                        }
                    }
                    break;
                case Weapon.SHOTGUN:
                    {
                        if (firingTimer < 0)
                        {
                            for (int i = -currentNumberOfProjectilesPerShot/2; i < currentNumberOfProjectilesPerShot / 2; i++)
                            {
                                float randomSpread = Random.Range(-currentSpread / 2, currentSpread / 2);
                                Quaternion rotation = playerManager.armGO.transform.rotation * Quaternion.Euler(0, 0, randomSpread);

                                Projectile newProjectile = Instantiate(bulletsPrefabs[1], transform.position, rotation).GetComponent<Projectile>();
                                newProjectile.speed = currentProjectileSpeed;
                                newProjectile.type = currentWeapon;
                            }

                            Vector2 direction = Vector3.Normalize(-playerManager.armGO.transform.right);
                            playerManager.physicsHandler.AddForce(direction * currentFiringKnockback);
                            firingTimer = 1 / currentFirerate;
                        }
                    }
                    break;
                case Weapon.SNIPER:
                    {
                        if (firingTimer < 0)
                        {
                            float randomSpread = Random.Range(-currentSpread / 2, currentSpread / 2);
                            Quaternion rotation = playerManager.armGO.transform.rotation * Quaternion.Euler(0, 0, randomSpread);

                            Projectile newProjectile = Instantiate(bulletsPrefabs[2], transform.position, rotation).GetComponent<Projectile>();
                            newProjectile.speed = currentProjectileSpeed;
                            newProjectile.type = currentWeapon;

                            Vector2 direction = Vector3.Normalize(-playerManager.armGO.transform.right);
                            playerManager.physicsHandler.AddForce(direction * currentFiringKnockback);
                            firingTimer = 1 / currentFirerate;
                        }
                    }
                    break;
                case Weapon.BAZOOKA:
                    {
                        if (firingTimer < 0)
                        {
                            float randomSpread = Random.Range(-currentSpread / 2, currentSpread / 2);
                            Quaternion rotation = playerManager.armGO.transform.rotation * Quaternion.Euler(0, 0, randomSpread);

                            Projectile newProjectile = Instantiate(bulletsPrefabs[3], transform.position, rotation).GetComponent<Projectile>();
                            newProjectile.speed = currentProjectileSpeed;
                            newProjectile.type = currentWeapon;

                            Vector2 direction = Vector3.Normalize(-playerManager.armGO.transform.right);
                            playerManager.physicsHandler.AddForce(direction * currentFiringKnockback);
                            firingTimer = 1 / currentFirerate;
                        }
                    }
                    break;
                case Weapon.MINIGUN:
                    {
                        if (firingTimer < 0)
                        {
                            float randomSpread = Random.Range(-currentSpread / 2, currentSpread / 2);
                            Quaternion rotation = playerManager.armGO.transform.rotation * Quaternion.Euler(0, 0, randomSpread);

                            Projectile newProjectile = Instantiate(bulletsPrefabs[4], transform.position, rotation).GetComponent<Projectile>();
                            newProjectile.speed = currentProjectileSpeed;
                            newProjectile.type = currentWeapon;
                            
                            Vector2 direction = Vector3.Normalize(-playerManager.armGO.transform.right);
                            playerManager.physicsHandler.AddForce(direction * currentFiringKnockback);
                            firingTimer = 1 / currentFirerate;
                        }
                    }
                    break;
            }
        }

        if (bulletsSpeedupTimer < 0)
        {
            if (isSpeedup)
            {
                isSpeedup = false;
            }
        }
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void FixedUpdate()
    {
        Shoot();
    }
    private void Update()
    {
        firingTimer -= Time.deltaTime;
        bulletsSpeedupTimer -= Time.deltaTime;
    }
    #endregion
}
