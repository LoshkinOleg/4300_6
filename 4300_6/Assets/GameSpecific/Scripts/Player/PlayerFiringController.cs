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
    [SerializeField] GameObject catridgePrefab = null;

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
                                }
                                break;
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
                    }
                    break;
                case MinigunSoundStage.STOPPED:
                    {
                        SoundManager.Instance.StopSoundWithFadeout("minigun_spinup");
                    }
                    break;
            }

            _minigunSoundStage = value;
        }
    }
    
    // Private variables
    Weapon _currentWeapon = Weapon.PISTOL;
    float currentProjectileSpeed;
    float currentFirerate;
    float currentSpread;
    int currentNumberOfProjectilesPerShot;
    float currentFiringKnockback;
    int _currentAmmo;
    bool _isSpeedup;
    float firingTimer;
    float bulletsSpeedupTimer;
    // Sound related
    bool isSpinningUp;
    bool isFiringMinigun;
    bool isSlowingDown;
    float spinupTimer;
    float spinupTime;
    float slowdownTimer;
    float slowdownTime;
    MinigunSoundStage _minigunSoundStage = MinigunSoundStage.STOPPED;
    bool isPlayingOutOfAmmo;
    float outOfAmmoTime;
    float outOfAmmoTimer;
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
            // Set up variables
            currentProjectileSpeed = playerManager.weaponsData[(int)value].projectileSpeed;
            currentFirerate = playerManager.weaponsData[(int)value].firerate;
            currentSpread = playerManager.weaponsData[(int)value].spread;
            currentNumberOfProjectilesPerShot = playerManager.weaponsData[(int)value].numberOfProjectiles;
            currentFiringKnockback = playerManager.weaponsData[(int)value].firingKnockback;
            currentAmmo = playerManager.weaponsData[(int)value].ammo;

            playerManager.UpdateCurrentWeaponSprite(value);
            minigunSoundStage = MinigunSoundStage.STOPPED;

            _currentWeapon = value;
        }
    }
    public int currentAmmo
    {
        get
        {
            return _currentAmmo;
        }
        private set
        {
            Color color;
            string text;
            int maximalNumberOfAmmo = playerManager.weaponsData[(int)currentWeapon].ammo;

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
            else if (value == 0)
            {
                color = Color.black;
                text = value.ToString();
            }
            else
            {
                color = Color.black;
                text = "*Clack!*";
            }
            GameManager.Instance.feedbackUIController.InstantiateText(gameObject, text, color);
            _currentAmmo = value;
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
        spinupTime = SoundManager.Instance.minigunSpinupTime;
        slowdownTime = SoundManager.Instance.minigunSlowdownTime;
        outOfAmmoTime = SoundManager.Instance.outOfAmmoTime;

    }
    #endregion

    // Private methods
    #region Private methods
    void Shoot()
    {
        if (playerManager.tryingToFire)
        {
            if (currentAmmo > 0)
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
                                // Decrement ammo count
                                currentAmmo--;

                                // Instantiate catridge
                                Instantiate(catridgePrefab, transform.position, new Quaternion());
                            }
                        }
                        break;
                    case Weapon.SHOTGUN:
                        {
                            if (firingTimer < 0)
                            {
                                for (int i = -currentNumberOfProjectilesPerShot / 2; i < currentNumberOfProjectilesPerShot / 2; i++)
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

                                currentAmmo--;

                                Instantiate(catridgePrefab, transform.position, new Quaternion());
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

                                Projectile newProjectile = Instantiate(bulletsPrefabs[1], transform.position, new Quaternion()).GetComponent<Projectile>();
                                newProjectile.speed = currentProjectileSpeed;
                                newProjectile.type = currentWeapon;

                                firingTimer = 1 / currentFirerate;

                                if (SoundManager.Instance != null)
                                {
                                    SoundManager.Instance.PlaySound("sniper_fire");
                                }

                                currentAmmo--;

                                Instantiate(catridgePrefab, transform.position, new Quaternion());
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

                                currentAmmo--;
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
                                    }
                                    break;
                                case MinigunSoundStage.SPINNING_UP:
                                    {
                                        if (spinupTimer < 0)
                                        {
                                            minigunSoundStage = MinigunSoundStage.FIRING;
                                        }
                                    }
                                    break;
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

                                            currentAmmo--;

                                            Instantiate(catridgePrefab, transform.position, new Quaternion());
                                        }
                                    }
                                    break;
                                case MinigunSoundStage.SLOWING_DOWN:
                                    {
                                        minigunSoundStage = MinigunSoundStage.FIRING;
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            else
            {
                if (firingTimer < 0)
                {
                    if (currentWeapon == Weapon.MINIGUN)
                    {
                        if (minigunSoundStage == MinigunSoundStage.FIRING)
                        {
                            minigunSoundStage = MinigunSoundStage.SLOWING_DOWN;
                        }
                    }

                    if (SoundManager.Instance != null)
                    {
                        if (!isPlayingOutOfAmmo)
                        {
                            SoundManager.Instance.PlaySound("out_of_ammo");
                            isPlayingOutOfAmmo = true;
                            outOfAmmoTimer = outOfAmmoTime;
                            firingTimer = 1 / currentFirerate;
                            currentAmmo--;
                        }
                    }
                }
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

            if (currentAmmo < 0)
            {
                currentWeapon = Weapon.PISTOL;
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
        if (minigunSoundStage == MinigunSoundStage.SPINNING_UP)
        {
            spinupTimer -= Time.deltaTime;
        }
        if (minigunSoundStage == MinigunSoundStage.SLOWING_DOWN)
        {
            slowdownTimer -= Time.deltaTime;
        }
        outOfAmmoTimer -= Time.deltaTime;
        if (outOfAmmoTimer < 0)
        {
            isPlayingOutOfAmmo = false;
        }
    }
    #endregion
}
