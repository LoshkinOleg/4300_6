using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FeedbackManager : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] Vector3[] muzzleFlashPositions = new Vector3[(int)Weapon.MINIGUN +1];
    [SerializeField] float[] catridgeSpinRange = new float[2];
    [SerializeField] float[] catridgeEjectionForceRange = new float[2];
    [SerializeField] float outOfAmmoFeedback_horizontalOffset = 0.5f;
    [SerializeField] float outOfAmmoFeedback_verticalOffset = 0.5f;
    [SerializeField] int outOfAmmoTextSize_numbers = 4;
    [SerializeField] int outOfAmmoTextSize_words = 2;
    [SerializeField] float outOfAmmoFeedback_lifetime = 1f;
    [SerializeField] [Range(0, 2)] float[] screenShakeIntensities = new float[(int)Weapon.MINIGUN + 1];
    [SerializeField] float _bulletDestructionFeedbackLifetime = 0.5f;
    [SerializeField] float _sniperDestructionFeedbackLifetime = 0.5f;
    [SerializeField] float _bazookaDestructionFeedbackLifetime = 0.5f;

    // References
    [SerializeField] SpriteRenderer[] parachutesSpriteRenderers = new SpriteRenderer[2];
    [SerializeField] SpriteRenderer[] weaponSpriteRenderers = new SpriteRenderer[2];
    [SerializeField] SpriteRenderer[] muzzleFlashSpriteRenderers = new SpriteRenderer[2];
    [SerializeField] TMP_Text[] livesText = new TMP_Text[2];
    [SerializeField] Image[] healthBarImages = new Image[2];
    [SerializeField] Sprite[] weaponSprites = new Sprite[(int)Weapon.MINIGUN + 1];
    [SerializeField] Sprite[] projectileSprites = new Sprite[(int)Weapon.MINIGUN + 1];
    [SerializeField] Sprite[] destructionSprites = new Sprite[(int)Weapon.MINIGUN + 1];

    // Prefabs
    [SerializeField] GameObject stunEffectPrefab = null;
    [SerializeField] GameObject[] catridgePrefabs = new GameObject[(int)Weapon.MINIGUN + 1];
    [SerializeField] GameObject ammoLeftPrefab = null;

    // Public properties
    public static FeedbackManager Instance => _instance;
    public float BulletDestructionFeedbackLifetime => _bulletDestructionFeedbackLifetime;
    public float SniperDestructionFeedbackLifetime => _sniperDestructionFeedbackLifetime;
    public float BazookaDestructionFeedbackLifetime => _bazookaDestructionFeedbackLifetime;

    // Private variables
    static FeedbackManager _instance = null;
    #endregion

    // Public methods
    #region Public methods
    public void DisplayAmmoLeft(GameObject caller)
    {

    }
    public void DisplayMuzzleFlash(GameObject caller)
    {
        PlayerManager playerManager = caller.GetComponent<PlayerManager>();

        if (playerManager.TryingToFire && playerManager.CurrentAmmo > 0)
        {
            if (caller.tag == "Player1")
            {
                if (!muzzleFlashGOs[0].activeSelf)
                {
                    muzzleFlashGOs[0].SetActive(true);
                }
            }
            else
            {
                if (!muzzleFlashGOs[1].activeSelf)
                {
                    muzzleFlashGOs[1].SetActive(true);
                }
            }

        }
        else
        {
            if (caller.tag == "Player1")
            {
                if (muzzleFlashGOs[0].activeSelf)
                {
                    muzzleFlashGOs[0].SetActive(false);
                }
            }
            else
            {
                if (muzzleFlashGOs[1].activeSelf)
                {
                    muzzleFlashGOs[1].SetActive(false);
                }
            }

        }
    }
    public void DisplayStunEffect(GameObject caller, float duration)
    {
        StunEffectController newStunEffect = Instantiate(stunEffectPrefab).GetComponent<StunEffectController>();
        newStunEffect.target = caller;
        newStunEffect.lifetime = duration;
    }
    public void DisplayAmmoLeft(GameObject caller, string ammoLeft, Color color)
    {
        Instantiate(ammoLeftPrefab).GetComponent<AmmoLeftTextController>().Init(gameObject.transform, caller.transform, outOfAmmoFeedback_horizontalOffset, outOfAmmoFeedback_verticalOffset, ammoLeft, color, outOfAmmoTextSize_words, outOfAmmoTextSize_numbers);
    }
    public void DisplayHitByProjectile(GameObject playerHit)
    {

    }
    public void DisplayBulletDestruction(Vector3 position, Weapon type, SpriteRenderer renderer)
    {

    }
    public void DisplayAppropriateProjectile(SpriteRenderer renderer, Weapon type)
    {
        renderer.sprite = projectileSprites[(int)type];
    }
    public void PlayFiringSound(Weapon type)
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
    public void PlayMinigunSound(PlayerFiringController.MinigunStage stage)
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
                    SoundManager.Instance.PlaySound("minigun_slowdown");
                }
                break;
        }
    }
    public void PlayOutOfAmmoSound()
    {
        if (SoundManager.Instance != null)
        {
            if (!isPlayingOutOfAmmo)
            {
                SoundManager.Instance.PlaySound("out_of_ammo");
                isPlayingOutOfAmmo = true;
                outOfAmmoTimer = outOfAmmoTime;
                firingTimer = 1 / currentFirerate;
            }
        }
    }
    public void ToggleParachute(GameObject caller)
    {
        if (caller.tag == "Player1")
        {
            parachutesGOs[0].SetActive(!parachutesGOs[0].activeSelf);
        }
        else
        {
            parachutesGOs[1].SetActive(!parachutesGOs[1].activeSelf);
        }
    }
    public void InstantiateWeaponCatridge(GameObject caller, Weapon type)
    {
        Rigidbody2D catridgeRigidbody = Instantiate(catridgePrefabs[(int)type], caller.transform.position, new Quaternion()).GetComponent<Rigidbody2D>();
        catridgeRigidbody.gravityScale = 0;
        catridgeRigidbody.angularVelocity = Random.Range(catridgeSpinRange[0], catridgeSpinRange[1]);
        catridgeRigidbody.velocity = new Vector2(Random.Range(catridgeEjectionForceRange[0], catridgeEjectionForceRange[1]), Random.Range(catridgeEjectionForceRange[0], catridgeEjectionForceRange[1]));
    }
    public void RocketFeedback()
    {
        ShakeScreen(screenShakeIntensities[3]);
        SoundManager.Instance.PlaySound("bazooka_hit");
    }
    public void ShakeScreen(float intensity = 2f)
    {
        GameManager.Instance.screenShake.ShakeScreen(intensity);
    }
    public void UpdateCurrentWeaponSprite(GameObject caller, Weapon weapon)
    {
        if (caller.tag == "Player1")
        {
            muzzleFlashGOs[0].transform.localPosition = muzzleFlashPositions[(int)weapon];
            weaponSpritesGOs[0].GetComponent<SpriteRenderer>().sprite = weaponSprites[(int)weapon];
        }
        else
        {
            muzzleFlashGOs[1].transform.localPosition = muzzleFlashPositions[(int)weapon];
            weaponSpritesGOs[1].GetComponent<SpriteRenderer>().sprite = weaponSprites[(int)weapon];
        }
    }
    public void UpdateLives(GameObject caller, int lives)
    {
        if (caller.tag == "Player1")
        {
            text.text = lives.ToString();
        }
        else
        {

        }
    }
    public void UpdateHealth(GameObject caller)
    {
        healthImage.fillAmount = playerManager.Health;
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Awake()
    {
        _instance = this;
    }
    #endregion

}
