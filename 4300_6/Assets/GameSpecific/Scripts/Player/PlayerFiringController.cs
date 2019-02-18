﻿using System.Collections;
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
    private enum MinigunSoundStage
    {
        STOPPED,
        SPINNING_UP,
        FIRING,
        SLOWING_DOWN
    }

    // Attributes
    #region Attributes
    // References
    [HideInInspector] public PlayerManager _playerManager = null;
    [SerializeField] GameObject[] bulletsPrefabs = new GameObject[(int)Weapon.MINIGUN + 1];

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
    // Sound related
    MinigunSoundStage minigunSoundStage // Triggers the right minigun sound when assigned to.
    {
        get
        {
            return _minigunSoundStage;
        }
        set
        {
            switch (value)
            {
                case MinigunSoundStage.SPINNING_UP:
                    {
                        SoundManager.Instance.PlaySound("minigun_spinup");
                    }
                    break;
                case MinigunSoundStage.FIRING:
                    {
                        switch (_minigunSoundStage)
                        {
                            case MinigunSoundStage.SPINNING_UP:
                                {
                                    SoundManager.Instance.PlaySound("minigun_fire");
                                }break;
                            case MinigunSoundStage.SLOWING_DOWN:
                                {
                                    SoundManager.Instance.StopSoundNoFadeout("minigun_slowdown");
                                    SoundManager.Instance.PlaySound("minigun_fire");
                                }
                                break;
                        }
                    }
                    break;
                case MinigunSoundStage.SLOWING_DOWN:
                    {
                        SoundManager.Instance.StopSoundWithFadeout("minigun_fire");
                        SoundManager.Instance.PlaySound("minigun_slowdown");
                    }break;
                case MinigunSoundStage.STOPPED:
                    {
                        SoundManager.Instance.StopSoundWithFadeout("minigun_spinup");
                    }
                    break;
            }

            _minigunSoundStage = value;
        }
    }
    bool isSpinningUp;
    bool isFiringMinigun;
    bool isSlowingDown;
    float spinupTimer;
    float spinupTime;
    float slowdownTimer;
    float slowdownTime;
    MinigunSoundStage _minigunSoundStage = MinigunSoundStage.STOPPED;
    #endregion

    // Public properties
    #region Public properties
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
    #endregion

    // Public methods
    #region Public methods
    public void SpeedBulletsUp()
    {
        isSpeedup = true;
    }
    public void Init()
    {
        currentWeapon = Weapon.SNIPER;
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
                            // Instantiate bullet
                            Projectile newProjectile = Instantiate(bulletsPrefabs[0], transform.position, rotation).GetComponent<Projectile>();
                            newProjectile.speed = currentProjectileSpeed;
                            newProjectile.type = currentWeapon;
                            // Apply firing knockback
                            Vector2 direction = Vector3.Normalize(-playerManager.armGO.transform.right);
                            playerManager.ApplyFiringKnockback(direction, currentFiringKnockback);
                            // Reset firing timer.
                            firingTimer = 1 / currentFirerate;
                            // Play sound
                            if (SoundManager.Instance != null)
                            {
                                SoundManager.Instance.PlaySound("pistol_fire");
                            }
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
                            playerManager.ApplyFiringKnockback(direction, currentFiringKnockback);

                            firingTimer = 1 / currentFirerate;

                            if (SoundManager.Instance != null)
                            {
                                SoundManager.Instance.PlaySound("shotgun_fire");
                            }
                        }
                    }
                    break;
                case Weapon.SNIPER:
                    {
                        if (firingTimer < 0)
                        {
                            // Cast a hitscan. Apply sniper hit mechanics if a player is hit.
                            if (playerManager.isLeftPlayer)
                            {
                                if (Physics2D.Raycast(playerManager.armGO.transform.position + playerManager.armGO.transform.right, playerManager.armGO.transform.right).collider.gameObject.tag == "Player2")
                                {
                                    GameManager.Instance.Player2.SniperHit();
                                }
                            }
                            else
                            {
                                if (Physics2D.Raycast(playerManager.armGO.transform.position + playerManager.armGO.transform.right, playerManager.armGO.transform.right).collider.gameObject.tag == "Player1")
                                {
                                    GameManager.Instance.Player1.SniperHit();
                                }
                            }

                            Vector2 direction = Vector3.Normalize(-playerManager.armGO.transform.right);
                            playerManager.ApplyFiringKnockback(direction, currentFiringKnockback);

                            firingTimer = 1 / currentFirerate;

                            if (SoundManager.Instance != null)
                            {
                                SoundManager.Instance.PlaySound("sniper_fire");
                            }
                        }
                    }
                    break;
                case Weapon.BAZOOKA:
                    {
                        if (firingTimer < 0)
                        {
                            // Instantiate a projectile without any spread applied.
                            Projectile newProjectile = Instantiate(bulletsPrefabs[3], transform.position, new Quaternion()).GetComponent<Projectile>();
                            newProjectile.speed = currentProjectileSpeed;
                            newProjectile.type = currentWeapon;

                            Vector2 direction = Vector3.Normalize(-playerManager.armGO.transform.right);
                            playerManager.ApplyFiringKnockback(direction, currentFiringKnockback);

                            firingTimer = 1 / currentFirerate;

                            if (SoundManager.Instance != null)
                            {
                                SoundManager.Instance.PlaySound("bazooka_fire");
                            }
                        }
                    }
                    break;
                case Weapon.MINIGUN:
                    {
                        switch (minigunSoundStage)
                        {
                            case MinigunSoundStage.STOPPED:
                                {
                                    minigunSoundStage = MinigunSoundStage.SPINNING_UP;
                                    spinupTimer = spinupTime;
                                    slowdownTimer = slowdownTime;
                                }break;
                            case MinigunSoundStage.SPINNING_UP:
                                {
                                    if (spinupTimer < 0)
                                    {
                                        minigunSoundStage = MinigunSoundStage.FIRING;
                                    }
                                }break;
                            case MinigunSoundStage.FIRING:
                                {
                                    if (firingTimer < 0)
                                    {
                                        float randomSpread = Random.Range(-currentSpread / 2, currentSpread / 2);
                                        Quaternion rotation = playerManager.armGO.transform.rotation * Quaternion.Euler(0, 0, randomSpread);

                                        Projectile newProjectile = Instantiate(bulletsPrefabs[4], transform.position, rotation).GetComponent<Projectile>();
                                        newProjectile.speed = currentProjectileSpeed;
                                        newProjectile.type = currentWeapon;

                                        Vector2 direction = Vector3.Normalize(-playerManager.armGO.transform.right);
                                        playerManager.ApplyFiringKnockback(direction, currentFiringKnockback);

                                        firingTimer = 1 / currentFirerate;
                                    }
                                }break;
                            case MinigunSoundStage.SLOWING_DOWN:
                                {
                                    minigunSoundStage = MinigunSoundStage.FIRING;
                                }break;
                        }

                        /*
                        // Manage spinup mechanic
                        if (spinupTimer > 0)
                        {
                            if (minigunSoundStage != MinigunSoundStage.SPINNING_UP)
                            {
                                minigunSoundStage = MinigunSoundStage.SPINNING_UP;

                                if (!isSpinningUp)
                                {
                                    SoundManager.Instance.PlayNewSound("minigun_spinup");
                                    isSpinningUp = true;
                                }
                            }
                        }
                        else
                        {
                            if (minigunSoundStage == MinigunSoundStage.SPINNING_UP)
                            {
                                minigunSoundStage = MinigunSoundStage.FIRING;
                            }
                            else if (minigunSoundStage == MinigunSoundStage.FIRING)
                            {
                                if (!isFiringMinigun)
                                {
                                    SoundManager.Instance.PlayNewSound("minigun_fire");
                                    isFiringMinigun = true;
                                    isSpinningUp = false;
                                }
                                else
                                {
                                    if (firingTimer < 0)
                                    {
                                        float randomSpread = Random.Range(-currentSpread / 2, currentSpread / 2);
                                        Quaternion rotation = playerManager.armGO.transform.rotation * Quaternion.Euler(0, 0, randomSpread);

                                        Projectile newProjectile = Instantiate(bulletsPrefabs[4], transform.position, rotation).GetComponent<Projectile>();
                                        newProjectile.speed = currentProjectileSpeed;
                                        newProjectile.type = currentWeapon;

                                        Vector2 direction = Vector3.Normalize(-playerManager.armGO.transform.right);
                                        playerManager.ApplyFiringKnockback(direction, currentFiringKnockback);

                                        firingTimer = 1 / currentFirerate;
                                    }
                                }
                            }
                        }*/
                    }break;
            }
        }
        else // If we're not trying to fire.
        {
            if (currentWeapon == Weapon.MINIGUN)
            {
                switch (minigunSoundStage)
                {
                    case MinigunSoundStage.SPINNING_UP:
                        {
                            minigunSoundStage = MinigunSoundStage.STOPPED;
                        }break;
                    case MinigunSoundStage.FIRING:
                        {
                            minigunSoundStage = MinigunSoundStage.SLOWING_DOWN;
                            slowdownTimer = slowdownTime;
                        }
                        break;
                    case MinigunSoundStage.SLOWING_DOWN:
                        {
                            if (slowdownTimer < 0)
                            {
                                minigunSoundStage = MinigunSoundStage.STOPPED;
                            }
                        }
                        break;
                }
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
    private void Start()
    {
        spinupTime = SoundManager.Instance.minigunSpinupTime;
        slowdownTime = SoundManager.Instance.minigunSlowdownTime;
    }
    private void FixedUpdate()
    {
        Shoot();
    }
    private void Update()
    {
        firingTimer -= Time.deltaTime;
        bulletsSpeedupTimer -= Time.deltaTime;
        if (minigunSoundStage == MinigunSoundStage.SPINNING_UP)
        {
            spinupTimer -= Time.deltaTime;
        }
        if (minigunSoundStage == MinigunSoundStage.SLOWING_DOWN)
        {
            slowdownTimer -= Time.deltaTime;
        }
    }
    #endregion
}
