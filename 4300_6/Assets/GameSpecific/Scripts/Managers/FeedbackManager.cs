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
    [SerializeField] [Range(0, 2)] float[] screenShakeIntensities = new float[(int)Weapon.MINIGUN + 1];
    [SerializeField] float _bulletDestructionFeedbackLifetime = 0.5f;
    [SerializeField] float _sniperDestructionFeedbackLifetime = 0.5f;
    [SerializeField] float _bazookaDestructionFeedbackLifetime = 0.5f;
    [SerializeField] float hitAnimationDuration = 0.3f;
    [SerializeField] float parachuteDeployementTime = 0.2f;
    [SerializeField] float _stunEffectVerticalOffset = 0.6f;

    // References
    [SerializeField] Sprite[] weaponSprites = new Sprite[(int)Weapon.MINIGUN + 1];
    [SerializeField] Sprite[] projectileSprites = new Sprite[(int)Weapon.MINIGUN + 1];
    [SerializeField] Sprite[] destructionSprites = new Sprite[(int)Weapon.MINIGUN + 1];
    [SerializeField] Sprite[] muzzleFlashSprites = new Sprite[3];
    [SerializeField] Sprite[] playerHeadSprites = new Sprite[2];
    [SerializeField] Sprite[] parachuteSprites = new Sprite[3];
    SpriteRenderer[] parachutesSpriteRenderers = new SpriteRenderer[2];
    SpriteRenderer[] weaponSpriteRenderers = new SpriteRenderer[2];
    SpriteRenderer[] muzzleFlashSpriteRenderers = new SpriteRenderer[2];
    SpriteRenderer[] playerHeadSpriteRenderers = new SpriteRenderer[2];
    TMP_Text[] livesText = new TMP_Text[2];
    Image[] healthBarImages = new Image[2];

    // Prefabs
    [SerializeField] GameObject stunEffectPrefab = null;
    [SerializeField] GameObject[] catridgePrefabs = new GameObject[(int)Weapon.MINIGUN + 1];
    [SerializeField] GameObject ammoLeftPrefab = null;

    // Public properties
    public static FeedbackManager Instance => _instance;
    public float BulletDestructionFeedbackLifetime => _bulletDestructionFeedbackLifetime;
    public float SniperDestructionFeedbackLifetime => _sniperDestructionFeedbackLifetime;
    public float BazookaDestructionFeedbackLifetime => _bazookaDestructionFeedbackLifetime;
    public float StunEffectVerticalOffset => _stunEffectVerticalOffset;

    // Private variables
    static FeedbackManager _instance = null;
    bool[] isSwitchingMuzzleFlash = new bool[2];
    float[] parachuteDeployementTimer = new float[2];
    #endregion

    // Public methods
    #region Public methods
    public void DisplayMuzzleFlash(PlayerManager caller)
    {
        if (caller.TryingToFire && caller.CurrentAmmo > 0)
        {
            if (caller.tag == "Player1")
            {
                StartCoroutine(SwitchMuzzleFlashSprite(caller.WeaponsData[(int)caller.CurrentWeapon].firerate, 0));
            }
            else
            {
                StartCoroutine(SwitchMuzzleFlashSprite(caller.WeaponsData[(int)caller.CurrentWeapon].firerate, 1));
            }
        }
    }
    public void DisplayStunEffect(GameObject caller, float duration)
    {
        StunEffectController newStunEffect = Instantiate(stunEffectPrefab).GetComponent<StunEffectController>();
        newStunEffect.target = caller.transform;
        newStunEffect.lifetime = duration;
        newStunEffect.verticalOffset = StunEffectVerticalOffset;
    }
    public void DisplayAmmoLeft(GameObject caller, string ammoLeft, Color color, float lifetime)
    {
        Instantiate(ammoLeftPrefab).GetComponent<AmmoLeftTextController>().Init(gameObject.transform, caller.transform, outOfAmmoFeedback_horizontalOffset, outOfAmmoFeedback_verticalOffset, ammoLeft, color, outOfAmmoTextSize_words, outOfAmmoTextSize_numbers, lifetime);
    }
    public void DisplayHit(GameObject caller)
    {
        if (caller.tag == "Player1")
        {
            StartCoroutine(DisplayHurtHead(0));
        }
        else
        {
            StartCoroutine(DisplayHurtHead(1));
        }
    }
    public void DisplayBulletDestruction(Weapon type, SpriteRenderer renderer)
    {
        renderer.sprite = destructionSprites[(int)type];
    }
    public void DisplayAppropriateProjectile(Weapon type, SpriteRenderer renderer)
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
                        SoundManager.Instance.PlaySound("minigun_slowdown");
                    }
                    break;
            }
        }

    }
    public void PlayOutOfAmmoSound()
    {
        if (SoundManager.Instance != null)          SoundManager.Instance.PlayOutOfAmmoSound();             else Debug.LogWarning("Variable not set up!");
    }
    public void ToggleParachute(PlayerManager caller)
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

        if (parachutesSpriteRenderers[player].sprite == parachuteSprites[0]) // Parachute closed
        {
            parachutesSpriteRenderers[player].sprite = parachuteSprites[1];
            parachuteDeployementTimer[player] = parachuteDeployementTime;
        }
        else if (parachutesSpriteRenderers[player].sprite == parachuteSprites[1]) // In transition
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
        }
        else // Parachute open
        {
            parachutesSpriteRenderers[player].sprite = parachuteSprites[1];
            parachuteDeployementTimer[player] = parachuteDeployementTime;
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
        ShakeScreen(Weapon.BAZOOKA);
        SoundManager.Instance.PlaySound("bazooka_hit");
    }
    public void ShakeScreen(Weapon type)
    {
        GameManager.Instance.screenShake.ShakeScreen(screenShakeIntensities[(int)type]);
    }
    public void UpdateCurrentWeaponSprite(GameObject caller, Weapon weapon)
    {
        if (caller.tag == "Player1")
        {
            muzzleFlashSpriteRenderers[0].gameObject.transform.localPosition = muzzleFlashPositions[(int)weapon];
            weaponSpriteRenderers[0].sprite = weaponSprites[(int)weapon];
        }
        else
        {
            muzzleFlashSpriteRenderers[1].gameObject.transform.localPosition = muzzleFlashPositions[(int)weapon];
            weaponSpriteRenderers[1].sprite = weaponSprites[(int)weapon];
        }
    }
    public void UpdateLives(GameObject caller, int lives)
    {
        if (caller.tag == "Player1")
        {
            livesText[0].text = lives.ToString();
        }
        else
        {
            livesText[1].text = lives.ToString();
        }
    }
    public void UpdateHealth(GameObject caller, float health)
    {
        if (caller.tag == "Player1")
        {
            healthBarImages[0].fillAmount = health;
        }
        else
        {
            healthBarImages[1].fillAmount = health;
        }
    }
    #endregion

    // Private methods
    #region Private methods
    IEnumerator SwitchMuzzleFlashSprite(float frequency, int player)
    {
        isSwitchingMuzzleFlash[player] = true;

        if (muzzleFlashSpriteRenderers[0].sprite == muzzleFlashSprites[1])
        {
            muzzleFlashSpriteRenderers[0].sprite = muzzleFlashSprites[2];
        }
        else // If it's sprite n° 2 or a blank sprite.
        {
            muzzleFlashSpriteRenderers[0].sprite = muzzleFlashSprites[1];
        }

        yield return new WaitForSeconds(1/frequency);

        // Reset sprite to blank.
        muzzleFlashSpriteRenderers[0].sprite = muzzleFlashSprites[0];

        isSwitchingMuzzleFlash[player] = false;
    }
    IEnumerator DisplayHurtHead(int player)
    {
        playerHeadSpriteRenderers[player].sprite = playerHeadSprites[1];

        yield return new WaitForSeconds(hitAnimationDuration);

        playerHeadSpriteRenderers[player].sprite = playerHeadSprites[0];
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Awake()
    {
        _instance = this;
        PlayerManager player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerManager>();
        PlayerManager player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerManager>();
        parachutesSpriteRenderers[0] = player1.parachuteSpriteRenderer;
        parachutesSpriteRenderers[1] = player2.parachuteSpriteRenderer;
        weaponSpriteRenderers[0] = player1.weaponSpriteRenderer;
        weaponSpriteRenderers[1] = player2.weaponSpriteRenderer;
        muzzleFlashSpriteRenderers[0] = player1.muzzleFlashSpriteRenderer;
        muzzleFlashSpriteRenderers[1] = player2.muzzleFlashSpriteRenderer;
        playerHeadSpriteRenderers[0] = player1.headSpriteRenderer;
        playerHeadSpriteRenderers[1] = player2.headSpriteRenderer;

    }
    private void Start()
    {
        HealthAndLivesUIController ui = GameObject.FindGameObjectWithTag("HPandLivesUI").GetComponent<HealthAndLivesUIController>();
        livesText[0] = ui.livesText_player1;
        livesText[1] = ui.livesText_player2;
        healthBarImages[0] = ui.healthBar_player1;
        healthBarImages[1] = ui.healthBar_player2;
    }
    #endregion

}
