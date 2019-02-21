using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationAndOrientationController : MonoBehaviour
{
    // Classes and Enums
    #region Classes and Enums
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
    #endregion

    // Attributes
    #region Attributes
    // References
    [HideInInspector] public PlayerManager _playerManager = null;
    [SerializeField] GameObject parachuteGO = null;
    [SerializeField] GameObject _armsGO = null;
    [SerializeField] GameObject weaponGO = null;
    [SerializeField] GameObject muzzleFlashGO = null;
    [SerializeField] Vector2[] muzzleFlashPositions = new Vector2[(int)PlayerFiringController.Weapon.MINIGUN +1];
    [SerializeField] Sprite[] weaponsSprites = new Sprite[(int)PlayerFiringController.Weapon.MINIGUN + 1];
    [SerializeField] GameObject headGO = null;
    [SerializeField] Sprite[] heads = new Sprite[2];
    [SerializeField] GameObject stunPrefab = null;

    // Private variables
    PlayerDirection currentPlayerDirection;
    GunDirection currentGunDirection = GunDirection.FORWARD;
    SpriteRenderer currentWeaponSprite = null;
    SpriteRenderer muzzleFlashSprite = null;
    SpriteRenderer headSprite = null;
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
    public GameObject armsGO => _armsGO;
    #endregion

    // Public methods
    #region Public methods
    public void ToggleParachute()
    {
        parachuteGO.SetActive(!parachuteGO.activeSelf);
    }
    public void UpdateCurrentWeaponSprite(PlayerFiringController.Weapon weapon)
    {
        muzzleFlashGO.transform.localPosition = muzzleFlashPositions[(int)weapon];
        currentWeaponSprite.sprite = weaponsSprites[(int)weapon];
    }
    public void DisplayStun(float duration)
    {
        StunEffectController newStunEffect = Instantiate(stunPrefab).GetComponent<StunEffectController>();
        newStunEffect.target = gameObject;
        newStunEffect.lifetime = duration;
    }
    public void Init()
    {
        if (playerManager.isLeftPlayer)
        {
            currentPlayerDirection = PlayerDirection.RIGHT;
        }
        else
        {
            currentPlayerDirection = PlayerDirection.LEFT;
        }

        currentWeaponSprite = weaponGO.GetComponent<SpriteRenderer>();
        muzzleFlashSprite = weaponGO.GetComponent<SpriteRenderer>();
        headSprite = headGO.GetComponent<SpriteRenderer>();
    }
    #endregion

    // Private methods
    #region Private methods
    void HandlePlayerOrientation()
    {
        if (playerManager.horizontalInput != 0)
        {
            if (playerManager.horizontalInput > 0)
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
            // Face enemy if the joystick is in it's neutral position.
            if (CheckEnemyDirection())
            {
                currentPlayerDirection = PlayerDirection.RIGHT;
            }
            else
            {
                currentPlayerDirection = PlayerDirection.LEFT;
            }
        }
    }
    void HandleGunOrientation()
    {
        if (playerManager.currentMovementMode != PlayerMovementController.MovementMode.GROUND)
        {
            // If player is aiming up or down.
            if (Mathf.Abs(playerManager.aimingVerticalInput) >= Mathf.Abs(playerManager.aimingHorizontalInput))
            {
                if (playerManager.aimingVerticalInput > 0)
                {
                    // V+
                    currentGunDirection = GunDirection.DOWN;
                }
                else if (playerManager.aimingVerticalInput < 0)
                {
                    // V-
                    currentGunDirection = GunDirection.UP;
                }
                else
                {   // V = 0
                    currentGunDirection = GunDirection.FORWARD;
                }
            }
            else // If the player is aiming left or right.
            {
                currentGunDirection = GunDirection.FORWARD;
                // Change player orientation if the player is going one way and firing in the other.
                if (playerManager.aimingHorizontalInput < 0 && currentPlayerDirection == PlayerDirection.RIGHT)
                {
                    currentPlayerDirection = PlayerDirection.LEFT;
                }
                else if (playerManager.aimingHorizontalInput > 0 && currentPlayerDirection == PlayerDirection.LEFT)
                {
                    currentPlayerDirection = PlayerDirection.RIGHT;
                }
            }
        }
        else // If current movement mode is GROUND
        {
            // Handle gun orientation
            currentGunDirection = GunDirection.ANYWHERE;
        }
    }
    void OrientSpriteAndGun()
    {
        // Analyse inputs.
        HandlePlayerOrientation();
        HandleGunOrientation();

        // Apply orientations accordingly.
        if (playerManager.currentMovementMode != PlayerMovementController.MovementMode.GROUND)
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
                                    _armsGO.transform.localEulerAngles = new Vector3(0, 0, 0);
                                }
                                break;
                            case GunDirection.UP:
                                {
                                    _armsGO.transform.localEulerAngles = new Vector3(0, 0, -90);
                                }
                                break;
                            case GunDirection.DOWN:
                                {
                                    _armsGO.transform.localEulerAngles = new Vector3(0, 0, 90);
                                }
                                break;
                            case GunDirection.ANYWHERE:
                                {
                                    _armsGO.transform.localEulerAngles = new Vector3(0, 180, Mathf.Atan2(playerManager.aimingVerticalInput, playerManager.aimingHorizontalInput) * Mathf.Rad2Deg);
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
                                    _armsGO.transform.localEulerAngles = new Vector3(0, 0, 0);
                                }
                                break;
                            case GunDirection.UP:
                                {
                                    _armsGO.transform.localEulerAngles = new Vector3(0, 0, -90);
                                }
                                break;
                            case GunDirection.DOWN:
                                {
                                    _armsGO.transform.localEulerAngles = new Vector3(0, 0, 90);
                                }
                                break;
                            case GunDirection.ANYWHERE:
                                {
                                    _armsGO.transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(playerManager.aimingVerticalInput, playerManager.aimingHorizontalInput) * Mathf.Rad2Deg);
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        else // If it is GROUND
        {
            if (playerManager.aimingHorizontalInput > 0)
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
                _armsGO.transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(playerManager.aimingVerticalInput, playerManager.aimingHorizontalInput) * Mathf.Rad2Deg);
            }
            else if (playerManager.aimingHorizontalInput < 0)
            {
                transform.localEulerAngles = new Vector3(0, 180, 0);
                _armsGO.transform.localEulerAngles = new Vector3(180, 180, -Mathf.Atan2(playerManager.aimingVerticalInput, playerManager.aimingHorizontalInput) * Mathf.Rad2Deg);
            }
            else
            {
                if (CheckEnemyDirection())
                {
                    // enemy on the right
                    transform.localEulerAngles = new Vector3(0, 0, 0);
                    _armsGO.transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(playerManager.aimingVerticalInput, playerManager.aimingHorizontalInput) * Mathf.Rad2Deg);
                }
                else
                {
                    // enemy on the left
                    transform.localEulerAngles = new Vector3(0, 180, 0);
                }
            }
        }
    }
    bool CheckEnemyDirection() // Returns true if the enemy is on the right or at the same x axis, false if he is on the left.
    {
        if (playerManager.isLeftPlayer)
        {
            if ((GameManager.Instance.Player2.gameObject.transform.position - transform.position).x >= 0)
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
            if ((GameManager.Instance.Player1.gameObject.transform.position - transform.position).x >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    void DisplayMuzzleFlash()
    {
        if (playerManager.tryingToFire && playerManager.currentAmmo > 0)
        {
            if (!muzzleFlashGO.activeSelf)
            {
                muzzleFlashGO.SetActive(true);
                headSprite.sprite = heads[1];
            }
        }
        else
        {
            if (muzzleFlashGO.activeSelf)
            {
                muzzleFlashGO.SetActive(false);
                headSprite.sprite = heads[0];
            }
        }
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void FixedUpdate()
    {
        OrientSpriteAndGun();
        DisplayMuzzleFlash();
    }
    #endregion
}
