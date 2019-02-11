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
            }
            else
            {
                currentProjectileSpeed /= PickupManager.instance.speedupMultiplier;
            }
            _isSpeedup = value;
        }
    }

    // Private variables
    Weapon _currentWeapon = Weapon.PISTOL;
    float currentProjectileSpeed;
    float currentFirerate;
    float currentSpread;
    float currentNumberOfProjectilesPerShot;
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
        currentWeapon = Weapon.PISTOL;
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
                            firingTimer = 1 / currentFirerate;
                        }
                    }
                    break;
                case Weapon.SHOTGUN:
                    {
                        if (firingTimer < 0)
                        {

                            /*
                            for (int i = -PickupManager.instance.numberOfShotgunPelletsPerShot/2; i < PickupManager.instance.numberOfShotgunPelletsPerShot/2; i++)
                            {
                                // Calculate the direction the pellet has to go.
                                float degresToAdd = PickupManager.instance.angleOfShotgunSpread / (PickupManager.instance.numberOfShotgunPelletsPerShot + 1);
                                if (i <= PickupManager.instance.numberOfShotgunPelletsPerShot / 2)
                                {
                                    degresToAdd = -degresToAdd;
                                }
                                Quaternion rotation = gunGO.transform.rotation * Quaternion.Euler(0,0,degresToAdd * i);

                                // Instantiate and setup the bullet.
                                Projectile newProjectile = Instantiate(bulletPrefab, transform.position, rotation).GetComponent<Projectile>();
                                newProjectile.speed = currentBulletsSpeed;
                                newProjectile.type = Projectile.Type.PELLET;
                            }
                            firingTimer = 1 / currentFirerate;
                            */
                        }
                    }
                    break;
                case Weapon.SNIPER:
                    {
                        if (firingTimer < 0)
                        {
                            Projectile newProjectile = Instantiate(bulletsPrefabs[2], transform.position, playerManager.armGO.transform.rotation).GetComponent<Projectile>();
                            newProjectile.speed = playerManager.weaponsData[2].projectileSpeed;
                            newProjectile.type = Weapon.SNIPER;
                            firingTimer = 1 / playerManager.weaponsData[2].firerate;
                        }
                    }
                    break;
                case Weapon.BAZOOKA:
                    {
                        if (firingTimer < 0)
                        {
                            Projectile newProjectile = Instantiate(bulletsPrefabs[3], transform.position, playerManager.armGO.transform.rotation).GetComponent<Projectile>();
                            newProjectile.speed = playerManager.weaponsData[3].projectileSpeed;
                            newProjectile.type = Weapon.BAZOOKA;
                            firingTimer = 1 / playerManager.weaponsData[3].firerate;
                        }
                    }
                    break;
                case Weapon.MINIGUN:
                    {
                        if (firingTimer < 0)
                        {
                            // Calculate the direction the bullet will go with spread accounted.
                            float randomSpread = Random.Range(-playerManager.weaponsData[4].spread / 2, -playerManager.weaponsData[4].spread / 2);
                            Quaternion rotation = playerManager.armGO.transform.rotation * Quaternion.Euler(0, 0, randomSpread);

                            Projectile newProjectile = Instantiate(bulletsPrefabs[4], transform.position, rotation).GetComponent<Projectile>();
                            newProjectile.speed = playerManager.weaponsData[4].projectileSpeed;
                            newProjectile.type = Weapon.MINIGUN;
                            firingTimer = 1 / playerManager.weaponsData[4].firerate;
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
