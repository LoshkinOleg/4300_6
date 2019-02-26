using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeedbackController : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Setup variables
    public PlayerManager _playerManager;

    // Inspector variables
    [SerializeField] Vector3[] frontArmPositions = new Vector3[(int)Weapon.MINIGUN + 1];
    [SerializeField] Vector3[] backArmPositions = new Vector3[(int)Weapon.MINIGUN + 1];
    [SerializeField] Vector3[] firingAndReloadingFX_positions = new Vector3[(int)Weapon.MINIGUN + 1];
    [SerializeField] float firingAndReloadingFX_lifetime = 0.3f;
    [SerializeField] float[] catridgeSpinForceRange = new float[2];
    [SerializeField] float[] catridgeEjectionForceRange = new float[2];
    [SerializeField] [Range(0, 2)] float[] firingScreenShakeIntensities = new float[(int)Weapon.MINIGUN + 1];
    [SerializeField] float ammoLeftFeedback_horizontalOffset = 0.5f;
    [SerializeField] float ammoLeftFeedback_verticalOffset = 0.5f;
    [SerializeField] int ammoLeftFeedback_numbersFontSize = 4;
    [SerializeField] int ammoLeftFeedback_wordsFontSize = 2;
    [SerializeField] float bulletDestructionFeedbackLifetime = 0.5f;
    [SerializeField] float sniperDestructionFeedbackLifetime = 0.5f;
    [SerializeField] float bazookaDestructionFeedbackLifetime = 0.5f;
    [SerializeField] float parachuteDeployementTime = 0.2f;
    [SerializeField] float stunEffectVerticalOffset = 0.6f;

    // References
    [SerializeField] SpriteRenderer parachute_SpriteRenderer = null;
    [SerializeField] SpriteRenderer frontArm_SpriteRenderers = null;
    [SerializeField] SpriteRenderer backArm_SpriteRenderers = null;
    [SerializeField] SpriteRenderer firingAndReloadingFX_SpriteRenderer = null;
    [SerializeField] Sprite blank_Sprite = null;
    [SerializeField] Sprite[] frontArm_Sprites = new Sprite[(int)Weapon.MINIGUN + 1]; // 0: pistol, 1: shotgun, 2: sniper, 3: bazooka, 4: minigun
    [SerializeField] Sprite[] backArm_Sprites = new Sprite[2]; // 0: straight arm, 1: minigun holding arm
    [SerializeField] Sprite[] reloadingArm_Sprites = new Sprite[3]; // 0: shotgun, 1: sniper, 2: bazooka
    [SerializeField] Sprite[] projectile_Sprites = new Sprite[4]; // 0: pistol and minigun, 1: shotgun, 2: sniper, 3: bazooka
    [SerializeField] Sprite[] projectileDestruction_Sprites = new Sprite[3]; // 0: pistol, shotgun and minigun, 1: sniper, 2: bazooka
    [SerializeField] Sprite[] firingAndReloadingFX_Sprites = new Sprite[2]; // 0: muzzle flash, 1: reload sparkle
    [SerializeField] Sprite[] parachute_Sprites = new Sprite[2]; // 0: opened, 1: in transition

    // Prefabs
    [SerializeField] GameObject[] catridgePrefabs = new GameObject[2];
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
    #endregion

    // Public methods
    #region Public methods
    public void Init()
    {
        firingAndReloadingFX_SpriteRenderer.sprite = blank_Sprite;
        parachute_SpriteRenderer.sprite = blank_Sprite;
    }
    public void HighlightHealthBar()
    {

    }
    public void DisplayStunEffect(float duration)
    {
        StunEffectController newStunEffect = Instantiate(stunEffectPrefab, caller.transform.position + new Vector3(0, StunEffectVerticalOffset, 0), new Quaternion()).GetComponent<StunEffectController>();
        newStunEffect.target = caller.transform;
        newStunEffect.lifetime = duration;
        newStunEffect.verticalOffset = StunEffectVerticalOffset;
    }
    public void DisplayAmmoLeft(string ammoLeft, Color color)
    {
        Instantiate(ammoLeftPrefab).GetComponent<AmmoLeftTextController>().Init(floatingTextFeedbackUI.transform, caller.transform, ammoLeftFeedback_horizontalOffset, ammoLeftFeedback_verticalOffset, ammoLeft, color, ammoLeftFeedback_wordsFontSize, ammoLeftFeedback_numbersFontSize, lifetime);
    }
    public void DisplayBulletDestruction()
    {
        renderer.sprite = projectileDestruction_Sprites[(int)type];
    }
    public void DisplayAppropriateProjectile()
    {
        renderer.sprite = projectile_Sprites[(int)type];
    }
    public void DisplayReloadingFeedbacks()
    {
        // Change arms to reloading sprites.
        int player = caller.IsLeftPlayer ? 0 : 1;
        switch (caller.CurrentWeapon)
        {
            case Weapon.SHOTGUN:
                {
                    frontArm_SpriteRenderers[player].sprite = reloadingArm_Sprites[0];
                }
                break;
            case Weapon.SNIPER:
                {
                    frontArm_SpriteRenderers[player].sprite = reloadingArm_Sprites[1];
                }
                break;
            case Weapon.BAZOOKA:
                {
                    frontArm_SpriteRenderers[player].sprite = reloadingArm_Sprites[2];
                }
                break;
        }

        // Play sound and display reloading sparkle.
        SoundManager.Instance.PlaySound("shotgun_reloading");
        StartCoroutine(DisplayReloadingSparkleAndResetArms(caller));
    }
    public void PlayFiringSound()
    {
        if (SoundManager.Instance != null)
        {
            switch (type)
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
            switch (stage)
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
        if (SoundManager.Instance != null) SoundManager.Instance.PlayOutOfAmmoSound(); else Debug.LogWarning("Variable not set up!");
        DisplayAmmoLeft(caller, "*Clack!*", Color.black, 1f);
    }
    public void ToggleParachute()
    {
        int player;
        if (caller.gameObject.tag == "Player1")
        {
            player = 0;
        }
        else
        {
            player = 1;
        }

        if (parachute_SpriteRenderer[player].sprite == parachute_Sprites[0]) // Parachute closed
        {
            parachute_SpriteRenderer[player].sprite = parachute_Sprites[1];
            StartCoroutine(DisplayParachuteSpriteAfterSeconds(parachute_SpriteRenderer[player], parachute_Sprites[2]));
            // parachuteDeployementTimer[player] = parachuteDeployementTime;
        }
        /*else if (parachutesSpriteRenderers[player].sprite == parachuteSprites[1]) // In transition
        {
            if (parachuteDeployementTimer[player] < 0)
            {
                if (caller.ParachuteIsOpen) // Closing parachute
                {
                    parachutesSpriteRenderers[player].sprite = parachuteSprites[0];
                }
                else // Opening parachute
                {
                    parachutesSpriteRenderers[player].sprite = parachuteSprites[2];
                }
            }
        }*/
        else // Parachute open
        {
            parachute_SpriteRenderer[player].sprite = parachute_Sprites[1];
            StartCoroutine(DisplayParachuteSpriteAfterSeconds(parachute_SpriteRenderer[player], parachute_Sprites[0]));
            // parachuteDeployementTimer[player] = parachuteDeployementTime;
        }
    }
    public void InstantiateWeaponCatridge()
    {
        Rigidbody2D catridgeRigidbody = Instantiate(catridgePrefabs[(int)type], caller.transform.position, new Quaternion()).GetComponent<Rigidbody2D>();
        catridgeRigidbody.gravityScale = 0;
        catridgeRigidbody.angularVelocity = Random.Range(catridgeSpinForceRange[0], catridgeSpinForceRange[1]);
        catridgeRigidbody.velocity = new Vector2(Random.Range(catridgeEjectionForceRange[0], catridgeEjectionForceRange[1]), Random.Range(catridgeEjectionForceRange[0], catridgeEjectionForceRange[1]));
    }
    public void RocketFeedback()
    {
        ShakeScreen(Weapon.BAZOOKA);
        SoundManager.Instance.PlaySound("bazooka_hit");
    }
    public void ShakeScreen()
    {
        GameManager.Instance.ScreenShake.ShakeScreen(firingScreenShakeIntensities[(int)type]);
    }
    public void UpdateCurrentWeaponSprite()
    {
        int player = caller.IsLeftPlayer ? 0 : 1;
        // Update positions.
        caller.FrontArmTransform.localPosition = frontArmPositions[(int)caller.CurrentWeapon];
        caller.BackArmTransform.localPosition = backArmPositions[(int)caller.CurrentWeapon];
        caller.FiringAndReloadingFX_Transform.localPosition = firingAndReloadingFX_positions[(int)caller.CurrentWeapon];

        // Display the right sprites.
        // Front arm
        if (player == 0)
        {
            frontArm_SpriteRenderers[player].sprite = frontArm_Sprites[(int)caller.CurrentWeapon];
        }
        else
        {
            frontArm_SpriteRenderers[player].sprite = terrorist_frontArm_Sprites[(int)caller.CurrentWeapon];
        }
        // Back arm
        if (caller.CurrentWeapon == Weapon.MINIGUN)
        {
            if (player == 0)
            {
                backArm_SpriteRenderers[player].sprite = backArm_Sprites[1];
            }
            else
            {
                backArm_SpriteRenderers[player].sprite = terrorist_backArm_Sprites[1];
            }
        }
        else if (caller.CurrentWeapon == Weapon.SHOTGUN ||
                 caller.CurrentWeapon == Weapon.SNIPER)
        {
            if (player == 0)
            {
                backArm_SpriteRenderers[player].sprite = backArm_Sprites[0];
            }
            else
            {
                backArm_SpriteRenderers[player].sprite = terrorist_backArm_Sprites[0];
            }
        }
        else
        {
            backArm_SpriteRenderers[player].sprite = blank_Sprite;
        }
    }
    public void UpdateMuzzleFlash()
    {
        int player = caller.IsLeftPlayer ? 0 : 1;
        if (caller.TryingToFire && caller.CurrentAmmo > 0 && caller.HasRecentlyShot)
        {
            // Set muzzle flash timer to start counting down since we're firing bullets that we have.
            muzzleFlashTimer[player] = firingAndReloadingFX_lifetime;

            // Switch sprite if it's the first run of this function during this firing session.
            if (muzzleFlashTimer[player] > 0)
            {
                if (firingAndReloadingFX_SpriteRenderer[player].sprite == firingAndReloadingFX_Sprites[0])
                {
                    firingAndReloadingFX_SpriteRenderer[player].sprite = firingAndReloadingFX_Sprites[1];
                }
            }
        }
        else // If we're not trying to fire the ammo we have or if we're out of ammo.
        {
            // Let the timer run out, then switch sprite back to blank sprite.
            if (muzzleFlashTimer[player] <= 0)
            {
                if (firingAndReloadingFX_SpriteRenderer[player].sprite == firingAndReloadingFX_Sprites[1])
                {
                    firingAndReloadingFX_SpriteRenderer[player].sprite = firingAndReloadingFX_Sprites[0];
                }
            }
        }
    }
    #endregion

    // Private methods
    #region Private methods
    IEnumerator DisplayParachuteSpriteAfterSeconds(SpriteRenderer renderer, Sprite sprite)
    {
        yield return new WaitForSeconds(parachuteDeployementTime);
        renderer.sprite = sprite;
    }
    IEnumerator ResetFiringAndReloadingFXSprite(int player)
    {
        yield return new WaitForSeconds(firingAndReloadingFX_lifetime);
        firingAndReloadingFX_SpriteRenderer[player].sprite = blank_Sprite;
    }
    IEnumerator DisplayReloadingSparkleAndResetArms(PlayerManager caller)
    {
        yield return new WaitForSeconds(SoundManager.Instance.reloadTime);

        // Display reloading sparkle and start coroutine to reset it back to invisible after some time.
        int player = caller.IsLeftPlayer ? 0 : 1;
        firingAndReloadingFX_SpriteRenderer[player].sprite = firingAndReloadingFX_Sprites[1];
        StartCoroutine(ResetFiringAndReloadingFXSprite(player));

        // Reset arms
        UpdateCurrentWeaponSprite(caller);
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
