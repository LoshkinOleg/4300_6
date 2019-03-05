﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFiringController : MonoBehaviour
{
    // Classes and Enums
    public enum MinigunStage
    {
        STOPPED,
        SPINNING_UP,
        FIRING,
        SLOWING_DOWN
    }

    // Attributes
    #region Attributes
    // Set up variables
    [HideInInspector] public PlayerManager _playerManager = null;

    // Inspector variables
    [SerializeField] float hasRecentlyShotDuration = 0.5f;

    // Prefabs
    [SerializeField] GameObject bulletsPrefab = null;
    
    // Private variables
    Weapon _currentWeapon = Weapon.PISTOL;
    float currentProjectileSpeed;
    float currentFirerate;
    float currentSpread;
    int currentNumberOfProjectilesPerShot;
    float currentFiringKnockback;
    int _currentAmmo;
    float firingTimer;
    bool _hasRecentlyShot;
    float hasRecentlyShotTimer;
    // Speedup mechanic
    bool _isSpeedup;
    float bulletsSpeedupTimer;
    // Minigun related
    MinigunStage _currentMinigunStage = MinigunStage.STOPPED;
    bool isSpinningUp;
    bool isFiringMinigun;
    bool isSlowingDown;
    float spinupTimer;
    float spinupTime;
    float slowdownTimer;
    float slowdownTime;
    // Reload related
    bool isPlayingReloadingSound;
    #endregion

    // Public properties
    #region Public properties
    public PlayerManager PlayerManager
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
    public Weapon CurrentWeapon
    {
        get
        {
            return _currentWeapon;
        }
        set
        {
            _currentWeapon = value;

            // Remove speedup effect.
            bool needToReapplySpeedup = false;
            float timerToReapply = bulletsSpeedupTimer;
            if (IsSpeedup)
            {
                IsSpeedup = false;
                needToReapplySpeedup = true;
            }

            // Set up variables
            currentProjectileSpeed = PlayerManager.WeaponsData[(int)value].projectileSpeed;
            currentFirerate = PlayerManager.WeaponsData[(int)value].firerate;
            currentSpread = PlayerManager.WeaponsData[(int)value].spread;
            currentNumberOfProjectilesPerShot = PlayerManager.WeaponsData[(int)value].numberOfProjectiles;
            currentFiringKnockback = PlayerManager.WeaponsData[(int)value].firingKnockback;
            CurrentAmmo = PlayerManager.WeaponsData[(int)value].ammo;
            // Reset minigun stage.
            CurrentMinigunStage = MinigunStage.STOPPED;
            // Display appropriate weapon.
            PlayerManager.UpdateCurrentWeaponSprite();

            // Reapply speed up.
            if (needToReapplySpeedup)
            {
                _isSpeedup = true;
                bulletsSpeedupTimer = timerToReapply;
                currentProjectileSpeed *= PickupManager.Instance.SpeedupMultiplier;
                currentFirerate *= PickupManager.Instance.SpeedupMultiplier;
            }
        }
    }
    public int CurrentAmmo
    {
        get
        {
            return _currentAmmo;
        }
        private set
        {
            // Display remaining ammo feedback.
            if (CurrentWeapon != Weapon.PISTOL) // Exclude pistol since it has unlimited ammo.
            {
                Color color;
                string text;
                int maximalNumberOfAmmo = PlayerManager.WeaponsData[(int)CurrentWeapon].ammo;

                if (value > (maximalNumberOfAmmo / 3) * 2)
                {
                    color = Color.green;
                    text = value.ToString();
                }
                else if (value > maximalNumberOfAmmo / 3)
                {
                    color = Color.yellow;
                    text = value.ToString();
                }
                else if (value <= maximalNumberOfAmmo / 3 && value > 0)
                {
                    color = Color.red;
                    text = value.ToString();
                }
                else
                {
                    color = Color.black;
                    text = value.ToString();
                }
                PlayerManager.DisplayAmmoLeft(text, color);
            }

            _currentAmmo = value;
        }
    }
    public bool HasRecentlyShot => _hasRecentlyShot;
    public MinigunStage CurrentMinigunStage // Triggers the right minigun sound when assigned to.
    {
        get
        {
            return _currentMinigunStage;
        }
        private set
        {
            // Play according sound when going from state A to state B if allowed.
            switch (_currentMinigunStage)
            {
                case MinigunStage.STOPPED: // Going from STOPPED
                    {
                        if (value == MinigunStage.SPINNING_UP) // To SPINNING_UP
                        {
                            // In this case, can only play a sound if we're going from STOPPED to SPINNING_UP.
                            PlayerManager.PlayMinigunSound(value);
                        }
                    }
                    break;
                case MinigunStage.SPINNING_UP:
                    {
                        if (value == MinigunStage.STOPPED || value == MinigunStage.FIRING)
                        {
                            PlayerManager.PlayMinigunSound(value);
                        }
                    }
                    break;
                case MinigunStage.FIRING:
                    {
                        if (value == MinigunStage.STOPPED || value == MinigunStage.SLOWING_DOWN)
                        {
                            PlayerManager.PlayMinigunSound(value);
                        }
                    }
                    break;
                case MinigunStage.SLOWING_DOWN:
                    {
                        if (value == MinigunStage.STOPPED || value == MinigunStage.FIRING)
                        {
                            PlayerManager.PlayMinigunSound(value);
                        }
                    }
                    break;
            }

            _currentMinigunStage = value;
        }
    }
    public bool IsSpeedup
    {
        get
        {
            return _isSpeedup;
        }
        private set
        {
            if (value)
            {
                if (_isSpeedup)
                {
                    bulletsSpeedupTimer = PickupManager.Instance.SpeedupPickupTime;
                }
                else
                {
                    bulletsSpeedupTimer = PickupManager.Instance.SpeedupPickupTime;
                    currentProjectileSpeed *= PickupManager.Instance.SpeedupMultiplier;
                    currentFirerate *= PickupManager.Instance.SpeedupMultiplier;
                }

            }
            else
            {
                currentProjectileSpeed /= PickupManager.Instance.SpeedupMultiplier;
                currentFirerate /= PickupManager.Instance.SpeedupMultiplier;
            }
            _isSpeedup = value;
        }
    }
    #endregion

    // Public methods
    #region Public methods
    public void SpeedBulletsUp()
    {
        IsSpeedup = true;
        PlayerManager.HighlightPlayer(Color.cyan);
    }
    public void Init()
    {
        spinupTime = SoundManager.Instance.MinigunSpinupTime;
        slowdownTime = SoundManager.Instance.MinigunSlowdownTime;
    }
    #endregion

    // Private methods
    #region Private methods
    void Shoot(Weapon type)
    {
        if (type == Weapon.MINIGUN) // Handle minigun spinup mechanics
        {
            switch (CurrentMinigunStage)
            {
                case MinigunStage.STOPPED:
                    {
                        // Start spinning up and reset minigun stage related variables
                        CurrentMinigunStage = MinigunStage.SPINNING_UP;
                        spinupTimer = spinupTime;
                        slowdownTimer = slowdownTime;
                    }
                    break;
                case MinigunStage.SPINNING_UP:
                    {
                        // Can start firing if spinup sound has ended.
                        if (spinupTimer < 0)
                        {
                            CurrentMinigunStage = MinigunStage.FIRING;
                        }
                    }
                    break;
                case MinigunStage.FIRING:
                    {
                        float randomSpread = Random.Range(-currentSpread / 2, currentSpread / 2);
                        Quaternion rotation = PlayerManager.ArmsTransform.transform.rotation * Quaternion.Euler(0, 0, randomSpread);
                        Projectile newProjectile = Instantiate(bulletsPrefab, transform.position, rotation).GetComponent<Projectile>();
                        newProjectile.speed = currentProjectileSpeed;
                        newProjectile.type = CurrentWeapon;
                        newProjectile.visualFeedbackLifetime = PlayerManager.BulletDestructionFeedbackLifetime;
                        
                        firingTimer = 1 / currentFirerate;
                        CurrentAmmo--;

                        Vector2 direction = Vector3.Normalize(-PlayerManager.ArmsTransform.transform.right);
                        PlayerManager.ApplyFiringKnockback(direction, currentFiringKnockback);

                        PlayerManager.InstantiateWeaponCatridge();
                        PlayerManager.ShakeScreen();

                        _hasRecentlyShot = true;
                        hasRecentlyShotTimer = hasRecentlyShotDuration;

                        // Sound is managed by currentMinigunStage property.
                    }
                    break;
                case MinigunStage.SLOWING_DOWN:
                    {
                        // Go back to firing if we've pressed the firing button during minigun slowdown.
                        CurrentMinigunStage = MinigunStage.FIRING;
                    }
                    break;
            }
        }
        else
        {
            switch (type)
            {
                case Weapon.PISTOL:
                    {
                        // Instantiate bullet
                        float randomSpread = Random.Range(-currentSpread / 2, currentSpread / 2);
                        Quaternion rotation = PlayerManager.ArmsTransform.transform.rotation * Quaternion.Euler(0, 0, randomSpread);
                        Projectile newProjectile = Instantiate(bulletsPrefab, transform.position, rotation).GetComponent<Projectile>();
                        newProjectile.speed = currentProjectileSpeed;
                        newProjectile.type = CurrentWeapon;
                        newProjectile.visualFeedbackLifetime = PlayerManager.BulletDestructionFeedbackLifetime;
                    }
                    break;
                case Weapon.SHOTGUN:
                    {
                        // Instantiate pellets in an arc in front of the player.
                        for (int i = -currentNumberOfProjectilesPerShot / 2; i < currentNumberOfProjectilesPerShot / 2; i++)
                        {
                            float randomSpread = Random.Range(-currentSpread / 2, currentSpread / 2);
                            Quaternion rotation = PlayerManager.ArmsTransform.transform.rotation * Quaternion.Euler(0, 0, randomSpread);

                            Projectile newProjectile = Instantiate(bulletsPrefab, transform.position, rotation).GetComponent<Projectile>();
                            newProjectile.speed = currentProjectileSpeed;
                            newProjectile.type = CurrentWeapon;
                            newProjectile.visualFeedbackLifetime = PlayerManager.BulletDestructionFeedbackLifetime;
                        }
                    }
                    break;
                case Weapon.SNIPER:
                    {
                        // Instantiate sniper firing FX
                        Projectile newProjectile = Instantiate(bulletsPrefab, transform.position, PlayerManager.ArmsTransform.rotation).GetComponent<Projectile>();
                        newProjectile.speed = 0;
                        newProjectile.type = CurrentWeapon;
                        newProjectile.visualFeedbackLifetime = PlayerManager.SniperDestructionFeedbackLifetime;

                        // Cast a hitscan. Apply sniper hit mechanics if a player is hit.
                        if (PlayerManager.IsLeftPlayer)
                        {
                            GameObject hit = Physics2D.Raycast(PlayerManager.ArmsTransform.transform.position + PlayerManager.ArmsTransform.transform.right, PlayerManager.ArmsTransform.transform.right).collider.gameObject;
                            if (hit.tag == "Player2")
                            {
                                GameManager.Instance.Players[1].SniperHit();
                            }
                            else if (hit.tag == "Player2Shield")
                            {
                                hit.GetComponent<Shield>().Hit(PlayerManager.WeaponsData[2].damage);
                            }
                        }
                        else
                        {
                            GameObject hit = Physics2D.Raycast(PlayerManager.ArmsTransform.transform.position + PlayerManager.ArmsTransform.transform.right, PlayerManager.ArmsTransform.transform.right).collider.gameObject;
                            if (hit.tag == "Player1")
                            {
                                GameManager.Instance.Players[0].SniperHit();
                            }
                            else if (hit.tag == "Player1Shield")
                            {
                                hit.GetComponent<Shield>().Hit(PlayerManager.WeaponsData[2].damage);
                            }
                        }
                    }
                    break;
                case Weapon.BAZOOKA:
                    {
                        // Instantiate a projectile without any spread applied.
                        Projectile newProjectile = Instantiate(bulletsPrefab, transform.position, PlayerManager.ArmsTransform.rotation).GetComponent<Projectile>();
                        newProjectile.speed = currentProjectileSpeed;
                        newProjectile.type = CurrentWeapon;
                        newProjectile.visualFeedbackLifetime = PlayerManager.BazookaDestructionFeedbackLifetime;
                    }
                    break;
            }

            // Modify firing variables
            firingTimer = 1 / currentFirerate;
            CurrentAmmo--;

            // Apply firing knockback
            _hasRecentlyShot = true;
            hasRecentlyShotTimer = hasRecentlyShotDuration;

            Vector2 direction = Vector3.Normalize(-PlayerManager.ArmsTransform.transform.right);
            PlayerManager.ApplyFiringKnockback(direction, currentFiringKnockback);

            // Trigger feedbacks
            PlayerManager.InstantiateWeaponCatridge();
            PlayerManager.PlayFiringSound();
            PlayerManager.ShakeScreen();
        }
    }
    void ProcessShooting()
    {
        if (PlayerManager.TryingToFire) // If we've pressed the firing button
        {
            if (firingTimer < 0) // We've reloaded.
            {
                if (CurrentAmmo > 0) // We've got ammo.
                {
                    Shoot(CurrentWeapon);
                }
                else // We're out of ammo but trying to fire.
                {
                    // If we were firing our minigun last update, change minigun stage.
                    if (CurrentWeapon == Weapon.MINIGUN)
                    {
                        if (CurrentMinigunStage == MinigunStage.FIRING)
                        {
                            CurrentMinigunStage = MinigunStage.SLOWING_DOWN;
                        }
                    }
                    firingTimer = 1 / currentFirerate;
                    // Play out of ammo audio feedback.
                    PlayerManager.DisplayOutOfAmmoFeedbacks();
                }
            }

        }
        else // If we're not trying to fire.
        {
            if (CurrentWeapon == Weapon.MINIGUN) // Handle minigun mechanics.
            {
                switch (CurrentMinigunStage)
                {
                    case MinigunStage.SPINNING_UP:
                        {
                            // Reset to STOPPED state if current state was SPINNING_UP.
                            CurrentMinigunStage = MinigunStage.STOPPED;
                        }break;
                    case MinigunStage.FIRING:
                        {
                            // Start slowing down and reset slowing down variables.
                            CurrentMinigunStage = MinigunStage.SLOWING_DOWN;
                            slowdownTimer = slowdownTime;
                        }
                        break;
                    case MinigunStage.SLOWING_DOWN:
                        {
                            // Stop minigun if the slowdown stage has ended.
                            if (slowdownTimer < 0)
                            {
                                CurrentMinigunStage = MinigunStage.STOPPED;
                            }
                        }
                        break;
                    case MinigunStage.STOPPED:
                        {
                            // Switch to pistol if we're out of ammo.
                            if (CurrentAmmo <= 0)
                            {
                                CurrentWeapon = Weapon.PISTOL;
                            }
                        }
                        break;
                }
            }
            else if (CurrentAmmo <= 0) // Switch to pistol if we're out of ammo.
            {
                CurrentWeapon = Weapon.PISTOL;
            }
        }

        // Update muzzle flash sprite.
        if (CurrentWeapon != Weapon.MINIGUN)
        {
            PlayerManager.UpdateMuzzleFlash();
        }
        else
        {
            if (CurrentMinigunStage != MinigunStage.SPINNING_UP) // Prevents the muzzle flash from being displayed during spinup
            {
                PlayerManager.UpdateMuzzleFlash();
            }
        }
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Update()
    {
        ProcessShooting();

        // Update timers and timer related variables.
        firingTimer -= Time.deltaTime;
        bulletsSpeedupTimer -= Time.deltaTime;
        hasRecentlyShotTimer -= Time.deltaTime;

        if (CurrentMinigunStage == MinigunStage.SPINNING_UP)
        {
            spinupTimer -= Time.deltaTime;
        }
        else if (CurrentMinigunStage == MinigunStage.SLOWING_DOWN)
        {
            slowdownTimer -= Time.deltaTime;
        }

        if (bulletsSpeedupTimer < 0)
        {
            if (IsSpeedup)
            {
                IsSpeedup = false;
            }
        }

        if (hasRecentlyShotTimer < 0)
        {
            _hasRecentlyShot = false;
        }

        if (firingTimer > 0 && firingTimer - SoundManager.Instance.ReloadTime <= 0)
        {
            if (CurrentWeapon != Weapon.PISTOL && CurrentWeapon != Weapon.MINIGUN)
            {
                if (!isPlayingReloadingSound)
                {
                    isPlayingReloadingSound = true;
                    PlayerManager.DisplayReloadingFeedbacks();
                }
            }
        }
        else if (firingTimer < 0)
        {
            isPlayingReloadingSound = false;
        }
    }
    #endregion
}
