﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeedbackController : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Setup variables
    [HideInInspector] public PlayerManager _playerManager;

    // Inspector variables
    [SerializeField] Vector3[] frontArmPositions = new Vector3[(int)Weapon.MINIGUN + 1];
    [SerializeField] Vector3[] backArmPositions = new Vector3[(int)Weapon.MINIGUN + 1];
    [SerializeField] Vector3[] firingAndReloadingFX_positions = new Vector3[(int)Weapon.MINIGUN + 1];
    [SerializeField] float firingAndReloadingFX_lifetime = 0.3f;
    [SerializeField] float[] catridgeSpinForceRange = new float[2];
    [SerializeField] float[] catridgeEjectionForceRange = new float[2];
    [SerializeField] float ammoLeftFeedback_horizontalOffset = 0.5f;
    [SerializeField] float ammoLeftFeedback_verticalOffset = 0.5f;
    [SerializeField] int ammoLeftFeedback_numbersFontSize = 4;
    [SerializeField] int ammoLeftFeedback_wordsFontSize = 2;
    [SerializeField] float ammoLeftFeedback_lifetime = 1f;
    [SerializeField] float _bulletDestructionFeedbackLifetime = 0.5f;
    [SerializeField] float _sniperDestructionFeedbackLifetime = 0.5f;
    [SerializeField] float _bazookaDestructionFeedbackLifetime = 0.5f;
    [SerializeField] float parachuteDeployementTime = 0.2f;
    [SerializeField] float stunEffectVerticalOffset = 0.6f;

    // References
    [SerializeField] SpriteRenderer parachute_SpriteRenderer = null;
    [SerializeField] SpriteRenderer frontArm_SpriteRenderer = null;
    [SerializeField] SpriteRenderer backArm_SpriteRenderer = null;
    [SerializeField] SpriteRenderer firingAndReloadingFX_SpriteRenderer = null;
    [SerializeField] Sprite blank_Sprite = null;
    [SerializeField] Sprite[] frontArm_Sprites = new Sprite[(int)Weapon.MINIGUN + 1]; // 0: pistol, 1: shotgun, 2: sniper, 3: bazooka, 4: minigun
    [SerializeField] Sprite[] backArm_Sprites = new Sprite[2]; // 0: straight arm, 1: minigun holding arm
    [SerializeField] Sprite[] reloadingArm_Sprites = new Sprite[3]; // 0: shotgun, 1: sniper, 2: bazooka
    [SerializeField] Sprite[] firingAndReloadingFX_Sprites = new Sprite[2]; // 0: muzzle flash, 1: reload sparkle
    [SerializeField] Sprite[] parachute_Sprites = new Sprite[2]; // 0: opened, 1: in transition
    [SerializeField] Sprite[] catridge_Sprites = new Sprite[2]; // 0: pistol/minigun, 1: shotgun/sniper

    // Prefabs
    [SerializeField] GameObject catridgePrefab = null;
    [SerializeField] GameObject ammoLeftPrefab = null;
    [SerializeField] GameObject stunEffectPrefab = null;

    // Private variables
    float muzzleFlashTimer;
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
    public float BulletDestructionFeedbackLifetime => _bulletDestructionFeedbackLifetime;
    public float SniperDestructionFeedbackLifetime => _sniperDestructionFeedbackLifetime;
    public float BazookaDestructionFeedbackLifetime => _bazookaDestructionFeedbackLifetime;
    #endregion

    // Public methods
    #region Public methods
    public void Init()
    {
        firingAndReloadingFX_SpriteRenderer.sprite = blank_Sprite;
        parachute_SpriteRenderer.sprite = blank_Sprite;
    }
    public void DisplayStunEffect(float duration)
    {
        StunEffectController newStunEffect = Instantiate(stunEffectPrefab, gameObject.transform.position + new Vector3(0, stunEffectVerticalOffset, 0), new Quaternion()).GetComponent<StunEffectController>();
        newStunEffect.target = gameObject.transform;
        newStunEffect.lifetime = duration;
        newStunEffect.verticalOffset = stunEffectVerticalOffset;
    }
    public void DisplayAmmoLeft(string ammoLeft, Color color)
    {
        Instantiate(ammoLeftPrefab).GetComponent<AmmoLeftTextController>().Init(GameManager.Instance.FloatingTextFeedbackUI.transform, gameObject.transform, ammoLeftFeedback_horizontalOffset, ammoLeftFeedback_verticalOffset, ammoLeft, color, ammoLeftFeedback_wordsFontSize, ammoLeftFeedback_numbersFontSize, ammoLeftFeedback_lifetime);
    }
    public void DisplayReloadingFeedbacks()
    {
        // Change arms to reloading sprites.
        switch (PlayerManager.CurrentWeapon)
        {
            case Weapon.SHOTGUN:
                {
                    frontArm_SpriteRenderer.sprite = reloadingArm_Sprites[0];
                }
                break;
            case Weapon.SNIPER:
                {
                    frontArm_SpriteRenderer.sprite = reloadingArm_Sprites[1];
                }
                break;
            case Weapon.BAZOOKA:
                {
                    frontArm_SpriteRenderer.sprite = reloadingArm_Sprites[2];
                }
                break;
        }

        // Play sound and display reloading sparkle.
        SoundManager.Instance.PlaySound("shotgun_reloading");
        StartCoroutine(DisplayReloadingSparkleAndResetArms());
    }
    public void PlayFiringSound()
    {
        if (SoundManager.Instance != null)
        {
            switch (PlayerManager.CurrentWeapon)
            {
                case Weapon.PISTOL:
                    {
                        SoundManager.Instance.PlaySound("pistol_fire");
                    }
                    break;
                case Weapon.SHOTGUN:
                    {
                        SoundManager.Instance.PlaySound("shotgun_fire");
                    }
                    break;
                case Weapon.SNIPER:
                    {
                        SoundManager.Instance.PlaySound("sniper_fire");
                    }
                    break;
                case Weapon.BAZOOKA:
                    {
                        SoundManager.Instance.PlaySound("bazooka_fire");
                    }
                    break;
            }
        }
    }
    public void PlayMinigunSound()
    {
        if (SoundManager.Instance != null)
        {
            switch (PlayerManager.CurrentMinigunStage)
            {
                case PlayerFiringController.MinigunStage.STOPPED:
                    {
                        SoundManager.Instance.StopSound("minigun_spinup");
                        SoundManager.Instance.StopSound("minigun_fire");
                        SoundManager.Instance.StopSound("minigun_slowdown");
                    }
                    break;
                case PlayerFiringController.MinigunStage.SPINNING_UP:
                    {
                        SoundManager.Instance.PlaySound("minigun_spinup");
                    }
                    break;
                case PlayerFiringController.MinigunStage.FIRING:
                    {
                        SoundManager.Instance.PlaySound("minigun_fire");
                    }
                    break;
                case PlayerFiringController.MinigunStage.SLOWING_DOWN:
                    {
                        SoundManager.Instance.StopSound("minigun_fire");
                        SoundManager.Instance.PlaySound("minigun_slowdown");
                    }
                    break;
            }
        }

    }
    public void DisplayOutOfAmmoFeedbacks()
    {
        SoundManager.Instance.PlayOutOfAmmoSound();
        DisplayAmmoLeft("*Clack!*", Color.black);
    }
    public void ToggleParachute()
    {
        if (parachute_SpriteRenderer.sprite == blank_Sprite) // Parachute closed
        {
            parachute_SpriteRenderer.sprite = parachute_Sprites[1];
            StartCoroutine(DisplayParachuteSpriteAfterSeconds(parachute_Sprites[0]));
        }
        else // Parachute open
        {
            parachute_SpriteRenderer.sprite = parachute_Sprites[1];
            StartCoroutine(DisplayParachuteSpriteAfterSeconds(blank_Sprite));
        }
    }
    public void InstantiateWeaponCatridge()
    {
        if (PlayerManager.CurrentWeapon != Weapon.BAZOOKA)
        {
            GameObject newCatridge = Instantiate(catridgePrefab, gameObject.transform.position, new Quaternion());
            Rigidbody2D rigidbody = newCatridge.GetComponent<Rigidbody2D>();
            SpriteRenderer renderer = newCatridge.GetComponent<SpriteRenderer>();
            rigidbody.gravityScale = 0;
            rigidbody.angularVelocity = Random.Range(catridgeSpinForceRange[0], catridgeSpinForceRange[1]);
            rigidbody.velocity = new Vector2(Random.Range(catridgeEjectionForceRange[0], catridgeEjectionForceRange[1]), Random.Range(catridgeEjectionForceRange[0], catridgeEjectionForceRange[1]));

            if (PlayerManager.CurrentWeapon == Weapon.PISTOL ||
                PlayerManager.CurrentWeapon == Weapon.MINIGUN)
            {
                renderer.sprite = catridge_Sprites[0];
            }
            else
            {
                renderer.sprite = catridge_Sprites[1];
            }
        }
    }
    public void UpdateCurrentWeaponSprite()
    {
        // Update positions.
        PlayerManager.FrontArmTransform.localPosition = frontArmPositions[(int)PlayerManager.CurrentWeapon];
        PlayerManager.BackArmTransform.localPosition = backArmPositions[(int)PlayerManager.CurrentWeapon];
        PlayerManager.FiringAndReloadingFX_Transform.localPosition = firingAndReloadingFX_positions[(int)PlayerManager.CurrentWeapon];

        // Display the right sprites.
        frontArm_SpriteRenderer.sprite = frontArm_Sprites[(int)PlayerManager.CurrentWeapon]; // Front arm
        // Back arm
        if (PlayerManager.CurrentWeapon == Weapon.MINIGUN)
        {
            backArm_SpriteRenderer.sprite = backArm_Sprites[1];
        }
        else if (PlayerManager.CurrentWeapon == Weapon.SHOTGUN ||
                 PlayerManager.CurrentWeapon == Weapon.SNIPER)
        {
            backArm_SpriteRenderer.sprite = backArm_Sprites[0];
        }
        else
        {
            backArm_SpriteRenderer.sprite = blank_Sprite;
        }
    }
    public void UpdateMuzzleFlash()
    {
        if (PlayerManager.TryingToFire && PlayerManager.CurrentAmmo > 0 && PlayerManager.HasRecentlyShot)
        {
            // Set muzzle flash timer to start counting down since we're firing bullets that we have.
            muzzleFlashTimer = firingAndReloadingFX_lifetime;

            // Switch sprite if it's the first run of this function during this firing session.
            if (muzzleFlashTimer > 0)
            {
                if (firingAndReloadingFX_SpriteRenderer.sprite == firingAndReloadingFX_Sprites[0])
                {
                    firingAndReloadingFX_SpriteRenderer.sprite = firingAndReloadingFX_Sprites[1];
                }
            }
        }
        else // If we're not trying to fire the ammo we have or if we're out of ammo.
        {
            // Let the timer run out, then switch sprite back to blank sprite.
            if (muzzleFlashTimer <= 0)
            {
                if (firingAndReloadingFX_SpriteRenderer.sprite == firingAndReloadingFX_Sprites[1])
                {
                    firingAndReloadingFX_SpriteRenderer.sprite = firingAndReloadingFX_Sprites[0];
                }
            }
        }
    }
    public void ShakeScreen()
    {
        GameManager.Instance.ScreenShake.ShakeScreen(PlayerManager.CurrentWeapon);
    }
    #endregion

    // Private methods
    #region Private methods
    IEnumerator DisplayParachuteSpriteAfterSeconds(Sprite sprite)
    {
        yield return new WaitForSeconds(parachuteDeployementTime);
        parachute_SpriteRenderer.sprite = sprite;
    }
    IEnumerator ResetFiringAndReloadingFXSprite()
    {
        yield return new WaitForSeconds(firingAndReloadingFX_lifetime);
        firingAndReloadingFX_SpriteRenderer.sprite = blank_Sprite;
    }
    IEnumerator DisplayReloadingSparkleAndResetArms()
    {
        yield return new WaitForSeconds(SoundManager.Instance.reloadTime);

        // Display reloading sparkle and start coroutine to reset it back to invisible after some time.
        firingAndReloadingFX_SpriteRenderer.sprite = firingAndReloadingFX_Sprites[1];
        StartCoroutine(ResetFiringAndReloadingFXSprite());

        // Reset arms
        UpdateCurrentWeaponSprite();
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Update()
    {
        muzzleFlashTimer -= Time.deltaTime;
    }
    #endregion
}
